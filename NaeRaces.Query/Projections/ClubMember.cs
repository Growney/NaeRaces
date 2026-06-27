using EventDbLite;
using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Query.Projections;

public class ClubMember : ContextProjection
{
    public record ClubMemberResult(Guid PilotId, string CallSign, string Name, string Email, string Nationality, DateTime? DateOfBirth, DateTime? MemberSince, int? MembershipLevelId, string? MembershipLevelName, int? PaymentOptionId, string? PaymentOptionName, DateTime? ValidUntil, IEnumerable<string> Roles);

    private class ClubMemberProjectionSnapshot
    {
        public record PilotInfo(Guid PilotId, string CallSign, string Name, string Email, string Nationality, DateTime? DateOfBirth);
        public record PilotMembership(Guid PilotId, Guid ClubId, DateTime MemberSince, int? MembershipLevelId, int? PaymentOptionId, DateTime? ValidUntil);
        public record PilotRole(Guid PilotId, Guid ClubId, IEnumerable<string> Roles);
        public record ClubInfo(Guid ClubId, List<MembershipLevel> MembershipLevels);
        public record MembershipLevel(int Id, string Name, List<PaymentOption> PaymentOptions);
        public record PaymentOption(int Id, string Name);

        public IEnumerable<PilotInfo> Pilots { get; set; } = Enumerable.Empty<PilotInfo>();
        public IEnumerable<PilotMembership> Memberships { get; set; } = Enumerable.Empty<PilotMembership>();
        public IEnumerable<PilotRole> Roles { get; set; } = Enumerable.Empty<PilotRole>();
        public IEnumerable<ClubInfo> Clubs { get; set; } = Enumerable.Empty<ClubInfo>();
    }

    private void Restore(ClubMemberProjectionSnapshot snapshot)
    {
        foreach (var pilot in snapshot.Pilots)
        {
            _pilots[pilot.PilotId] = new PilotDetails(pilot.CallSign, pilot.Name, pilot.Email, pilot.Nationality, pilot.DateOfBirth);
        }

        foreach (var membership in snapshot.Memberships)
        {
            if (!_memberships.TryGetValue(membership.ClubId, out var clubMemberships))
            {
                clubMemberships = new Dictionary<Guid, MembershipInfo>();
                _memberships[membership.ClubId] = clubMemberships;
            }

            clubMemberships[membership.PilotId] = new MembershipInfo(membership.MemberSince, membership.MembershipLevelId, membership.PaymentOptionId, membership.ValidUntil);
        }

        foreach (var role in snapshot.Roles)
        {
            if (!_clubRoles.TryGetValue(role.ClubId, out var clubRoles))
            {
                clubRoles = new Dictionary<Guid, HashSet<string>>();
                _clubRoles[role.ClubId] = clubRoles;
            }

            clubRoles[role.PilotId] = new HashSet<string>(role.Roles);
        }

        foreach (var club in snapshot.Clubs)
        {
            List<MembershipLevelDetails> levels = new();

            foreach (var level in club.MembershipLevels)
            {
                List<PaymentOptionDetails> options = new();

                foreach (var option in level.PaymentOptions)
                {
                    options.Add(new PaymentOptionDetails(option.Id, option.Name));
                }

                levels.Add(new MembershipLevelDetails(level.Id, level.Name, options));
            }

            _clubInfo[club.ClubId] = new ClubDetails(levels);
        }
    }

    private ClubMemberProjectionSnapshot Snapshot()
    {
        List<ClubMemberProjectionSnapshot.PilotInfo> pilots = new();

        foreach (var pilotKvp in _pilots)
        {
            pilots.Add(new ClubMemberProjectionSnapshot.PilotInfo(pilotKvp.Key, pilotKvp.Value.CallSign, pilotKvp.Value.Name, pilotKvp.Value.Email, pilotKvp.Value.Nationality, pilotKvp.Value.DateOfBirth));
        }

        List<ClubMemberProjectionSnapshot.PilotMembership> memberships = new();

        foreach (var clubKvp in _memberships)
        {
            foreach (var pilotKvp in clubKvp.Value)
            {
                memberships.Add(new ClubMemberProjectionSnapshot.PilotMembership(pilotKvp.Key, clubKvp.Key, pilotKvp.Value.MemberSince, pilotKvp.Value.MembershipLevelId, pilotKvp.Value.PaymentOptionId, pilotKvp.Value.ValidUntil));
            }
        }

        List<ClubMemberProjectionSnapshot.PilotRole> roles = new();

        foreach (var clubKvp in _clubRoles)
        {
            foreach (var pilotKvp in clubKvp.Value)
            {
                roles.Add(new ClubMemberProjectionSnapshot.PilotRole(pilotKvp.Key, clubKvp.Key, pilotKvp.Value));
            }
        }

        List<ClubMemberProjectionSnapshot.ClubInfo> clubs = new();

        foreach (var clubKvp in _clubInfo)
        {
            List<ClubMemberProjectionSnapshot.MembershipLevel> levels = new();

            foreach (var level in clubKvp.Value.MembershipLevels)
            {
                List<ClubMemberProjectionSnapshot.PaymentOption> options = new();

                foreach (var option in level.PaymentOptions)
                {
                    options.Add(new ClubMemberProjectionSnapshot.PaymentOption(option.Id, option.Name));
                }

                levels.Add(new ClubMemberProjectionSnapshot.MembershipLevel(level.Id, level.Name, options));
            }

            clubs.Add(new ClubMemberProjectionSnapshot.ClubInfo(clubKvp.Key, levels));
        }

        return new ClubMemberProjectionSnapshot()
        {
            Pilots = pilots,
            Memberships = memberships,
            Roles = roles,
            Clubs = clubs
        };
    }

    private record PilotDetails(string CallSign, string Name, string Email, string Nationality, DateTime? DateOfBirth);
    private record MembershipInfo(DateTime MemberSince, int? MembershipLevelId, int? PaymentOptionId, DateTime? ValidUntil);
    private record ClubDetails(List<MembershipLevelDetails> MembershipLevels);
    private record MembershipLevelDetails(int Id, string Name, List<PaymentOptionDetails> PaymentOptions);
    private record PaymentOptionDetails(int Id, string Name);

    private Dictionary<Guid, PilotDetails> _pilots = new();
    private Dictionary<Guid, Dictionary<Guid, MembershipInfo>> _memberships = new();
    private Dictionary<Guid, Dictionary<Guid, HashSet<string>>> _clubRoles = new();
    private Dictionary<Guid, ClubDetails> _clubInfo = new();

    public IEnumerable<ClubMemberResult> GetClubMembers(Guid clubId)
    {
        if (!_clubInfo.TryGetValue(clubId, out var clubInfo))
        {
            return Enumerable.Empty<ClubMemberResult>();
        }

        _memberships.TryGetValue(clubId, out var clubMemberships);
        _clubRoles.TryGetValue(clubId, out var clubRoles);

        HashSet<Guid> pilotIds = new();

        if (clubMemberships is not null)
        {
            foreach (var pilotId in clubMemberships.Keys)
            {
                pilotIds.Add(pilotId);
            }
        }

        if (clubRoles is not null)
        {
            foreach (var roleKvp in clubRoles)
            {
                if (roleKvp.Value.Count > 0)
                {
                    pilotIds.Add(roleKvp.Key);
                }
            }
        }

        List<ClubMemberResult> results = new();

        foreach (var pilotId in pilotIds)
        {
            if (!_pilots.TryGetValue(pilotId, out var pilot))
            {
                continue;
            }

            MembershipInfo? membership = null;
            if (clubMemberships is not null && clubMemberships.TryGetValue(pilotId, out var membershipInfo))
            {
                membership = membershipInfo;
            }

            MembershipLevelDetails? level = null;
            PaymentOptionDetails? option = null;
            IEnumerable<string> roles = [];

            if (clubRoles is not null && clubRoles.TryGetValue(pilotId, out var pilotRoles))
            {
                roles = pilotRoles;
            }

            if (membership?.MembershipLevelId is int membershipLevelId)
            {
                level = clubInfo.MembershipLevels.FirstOrDefault(x => x.Id == membershipLevelId);

                if (membership.PaymentOptionId is int paymentOptionId)
                {
                    option = level?.PaymentOptions.FirstOrDefault(x => x.Id == paymentOptionId);
                }
            }

            results.Add(new ClubMemberResult(
                pilotId,
                pilot.CallSign,
                pilot.Name,
                pilot.Email,
                pilot.Nationality,
                pilot.DateOfBirth,
                membership?.MemberSince,
                membership?.MembershipLevelId,
                level?.Name,
                membership?.PaymentOptionId,
                option?.Name,
                membership?.ValidUntil,
                roles));
        }

        return results;
    }

    private void When(PilotRegistered registered)
    {
        _pilots[registered.PilotId] = new PilotDetails(registered.CallSign, string.Empty, registered.Email, string.Empty, null);
    }

    private void When(PilotCallSignChanged changed)
    {
        if (_pilots.TryGetValue(changed.PilotId, out var pilot))
        {
            _pilots[changed.PilotId] = pilot with { CallSign = changed.NewCallSign };
        }
    }

    private void When(PilotNameSet nameSet)
    {
        if (_pilots.TryGetValue(nameSet.PilotId, out var pilot))
        {
            _pilots[nameSet.PilotId] = pilot with { Name = nameSet.Name };
        }
    }

    private void When(PilotNationalitySet nationalitySet)
    {
        if (_pilots.TryGetValue(nationalitySet.PilotId, out var pilot))
        {
            _pilots[nationalitySet.PilotId] = pilot with { Nationality = nationalitySet.Nationality };
        }
    }

    private void When(PilotDateOfBirthSet dobSet)
    {
        if (_pilots.TryGetValue(dobSet.PilotId, out var pilot))
        {
            _pilots[dobSet.PilotId] = pilot with { DateOfBirth = dobSet.DateOfBirth };
        }
    }

    private void When(ClubFormed formed)
    {
        _clubInfo[formed.ClubId] = new ClubDetails(new List<MembershipLevelDetails>());
    }

    private void When(ClubMembershipLevelAdded added)
    {
        if (_clubInfo.TryGetValue(added.ClubId, out var club))
        {
            club.MembershipLevels.Add(new MembershipLevelDetails(added.MembershipLevelId, added.Name, new List<PaymentOptionDetails>()));
        }
    }

    private void When(ClubMembershipLevelRemoved removed)
    {
        if (!_clubInfo.TryGetValue(removed.ClubId, out var club))
        {
            return;
        }

        var toRemove = club.MembershipLevels.FirstOrDefault(x => x.Id == removed.MembershipLevelId);

        if (toRemove is null)
        {
            return;
        }

        club.MembershipLevels.Remove(toRemove);
    }

    private void When(ClubMembershipLevelRenamed renamed)
    {
        if (!_clubInfo.TryGetValue(renamed.ClubId, out var club))
        {
            return;
        }

        var indexOf = club.MembershipLevels.FindIndex(x => x.Id == renamed.MembershipLevelId);

        if (indexOf < 0)
        {
            return;
        }

        club.MembershipLevels[indexOf] = club.MembershipLevels[indexOf] with { Name = renamed.NewName };
    }

    private void When(ClubMembershipLevelAnnualPaymentOptionAdded optionAdded) => AddPaymentOption(optionAdded.ClubId, optionAdded.MembershipLevelId, optionAdded.PaymentOptionId, optionAdded.Name);
    private void When(ClubMembershipLevelMonthlyPaymentOptionAdded optionAdded) => AddPaymentOption(optionAdded.ClubId, optionAdded.MembershipLevelId, optionAdded.PaymentOptionId, optionAdded.Name);
    private void When(ClubMembershipLevelSubscriptionPaymentOptionAdded optionAdded) => AddPaymentOption(optionAdded.ClubId, optionAdded.MembershipLevelId, optionAdded.PaymentOptionId, optionAdded.Name);

    private void AddPaymentOption(Guid clubId, int membershipLevelId, int paymentOptionId, string name)
    {
        if (!_clubInfo.TryGetValue(clubId, out var club))
        {
            return;
        }

        club.MembershipLevels.FirstOrDefault(x => x.Id == membershipLevelId)?.PaymentOptions.Add(new PaymentOptionDetails(paymentOptionId, name));
    }

    private void When(ClubMembershipLevelPaymentOptionRemoved removedOption)
    {
        if (!_clubInfo.TryGetValue(removedOption.ClubId, out var club))
        {
            return;
        }

        var level = club.MembershipLevels.FirstOrDefault(x => x.Id == removedOption.MembershipLevelId);

        if (level is null)
        {
            return;
        }

        var toRemove = level.PaymentOptions.FirstOrDefault(x => x.Id == removedOption.PaymentOptionId);

        if (toRemove is null)
        {
            return;
        }

        level.PaymentOptions.Remove(toRemove);
    }

    private void When(ClubMembershipLevelPaymentOptionRenamed renamedOption)
    {
        if (!_clubInfo.TryGetValue(renamedOption.ClubId, out var club))
        {
            return;
        }

        var level = club.MembershipLevels.FirstOrDefault(x => x.Id == renamedOption.MembershipLevelId);

        if (level is null)
        {
            return;
        }

        var optionIndex = level.PaymentOptions.FindIndex(x => x.Id == renamedOption.PaymentOptionId);

        if (optionIndex < 0)
        {
            return;
        }

        level.PaymentOptions[optionIndex] = level.PaymentOptions[optionIndex] with { Name = renamedOption.NewName };
    }

    private void When(ClubMemberRoleAssigned assigned)
    {
        if (!_clubRoles.TryGetValue(assigned.ClubId, out var clubRoles))
        {
            clubRoles = new Dictionary<Guid, HashSet<string>>();
            _clubRoles[assigned.ClubId] = clubRoles;
        }

        if (!clubRoles.TryGetValue(assigned.PilotId, out var roles))
        {
            roles = new HashSet<string>();
            clubRoles[assigned.PilotId] = roles;
        }

        roles.Add(assigned.Role);
    }

    private void When(ClubMemberRoleRevoked revoked)
    {
        if (!_clubRoles.TryGetValue(revoked.ClubId, out var clubRoles))
        {
            return;
        }

        if (!clubRoles.TryGetValue(revoked.PilotId, out var roles))
        {
            return;
        }

        roles.Remove(revoked.Role);

        if (roles.Count == 0)
        {
            clubRoles.Remove(revoked.PilotId);
        }
    }

    private void When(PilotClubMembershipConfirmed confirmed) => ConfirmMembership(confirmed.PilotId, confirmed.ClubId, confirmed.MembershipLevelId, confirmed.PaymentOptionId, confirmed.ValidUntil);
    private void When(PilotClubMembershipManuallyConfirmed confirmed) => ConfirmMembership(confirmed.PilotId, confirmed.ClubId, confirmed.MembershipLevelId, confirmed.PaymentOptionId, confirmed.ValidUntil);

    private void ConfirmMembership(Guid pilotId, Guid clubId, int membershipLevelId, int paymentOptionId, DateTime? validUntil)
    {
        if (!_memberships.TryGetValue(clubId, out var clubMemberships))
        {
            clubMemberships = new Dictionary<Guid, MembershipInfo>();
            _memberships[clubId] = clubMemberships;
        }

        if (!clubMemberships.ContainsKey(pilotId))
        {
            DateTime membershipStart = Metadata?.InceptionUtc ?? DateTime.UtcNow;
            clubMemberships.Add(pilotId, new MembershipInfo(membershipStart, membershipLevelId, paymentOptionId, validUntil));
        }
        else
        {
            clubMemberships[pilotId] = clubMemberships[pilotId] with { MembershipLevelId = membershipLevelId, PaymentOptionId = paymentOptionId, ValidUntil = validUntil };
        }
    }

    private void When(PilotClubMembershipRenewed renewed)
    {
        if (_memberships.TryGetValue(renewed.ClubId, out var clubMemberships) && clubMemberships.TryGetValue(renewed.PilotId, out var membership))
        {
            clubMemberships[renewed.PilotId] = membership with { ValidUntil = renewed.NewValidUntil };
        }
    }

    private void When(PilotClubMembershipExpired expired) => RemoveMembership(expired.PilotId, expired.ClubId);
    private void When(PilotClubMembershipCancelled cancelled) => RemoveMembership(cancelled.PilotId, cancelled.ClubId);
    private void When(PilotClubMembershipRevoked revoked) => RemoveMembership(revoked.PilotId, revoked.ClubId);

    private void RemoveMembership(Guid pilotId, Guid clubId)
    {
        if (_memberships.TryGetValue(clubId, out var clubMemberships))
        {
            clubMemberships.Remove(pilotId);
        }
    }
}
