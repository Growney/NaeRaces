using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;

namespace NaeRaces.Query.Projections;

public class PilotRelevantClubsProjection
{
    public record PilotRelevantClub(Guid PilotId, Guid ClubId, string ClubCode, string ClubName, PilotClubRelationship Relationship);
    public record PilotClubRelationship(PilotClubMembership? Membership, bool IsFollowing, IEnumerable<string> Roles);
    public record PilotClubMembership(int MembershipLevelId, string MembershipName, int PaymentOptionId, string PaymentOptionName, bool IsConfirmed, DateTime? Expiry);

    private class PilotRelevantClubsProjectionSnapshot
    {
        public record ClubInfo(Guid ClubId, string Code, string Name, List<ClubMembershipLevel> MembershipLevels);
        public record ClubMembershipLevel(int Id, string Name, List<ClubMembershipPaymentOption> PaymentOptions);
        public record ClubMembershipPaymentOption(int Id, string Name);
        public record PilotClubRelationship(Guid PilotId, Guid ClubId, PilotClubMembership? Membership, bool IsFollowing, IEnumerable<string> Roles);
        public record PilotClubMembership(int MembershipLevelId, int PaymentOptionId, bool IsConfirmed, DateTime? Expiry);

        public IEnumerable<ClubInfo> Clubs { get; set; } = Enumerable.Empty<ClubInfo>();
        public IEnumerable<PilotClubRelationship> Relationships { get; set; } = Enumerable.Empty<PilotClubRelationship>();

    }

    private void Restore(PilotRelevantClubsProjectionSnapshot snapshot)
    {
        foreach (var club in snapshot.Clubs)
        {
            List<MembershipDetails> membershipLevels = new();

            foreach (var level in club.MembershipLevels)
            {
                List<PaymentOptionDetails> paymentOptions = new();

                foreach (var option in level.PaymentOptions)
                {
                    paymentOptions.Add(new(option.Id, option.Name));
                }
                membershipLevels.Add(new(level.Id, level.Name, paymentOptions));
            }

            _clubInfo.Add(club.ClubId, new(club.Code, club.Name, membershipLevels));
        }

        foreach (var relationship in snapshot.Relationships)
        {
            if (!_pilotClubRelationships.TryGetValue(relationship.PilotId, out var clubRelationships))
            {
                clubRelationships = new Dictionary<Guid, ClubRelationship>();
                _pilotClubRelationships.Add(relationship.PilotId, clubRelationships);
            }
            ClubMembership? membership = null;

            if (relationship.Membership is not null)
            {
                membership = new ClubMembership(relationship.Membership.MembershipLevelId, relationship.Membership.PaymentOptionId, relationship.Membership.IsConfirmed, relationship.Membership.Expiry);
            }

            clubRelationships[relationship.ClubId] = new ClubRelationship(relationship.Roles.ToList(), relationship.IsFollowing, membership);
        }
    }

    private PilotRelevantClubsProjectionSnapshot Snapshot()
    {
        List<PilotRelevantClubsProjectionSnapshot.ClubInfo> clubs = new();

        foreach (var clubKvp in _clubInfo)
        {
            List<PilotRelevantClubsProjectionSnapshot.ClubMembershipLevel> levels = new List<PilotRelevantClubsProjectionSnapshot.ClubMembershipLevel>();

            foreach (var level in clubKvp.Value.MembershipDetails)
            {
                List<PilotRelevantClubsProjectionSnapshot.ClubMembershipPaymentOption> paymentOptions = new List<PilotRelevantClubsProjectionSnapshot.ClubMembershipPaymentOption>();
                foreach (var option in level.PaymentOptions)
                {
                    paymentOptions.Add(new PilotRelevantClubsProjectionSnapshot.ClubMembershipPaymentOption(option.Id, option.Name));
                }
                levels.Add(new PilotRelevantClubsProjectionSnapshot.ClubMembershipLevel(level.Id, level.Name, paymentOptions));
            }

            clubs.Add(new PilotRelevantClubsProjectionSnapshot.ClubInfo(clubKvp.Key, clubKvp.Value.Code, clubKvp.Value.Name, levels));
        }

        List<PilotRelevantClubsProjectionSnapshot.PilotClubRelationship> relationships = new();

        foreach (var pilotKvp in _pilotClubRelationships)
        {
            foreach (var clubKvp in pilotKvp.Value)
            {
                PilotRelevantClubsProjectionSnapshot.PilotClubMembership? membership = null;
                if (clubKvp.Value.Membership is not null)
                {
                    membership = new PilotRelevantClubsProjectionSnapshot.PilotClubMembership(clubKvp.Value.Membership.MembershipLevelId, clubKvp.Value.Membership.PaymentOptionId, clubKvp.Value.Membership.IsConfirmed, clubKvp.Value.Membership.Expiry);
                }

                relationships.Add(new PilotRelevantClubsProjectionSnapshot.PilotClubRelationship(pilotKvp.Key, clubKvp.Key, membership, clubKvp.Value.IsFollowing, clubKvp.Value.Roles));
            }
        }

        return new PilotRelevantClubsProjectionSnapshot()
        {
            Clubs = clubs,
            Relationships = relationships
        };
    }

    private record PaymentOptionDetails(int Id, string Name);
    private record MembershipDetails(int Id, string Name, List<PaymentOptionDetails> PaymentOptions);
    private record ClubDetails(string Code, string Name, List<MembershipDetails> MembershipDetails);
    private record ClubMembership(int MembershipLevelId, int PaymentOptionId, bool IsConfirmed, DateTime? Expiry);
    private record ClubRelationship(List<string> Roles, bool IsFollowing, ClubMembership? Membership);

    private Dictionary<Guid, ClubDetails> _clubInfo = new();
    private Dictionary<Guid, Dictionary<Guid, ClubRelationship>> _pilotClubRelationships = new();

    public IEnumerable<PilotRelevantClub> GetRelevantClubs(Guid pilotId)
    {
        if (!_pilotClubRelationships.TryGetValue(pilotId, out var clubRelationships))
        {
            yield break;
        }

        foreach (var relationshipKvp in clubRelationships)
        {
            if (!_clubInfo.TryGetValue(relationshipKvp.Key, out var clubInfo))
            {
                continue;
            }
            PilotClubMembership? membership = null;

            if (relationshipKvp.Value.Membership is not null)
            {
                ClubMembership clubMembership = relationshipKvp.Value.Membership;

                MembershipDetails? membershipDetails = clubInfo.MembershipDetails.FirstOrDefault(x => x.Id == clubMembership.MembershipLevelId);
                PaymentOptionDetails? paymentOptionDetails = membershipDetails?.PaymentOptions.FirstOrDefault(x => x.Id == clubMembership.PaymentOptionId);

                if (membershipDetails is not null && paymentOptionDetails is not null)
                {
                    membership = new PilotClubMembership(clubMembership.MembershipLevelId, membershipDetails.Name, clubMembership.PaymentOptionId, paymentOptionDetails.Name, clubMembership.IsConfirmed, clubMembership.Expiry);
                }
            }

            PilotClubRelationship relationship = new(membership, relationshipKvp.Value.IsFollowing, relationshipKvp.Value.Roles);

            yield return new PilotRelevantClub(pilotId, relationshipKvp.Key, clubInfo.Code, clubInfo.Name, relationship);
        }
    }

    private void When(ClubDetailsChanged changed)
    {
        _clubInfo[changed.ClubId] = _clubInfo[changed.ClubId] with { Code = changed.Code, Name = changed.Name };
    }
    private void When(ClubFormed formed)
    {
        _clubInfo[formed.ClubId] = new ClubDetails(formed.Code, formed.Name, new List<MembershipDetails>());
    }

    private void When(ClubMembershipLevelAdded added)
    {
        _clubInfo[added.ClubId].MembershipDetails.Add(new MembershipDetails(added.MembershipLevelId, added.Name, new List<PaymentOptionDetails>()));
    }
    private void When(ClubMembershipLevelRemoved removed)
    {
        var toRemove = _clubInfo[removed.ClubId].MembershipDetails.FirstOrDefault(x => x.Id == removed.MembershipLevelId);
        if (toRemove is null)
        {
            return;
        }

        _clubInfo[removed.ClubId].MembershipDetails.Remove(toRemove);
    }
    private void When(ClubMembershipLevelRenamed renamed)
    {
        var indexOf = _clubInfo[renamed.ClubId].MembershipDetails.FindIndex(x => x.Id == renamed.MembershipLevelId);

        if (indexOf < 0)
        {
            return;
        }

        _clubInfo[renamed.ClubId].MembershipDetails[indexOf] = _clubInfo[renamed.ClubId].MembershipDetails[indexOf] with { Name = renamed.NewName };
    }
    private void When(ClubMembershipLevelAnnualPaymentOptionAdded optionAdded) => AddOption(optionAdded.ClubId, optionAdded.MembershipLevelId, optionAdded.PaymentOptionId, optionAdded.Name);
    private void When(ClubMembershipLevelMonthlyPaymentOptionAdded optionAdded) => AddOption(optionAdded.ClubId, optionAdded.MembershipLevelId, optionAdded.PaymentOptionId, optionAdded.Name);
    private void When(ClubMembershipLevelSubscriptionPaymentOptionAdded optionAdded) => AddOption(optionAdded.ClubId, optionAdded.MembershipLevelId, optionAdded.PaymentOptionId, optionAdded.Name);

    private void AddOption(Guid clubId, int membershipLevelId, int paymentOptionId, string name)
    {
        _clubInfo[clubId].MembershipDetails.FirstOrDefault(x => x.Id == membershipLevelId)?.PaymentOptions.Add(new PaymentOptionDetails(paymentOptionId, name));
    }

    private void When(ClubMembershipLevelPaymentOptionRemoved removedOption)
    {
        var membershipDetails = _clubInfo[removedOption.ClubId].MembershipDetails.FirstOrDefault(x => x.Id == removedOption.MembershipLevelId);

        if (membershipDetails is null)
        {
            return;
        }

        var toRemove = membershipDetails.PaymentOptions.FirstOrDefault(x => x.Id == removedOption.PaymentOptionId);

        if (toRemove is null)
        {
            return;
        }

        membershipDetails.PaymentOptions.Remove(toRemove);
    }

    private void When(ClubMembershipLevelPaymentOptionRenamed renamedOption)
    {
        var membershipDetails = _clubInfo[renamedOption.ClubId].MembershipDetails.FirstOrDefault(x => x.Id == renamedOption.MembershipLevelId);

        if (membershipDetails is null)
        {
            return;
        }

        var optionIndex = membershipDetails.PaymentOptions.FindIndex(x => x.Id == renamedOption.PaymentOptionId);

        if (optionIndex < 0)
        {
            return;
        }

        membershipDetails.PaymentOptions[optionIndex] = membershipDetails.PaymentOptions[optionIndex] with { Name = renamedOption.NewName };
    }

    private void When(PilotFollowedClub followed)
    {
        if (!_pilotClubRelationships.TryGetValue(followed.PilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(followed.PilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(followed.ClubId, out var relationship))
        {
            clubRelationships[followed.ClubId] = new ClubRelationship([], true, null);
        }
        else
        {
            clubRelationships[followed.ClubId] = clubRelationships[followed.ClubId] with { IsFollowing = true };
        }
    }
    private void When(PilotUnfollowedClub unfollowed)
    {
        if (!_pilotClubRelationships.TryGetValue(unfollowed.PilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(unfollowed.PilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(unfollowed.ClubId, out var relationship))
        {
            clubRelationships[unfollowed.ClubId] = new ClubRelationship([], false, null);
        }
        else
        {
            clubRelationships[unfollowed.ClubId] = clubRelationships[unfollowed.ClubId] with { IsFollowing = false };
        }
    }

    private void When(ClubMemberRoleAssigned assigned)
    {
        if (!_pilotClubRelationships.TryGetValue(assigned.PilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(assigned.PilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(assigned.ClubId, out var relationship))
        {
            clubRelationships[assigned.ClubId] = new ClubRelationship([assigned.Role], false, null);
        }
        else
        {
            clubRelationships[assigned.ClubId] = clubRelationships[assigned.ClubId] with { Roles = [.. clubRelationships[assigned.ClubId].Roles, assigned.Role] };
        }
    }

    private void When(ClubMemberRoleRevoked revoked)
    {
        if (!_pilotClubRelationships.TryGetValue(revoked.PilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(revoked.PilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(revoked.ClubId, out var relationship))
        {
            clubRelationships[revoked.ClubId] = new ClubRelationship([], false, null);
        }
        else
        {
            clubRelationships[revoked.ClubId] = clubRelationships[revoked.ClubId] with { Roles = [.. clubRelationships[revoked.ClubId].Roles.Where(x => x != revoked.Role)] };
        }
    }

    private void When(PilotRegisteredForClubMembershipLevel registered)
    {
        if (!_pilotClubRelationships.TryGetValue(registered.PilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(registered.PilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(registered.ClubId, out var relationship))
        {
            clubRelationships[registered.ClubId] = new ClubRelationship([], false, new ClubMembership(registered.MembershipLevelId, registered.PaymentOptionId, false, null));
        }
        else
        {
            clubRelationships[registered.ClubId] = clubRelationships[registered.ClubId] with { Membership = new ClubMembership(registered.MembershipLevelId, registered.PaymentOptionId, false, null) };
        }
    }

    private void When(PilotClubMembershipConfirmed confirmed) => ConfirmMembership(confirmed.PilotId, confirmed.ClubId, confirmed.MembershipLevelId, confirmed.PaymentOptionId, confirmed.ValidUntil);
    private void When(PilotClubMembershipManuallyConfirmed confirmed) => ConfirmMembership(confirmed.PilotId, confirmed.ClubId, confirmed.MembershipLevelId, confirmed.PaymentOptionId, confirmed.ValidUntil);

    private void ConfirmMembership(Guid pilotId, Guid clubId, int membershipLevelId, int paymentOptionId, DateTime? validUntil)
    {
        if (!_pilotClubRelationships.TryGetValue(pilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(pilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(clubId, out var relationship))
        {
            clubRelationships[clubId] = new ClubRelationship([], false, new ClubMembership(membershipLevelId, paymentOptionId, true, validUntil));
        }
        else
        {
            clubRelationships[clubId] = clubRelationships[clubId] with { Membership = new ClubMembership(membershipLevelId, paymentOptionId, true, validUntil) };
        }
    }

    private void When(PilotClubMembershipExpired remove) => RemoveMembership(remove.PilotId, remove.ClubId);
    private void When(PilotClubMembershipCancelled remove) => RemoveMembership(remove.PilotId, remove.ClubId);
    private void When(PilotClubMembershipRevoked remove) => RemoveMembership(remove.PilotId, remove.ClubId);

    private void RemoveMembership(Guid pilotId, Guid clubId)
    {
        if (!_pilotClubRelationships.TryGetValue(pilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(pilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(clubId, out var relationship))
        {
            clubRelationships[clubId] = new ClubRelationship([], false, null);
        }
        else
        {
            clubRelationships[clubId] = clubRelationships[clubId] with { Membership = null };
        }
    }
}
