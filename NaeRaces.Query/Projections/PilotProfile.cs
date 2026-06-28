using EventDbLite;
using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NaeRaces.Query.Projections;

public class PilotProfile : ContextProjection
{
    public record PilotProfileResult(
        Guid PilotId,
        string CallSign,
        string Email,
        string UserName,
        string Name,
        string Nationality,
        DateTime? DateOfBirth,
        IEnumerable<string> Roles,
        IEnumerable<PilotGovernmentDocumentValidation> GovernmentDocumentValidations,
        IEnumerable<PilotInsuranceValidation> InsuranceValidations,
        IEnumerable<PilotDateOfBirthValidation> DateOfBirthValidations,
        IEnumerable<PilotClubResult> Clubs);

    public record PilotGovernmentDocumentValidation(string GovernmentDocument, DateTime ValidUntil, Guid ClubId, Guid ValidatedByPilotId, bool IsValidatingMemberOnCommiteeOfClub);
    public record PilotInsuranceValidation(string InsuranceProvider, DateTime ValidUntil, Guid ClubId, Guid ValidatedByPilotId, bool IsValidatingMemberOnCommiteeOfClub);
    public record PilotDateOfBirthValidation(Guid ClubId, Guid ValidatedByPilotId, bool IsValidatingMemberOnCommiteeOfClub);

    public record PilotClubResult(Guid ClubId, string ClubCode, string ClubName, PilotClubRelationship Relationship);
    public record PilotClubRelationship(PilotClubMembership? Membership, bool IsFollowing, IEnumerable<string> Roles);
    public record PilotClubMembership(int MembershipLevelId, string MembershipLevelName, int PaymentOptionId, string PaymentOptionName, bool IsConfirmed, DateTime? ValidUntil);

    private class PilotProfileProjectionSnapshot
    {
        public record PilotData(
            Guid PilotId,
            string CallSign,
            string Email,
            string UserName,
            string Name,
            string Nationality,
            DateTime? DateOfBirth,
            IEnumerable<string> Roles,
            IEnumerable<PilotGovernmentDocumentValidation> GovernmentDocumentValidations,
            IEnumerable<PilotInsuranceValidation> InsuranceValidations,
            IEnumerable<PilotDateOfBirthValidation> DateOfBirthValidations);

        public record ClubInfo(Guid ClubId, string Code, string Name, List<ClubMembershipLevel> MembershipLevels);
        public record ClubMembershipLevel(int Id, string Name, List<ClubMembershipPaymentOption> PaymentOptions);
        public record ClubMembershipPaymentOption(int Id, string Name);
        public record PilotClubRelationship(Guid PilotId, Guid ClubId, PilotClubMembership? Membership, bool IsFollowing, IEnumerable<string> Roles);
        public record PilotClubMembership(int MembershipLevelId, int PaymentOptionId, bool IsConfirmed, DateTime? ValidUntil);

        public IEnumerable<PilotData> Pilots { get; set; } = Enumerable.Empty<PilotData>();
        public IEnumerable<ClubInfo> Clubs { get; set; } = Enumerable.Empty<ClubInfo>();
        public IEnumerable<PilotClubRelationship> Relationships { get; set; } = Enumerable.Empty<PilotClubRelationship>();
    }

    private record PaymentOptionDetails(int Id, string Name);
    private record MembershipDetails(int Id, string Name, List<PaymentOptionDetails> PaymentOptions);
    private record ClubDetails(string Code, string Name, List<MembershipDetails> MembershipDetails);
    private record ClubMembership(int MembershipLevelId, int PaymentOptionId, bool IsConfirmed, DateTime? ValidUntil);
    private record ClubRelationship(HashSet<string> Roles, bool IsFollowing, ClubMembership? Membership);
    private record PilotDetails(
        string CallSign,
        string Email,
        string UserName,
        string Name,
        string Nationality,
        DateTime? DateOfBirth,
        HashSet<string> Roles,
        List<PilotGovernmentDocumentValidation> GovernmentDocumentValidations,
        List<PilotInsuranceValidation> InsuranceValidations,
        List<PilotDateOfBirthValidation> DateOfBirthValidations);

    private readonly Dictionary<Guid, PilotDetails> _pilotInfo = new();
    private readonly Dictionary<Guid, ClubDetails> _clubInfo = new();
    private readonly Dictionary<Guid, Dictionary<Guid, ClubRelationship>> _pilotClubRelationships = new();

    public PilotProfileResult? GetPilotProfile(Guid pilotId)
    {
        if (!_pilotInfo.TryGetValue(pilotId, out var pilot))
        {
            return null;
        }

        _pilotClubRelationships.TryGetValue(pilotId, out var clubRelationships);

        List<PilotClubResult> clubs = new();

        if (clubRelationships is not null)
        {
            foreach (var relationshipKvp in clubRelationships)
            {
                if (!_clubInfo.TryGetValue(relationshipKvp.Key, out var club))
                {
                    continue;
                }

                var relationship = relationshipKvp.Value;
                if (!relationship.IsFollowing && relationship.Membership is null && relationship.Roles.Count == 0)
                {
                    continue;
                }

                PilotClubMembership? membership = null;
                if (relationship.Membership is not null)
                {
                    var membershipDetails = club.MembershipDetails.FirstOrDefault(x => x.Id == relationship.Membership.MembershipLevelId);
                    var paymentOption = membershipDetails?.PaymentOptions.FirstOrDefault(x => x.Id == relationship.Membership.PaymentOptionId);

                    if (membershipDetails is not null && paymentOption is not null)
                    {
                        membership = new PilotClubMembership(
                            relationship.Membership.MembershipLevelId,
                            membershipDetails.Name,
                            relationship.Membership.PaymentOptionId,
                            paymentOption.Name,
                            relationship.Membership.IsConfirmed,
                            relationship.Membership.ValidUntil);
                    }
                }

                clubs.Add(new PilotClubResult(
                    relationshipKvp.Key,
                    club.Code,
                    club.Name,
                    new PilotClubRelationship(membership, relationship.IsFollowing, relationship.Roles)));
            }
        }

        return new PilotProfileResult(
            pilotId,
            pilot.CallSign,
            pilot.Email,
            pilot.UserName,
            pilot.Name,
            pilot.Nationality,
            pilot.DateOfBirth,
            pilot.Roles,
            pilot.GovernmentDocumentValidations,
            pilot.InsuranceValidations,
            pilot.DateOfBirthValidations,
            clubs);
    }

    private void Restore(PilotProfileProjectionSnapshot snapshot)
    {
        foreach (var pilot in snapshot.Pilots)
        {
            _pilotInfo[pilot.PilotId] = new PilotDetails(
                pilot.CallSign,
                pilot.Email,
                pilot.UserName,
                pilot.Name,
                pilot.Nationality,
                pilot.DateOfBirth,
                new HashSet<string>(pilot.Roles),
                pilot.GovernmentDocumentValidations.ToList(),
                pilot.InsuranceValidations.ToList(),
                pilot.DateOfBirthValidations.ToList());
        }

        foreach (var club in snapshot.Clubs)
        {
            List<MembershipDetails> levels = [];

            foreach (var level in club.MembershipLevels)
            {
                List<PaymentOptionDetails> options = [];

                foreach (var option in level.PaymentOptions)
                {
                    options.Add(new PaymentOptionDetails(option.Id, option.Name));
                }

                levels.Add(new MembershipDetails(level.Id, level.Name, options));
            }

            _clubInfo[club.ClubId] = new ClubDetails(club.Code, club.Name, levels);
        }

        foreach (var relationship in snapshot.Relationships)
        {
            if (!_pilotClubRelationships.TryGetValue(relationship.PilotId, out var clubRelationships))
            {
                clubRelationships = new Dictionary<Guid, ClubRelationship>();
                _pilotClubRelationships[relationship.PilotId] = clubRelationships;
            }

            ClubMembership? membership = null;

            if (relationship.Membership is not null)
            {
                membership = new ClubMembership(
                    relationship.Membership.MembershipLevelId,
                    relationship.Membership.PaymentOptionId,
                    relationship.Membership.IsConfirmed,
                    relationship.Membership.ValidUntil);
            }

            clubRelationships[relationship.ClubId] = new ClubRelationship(new HashSet<string>(relationship.Roles), relationship.IsFollowing, membership);
        }
    }

    private PilotProfileProjectionSnapshot Snapshot()
    {
        List<PilotProfileProjectionSnapshot.PilotData> pilots = [];

        foreach (var pilotKvp in _pilotInfo)
        {
            pilots.Add(new PilotProfileProjectionSnapshot.PilotData(
                pilotKvp.Key,
                pilotKvp.Value.CallSign,
                pilotKvp.Value.Email,
                pilotKvp.Value.UserName,
                pilotKvp.Value.Name,
                pilotKvp.Value.Nationality,
                pilotKvp.Value.DateOfBirth,
                pilotKvp.Value.Roles,
                pilotKvp.Value.GovernmentDocumentValidations,
                pilotKvp.Value.InsuranceValidations,
                pilotKvp.Value.DateOfBirthValidations));
        }

        List<PilotProfileProjectionSnapshot.ClubInfo> clubs = [];

        foreach (var clubKvp in _clubInfo)
        {
            List<PilotProfileProjectionSnapshot.ClubMembershipLevel> levels = [];

            foreach (var level in clubKvp.Value.MembershipDetails)
            {
                List<PilotProfileProjectionSnapshot.ClubMembershipPaymentOption> options = [];

                foreach (var option in level.PaymentOptions)
                {
                    options.Add(new PilotProfileProjectionSnapshot.ClubMembershipPaymentOption(option.Id, option.Name));
                }

                levels.Add(new PilotProfileProjectionSnapshot.ClubMembershipLevel(level.Id, level.Name, options));
            }

            clubs.Add(new PilotProfileProjectionSnapshot.ClubInfo(clubKvp.Key, clubKvp.Value.Code, clubKvp.Value.Name, levels));
        }

        List<PilotProfileProjectionSnapshot.PilotClubRelationship> relationships = [];

        foreach (var pilotKvp in _pilotClubRelationships)
        {
            foreach (var clubKvp in pilotKvp.Value)
            {
                PilotProfileProjectionSnapshot.PilotClubMembership? membership = null;

                if (clubKvp.Value.Membership is not null)
                {
                    membership = new PilotProfileProjectionSnapshot.PilotClubMembership(
                        clubKvp.Value.Membership.MembershipLevelId,
                        clubKvp.Value.Membership.PaymentOptionId,
                        clubKvp.Value.Membership.IsConfirmed,
                        clubKvp.Value.Membership.ValidUntil);
                }

                relationships.Add(new PilotProfileProjectionSnapshot.PilotClubRelationship(
                    pilotKvp.Key,
                    clubKvp.Key,
                    membership,
                    clubKvp.Value.IsFollowing,
                    clubKvp.Value.Roles));
            }
        }

        return new PilotProfileProjectionSnapshot()
        {
            Pilots = pilots,
            Clubs = clubs,
            Relationships = relationships
        };
    }

    private void When(PilotRegistered registered)
    {
        _pilotInfo[registered.PilotId] = new PilotDetails(
            registered.CallSign,
            registered.Email,
            registered.UserName,
            string.Empty,
            string.Empty,
            null,
            new HashSet<string>(),
            [],
            [],
            []);
    }

    private void When(PilotCallSignChanged changed)
    {
        if (_pilotInfo.TryGetValue(changed.PilotId, out var pilot))
        {
            _pilotInfo[changed.PilotId] = pilot with { CallSign = changed.NewCallSign };
        }
    }

    private void When(PilotNameSet nameSet)
    {
        if (_pilotInfo.TryGetValue(nameSet.PilotId, out var pilot))
        {
            _pilotInfo[nameSet.PilotId] = pilot with { Name = nameSet.Name };
        }
    }

    private void When(PilotNationalitySet nationalitySet)
    {
        if (_pilotInfo.TryGetValue(nationalitySet.PilotId, out var pilot))
        {
            _pilotInfo[nationalitySet.PilotId] = pilot with { Nationality = nationalitySet.Nationality };
        }
    }

    private void When(PilotDateOfBirthSet dobSet)
    {
        if (_pilotInfo.TryGetValue(dobSet.PilotId, out var pilot))
        {
            _pilotInfo[dobSet.PilotId] = pilot with { DateOfBirth = dobSet.DateOfBirth };
        }
    }

    private void When(PilotPasswordChanged changed)
    {
    }

    private void When(PilotRoleAssigned assigned)
    {
        if (_pilotInfo.TryGetValue(assigned.PilotId, out var pilot))
        {
            pilot.Roles.Add(assigned.Role);
        }
    }

    private void When(PilotRoleRemoved removed)
    {
        if (_pilotInfo.TryGetValue(removed.PilotId, out var pilot))
        {
            pilot.Roles.Remove(removed.Role);
        }
    }

    private void When(PilotGovernmentDocumentationValidatedByPeer validated)
    {
        if (_pilotInfo.TryGetValue(validated.PilotId, out var pilot))
        {
            pilot.GovernmentDocumentValidations.Add(new PilotGovernmentDocumentValidation(
                validated.GovernmentDocument,
                validated.ValidUntil,
                validated.ClubId,
                validated.ValidatedByPilotId,
                validated.IsValidatingMemberOnCommiteeOfClub));
        }
    }

    private void When(PilotInsuranceValidatedByPeer validated)
    {
        if (_pilotInfo.TryGetValue(validated.PilotId, out var pilot))
        {
            pilot.InsuranceValidations.Add(new PilotInsuranceValidation(
                validated.InsuranceProvider,
                validated.ValidUntil,
                validated.ClubId,
                validated.ValidatedByPilotId,
                validated.IsValidatingMemberOnCommiteeOfClub));
        }
    }

    private void When(PilotDateOfBirthValidatedByPeer validated)
    {
        if (_pilotInfo.TryGetValue(validated.PilotId, out var pilot))
        {
            pilot.DateOfBirthValidations.Add(new PilotDateOfBirthValidation(
                validated.ClubId,
                validated.ValidatedByPilotId,
                validated.IsValidatingMemberOnCommiteeOfClub));
        }
    }

    private void When(ClubFormed formed)
    {
        _clubInfo[formed.ClubId] = new ClubDetails(formed.Code, formed.Name, []);
    }

    private void When(ClubDetailsChanged changed)
    {
        if (_clubInfo.TryGetValue(changed.ClubId, out var club))
        {
            _clubInfo[changed.ClubId] = club with { Code = changed.Code, Name = changed.Name };
        }
    }

    private void When(ClubMembershipLevelAdded added)
    {
        if (_clubInfo.TryGetValue(added.ClubId, out var club))
        {
            club.MembershipDetails.Add(new MembershipDetails(added.MembershipLevelId, added.Name, []));
        }
    }

    private void When(ClubMembershipLevelRemoved removed)
    {
        if (!_clubInfo.TryGetValue(removed.ClubId, out var club))
        {
            return;
        }

        var toRemove = club.MembershipDetails.FirstOrDefault(x => x.Id == removed.MembershipLevelId);
        if (toRemove is not null)
        {
            club.MembershipDetails.Remove(toRemove);
        }
    }

    private void When(ClubMembershipLevelRenamed renamed)
    {
        if (!_clubInfo.TryGetValue(renamed.ClubId, out var club))
        {
            return;
        }

        var index = club.MembershipDetails.FindIndex(x => x.Id == renamed.MembershipLevelId);
        if (index >= 0)
        {
            club.MembershipDetails[index] = club.MembershipDetails[index] with { Name = renamed.NewName };
        }
    }

    private void When(ClubMembershipLevelAnnualPaymentOptionAdded optionAdded) => AddPaymentOption(optionAdded.ClubId, optionAdded.MembershipLevelId, optionAdded.PaymentOptionId, optionAdded.Name);
    private void When(ClubMembershipLevelMonthlyPaymentOptionAdded optionAdded) => AddPaymentOption(optionAdded.ClubId, optionAdded.MembershipLevelId, optionAdded.PaymentOptionId, optionAdded.Name);
    private void When(ClubMembershipLevelSubscriptionPaymentOptionAdded optionAdded) => AddPaymentOption(optionAdded.ClubId, optionAdded.MembershipLevelId, optionAdded.PaymentOptionId, optionAdded.Name);

    private void AddPaymentOption(Guid clubId, int membershipLevelId, int paymentOptionId, string name)
    {
        if (_clubInfo.TryGetValue(clubId, out var club))
        {
            club.MembershipDetails.FirstOrDefault(x => x.Id == membershipLevelId)?.PaymentOptions.Add(new PaymentOptionDetails(paymentOptionId, name));
        }
    }

    private void When(ClubMembershipLevelPaymentOptionRemoved removedOption)
    {
        if (!_clubInfo.TryGetValue(removedOption.ClubId, out var club))
        {
            return;
        }

        var level = club.MembershipDetails.FirstOrDefault(x => x.Id == removedOption.MembershipLevelId);
        var toRemove = level?.PaymentOptions.FirstOrDefault(x => x.Id == removedOption.PaymentOptionId);

        if (toRemove is not null)
        {
            level!.PaymentOptions.Remove(toRemove);
        }
    }

    private void When(ClubMembershipLevelPaymentOptionRenamed renamedOption)
    {
        if (!_clubInfo.TryGetValue(renamedOption.ClubId, out var club))
        {
            return;
        }

        var level = club.MembershipDetails.FirstOrDefault(x => x.Id == renamedOption.MembershipLevelId);
        if (level is null)
        {
            return;
        }

        var optionIndex = level.PaymentOptions.FindIndex(x => x.Id == renamedOption.PaymentOptionId);
        if (optionIndex >= 0)
        {
            level.PaymentOptions[optionIndex] = level.PaymentOptions[optionIndex] with { Name = renamedOption.NewName };
        }
    }

    private void When(PilotFollowedClub followed)
    {
        var relationship = GetOrCreateRelationship(followed.PilotId, followed.ClubId);
        _pilotClubRelationships[followed.PilotId][followed.ClubId] = relationship with { IsFollowing = true };
    }

    private void When(PilotUnfollowedClub unfollowed)
    {
        var relationship = GetOrCreateRelationship(unfollowed.PilotId, unfollowed.ClubId);
        _pilotClubRelationships[unfollowed.PilotId][unfollowed.ClubId] = relationship with { IsFollowing = false };
    }

    private void When(ClubMemberRoleAssigned assigned)
    {
        var relationship = GetOrCreateRelationship(assigned.PilotId, assigned.ClubId);
        relationship.Roles.Add(assigned.Role);
    }

    private void When(ClubMemberRoleRevoked revoked)
    {
        var relationship = GetOrCreateRelationship(revoked.PilotId, revoked.ClubId);
        relationship.Roles.Remove(revoked.Role);
    }

    private void When(PilotRegisteredForClubMembershipLevel registered)
    {
        var relationship = GetOrCreateRelationship(registered.PilotId, registered.ClubId);
        _pilotClubRelationships[registered.PilotId][registered.ClubId] = relationship with
        {
            Membership = new ClubMembership(registered.MembershipLevelId, registered.PaymentOptionId, false, null)
        };
    }

    private void When(PilotClubMembershipConfirmed confirmed) => ConfirmMembership(confirmed.PilotId, confirmed.ClubId, confirmed.MembershipLevelId, confirmed.PaymentOptionId, confirmed.ValidUntil);
    private void When(PilotClubMembershipManuallyConfirmed confirmed) => ConfirmMembership(confirmed.PilotId, confirmed.ClubId, confirmed.MembershipLevelId, confirmed.PaymentOptionId, confirmed.ValidUntil);

    private void ConfirmMembership(Guid pilotId, Guid clubId, int membershipLevelId, int paymentOptionId, DateTime? validUntil)
    {
        var relationship = GetOrCreateRelationship(pilotId, clubId);
        _pilotClubRelationships[pilotId][clubId] = relationship with
        {
            Membership = new ClubMembership(membershipLevelId, paymentOptionId, true, validUntil)
        };
    }

    private void When(PilotClubMembershipRenewed renewed)
    {
        var relationship = GetOrCreateRelationship(renewed.PilotId, renewed.ClubId);

        if (relationship.Membership is null)
        {
            return;
        }

        _pilotClubRelationships[renewed.PilotId][renewed.ClubId] = relationship with
        {
            Membership = relationship.Membership with { ValidUntil = renewed.NewValidUntil }
        };
    }

    private void When(PilotClubMembershipExpired expired) => RemoveMembership(expired.PilotId, expired.ClubId);
    private void When(PilotClubMembershipCancelled cancelled) => RemoveMembership(cancelled.PilotId, cancelled.ClubId);
    private void When(PilotClubMembershipRevoked revoked) => RemoveMembership(revoked.PilotId, revoked.ClubId);

    private void RemoveMembership(Guid pilotId, Guid clubId)
    {
        var relationship = GetOrCreateRelationship(pilotId, clubId);
        _pilotClubRelationships[pilotId][clubId] = relationship with { Membership = null };
    }

    private ClubRelationship GetOrCreateRelationship(Guid pilotId, Guid clubId)
    {
        if (!_pilotClubRelationships.TryGetValue(pilotId, out var clubRelationships))
        {
            clubRelationships = new Dictionary<Guid, ClubRelationship>();
            _pilotClubRelationships[pilotId] = clubRelationships;
        }

        if (!clubRelationships.TryGetValue(clubId, out var relationship))
        {
            relationship = new ClubRelationship([], false, null);
            clubRelationships[clubId] = relationship;
        }

        return relationship;
    }
}
