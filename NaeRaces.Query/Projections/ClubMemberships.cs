using NaeRaces.Events;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Query.Projections;

public class ClubMemberships
{
    private record PaymentOptionDetails(int Id, string Name, ClubMembershipLevelPaymentType PaymentType, string Currency, decimal Price, int? DayOfMonthDue, int? PaymentInterval);
    private record MembershipLevelDetails(int Id, string Name, Guid? PilotPolicyId, long? PolicyVersion, List<PaymentOptionDetails> PaymentOptions);
    private record ClubDetails(List<MembershipLevelDetails> MembershipLevels);

    private class ClubMembershipsSnapshot
    {
        public record PaymentOption(int Id, string Name, ClubMembershipLevelPaymentType PaymentType, string Currency, decimal Price, int? DayOfMonthDue, int? PaymentInterval);
        public record MembershipLevel(int Id, string Name, Guid? PilotPolicyId, long? PolicyVersion, List<PaymentOption> PaymentOptions);
        public record Club(Guid ClubId, List<MembershipLevel> MembershipLevels);

        public IEnumerable<Club> Clubs { get; set; } = Enumerable.Empty<Club>();
    }

    private Dictionary<Guid, ClubDetails> _clubs = new();

    private ClubMembershipsSnapshot Snapshot()
    {
        List<ClubMembershipsSnapshot.Club> clubs = new();

        foreach (var clubKvp in _clubs)
        {
            List<ClubMembershipsSnapshot.MembershipLevel> levels = new();

            foreach (var level in clubKvp.Value.MembershipLevels)
            {
                List<ClubMembershipsSnapshot.PaymentOption> options = new();

                foreach (var option in level.PaymentOptions)
                {
                    options.Add(new ClubMembershipsSnapshot.PaymentOption(option.Id, option.Name, option.PaymentType, option.Currency, option.Price, option.DayOfMonthDue, option.PaymentInterval));
                }

                levels.Add(new ClubMembershipsSnapshot.MembershipLevel(level.Id, level.Name, level.PilotPolicyId, level.PolicyVersion, options));
            }

            clubs.Add(new ClubMembershipsSnapshot.Club(clubKvp.Key, levels));
        }

        return new ClubMembershipsSnapshot() { Clubs = clubs };
    }

    private void Restore(ClubMembershipsSnapshot snapshot)
    {
        foreach (var club in snapshot.Clubs)
        {
            List<MembershipLevelDetails> levels = new();

            foreach (var level in club.MembershipLevels)
            {
                List<PaymentOptionDetails> options = new();

                foreach (var option in level.PaymentOptions)
                {
                    options.Add(new PaymentOptionDetails(option.Id, option.Name, option.PaymentType, option.Currency, option.Price, option.DayOfMonthDue, option.PaymentInterval));
                }

                levels.Add(new MembershipLevelDetails(level.Id, level.Name, level.PilotPolicyId, level.PolicyVersion, options));
            }

            _clubs[club.ClubId] = new ClubDetails(levels);
        }
    }

    private void When(ClubFormed formed)
    {
        _clubs[formed.ClubId] = new ClubDetails(new List<MembershipLevelDetails>());
    }

    private void When(ClubMembershipLevelAdded added)
    {
        if (_clubs.TryGetValue(added.ClubId, out var club))
        {
            club.MembershipLevels.Add(new MembershipLevelDetails(added.MembershipLevelId, added.Name, null, null, new List<PaymentOptionDetails>()));
        }
    }

    private void When(ClubMembershipLevelRemoved removed)
    {
        if (!_clubs.TryGetValue(removed.ClubId, out var club))
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
        if (!_clubs.TryGetValue(renamed.ClubId, out var club))
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

    private void When(ClubMembershipLevelPolicySet policySet)
    {
        if (!_clubs.TryGetValue(policySet.ClubId, out var club))
        {
            return;
        }

        var indexOf = club.MembershipLevels.FindIndex(x => x.Id == policySet.MembershipLevelId);

        if (indexOf < 0)
        {
            return;
        }

        club.MembershipLevels[indexOf] = club.MembershipLevels[indexOf] with { PilotPolicyId = policySet.PilotPolicyId, PolicyVersion = policySet.PolicyVersion };
    }

    private void When(ClubMembershipLevelPolicyCleared policyCleared)
    {
        if (!_clubs.TryGetValue(policyCleared.ClubId, out var club))
        {
            return;
        }

        var indexOf = club.MembershipLevels.FindIndex(x => x.Id == policyCleared.MembershipLevelId);

        if (indexOf < 0)
        {
            return;
        }

        club.MembershipLevels[indexOf] = club.MembershipLevels[indexOf] with { PilotPolicyId = null, PolicyVersion = null };
    }

    private void When(ClubMembershipLevelAnnualPaymentOptionAdded optionAdded)
    {
        AddPaymentOption(optionAdded.ClubId, optionAdded.MembershipLevelId,
            new PaymentOptionDetails(optionAdded.PaymentOptionId, optionAdded.Name, ClubMembershipLevelPaymentType.Annual, optionAdded.Currency, optionAdded.Price, null, null));
    }

    private void When(ClubMembershipLevelMonthlyPaymentOptionAdded optionAdded)
    {
        AddPaymentOption(optionAdded.ClubId, optionAdded.MembershipLevelId,
            new PaymentOptionDetails(optionAdded.PaymentOptionId, optionAdded.Name, ClubMembershipLevelPaymentType.Monthly, optionAdded.Currency, optionAdded.Price, optionAdded.DayOfMonthDue, optionAdded.PaymentInterval));
    }

    private void When(ClubMembershipLevelSubscriptionPaymentOptionAdded optionAdded)
    {
        AddPaymentOption(optionAdded.ClubId, optionAdded.MembershipLevelId,
            new PaymentOptionDetails(optionAdded.PaymentOptionId, optionAdded.Name, ClubMembershipLevelPaymentType.Subscription, optionAdded.Currency, optionAdded.Price, null, optionAdded.PaymentInterval));
    }

    private void AddPaymentOption(Guid clubId, int membershipLevelId, PaymentOptionDetails option)
    {
        if (!_clubs.TryGetValue(clubId, out var club))
        {
            return;
        }

        club.MembershipLevels.FirstOrDefault(x => x.Id == membershipLevelId)?.PaymentOptions.Add(option);
    }

    private void When(ClubMembershipLevelPaymentOptionRemoved removedOption)
    {
        if (!_clubs.TryGetValue(removedOption.ClubId, out var club))
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
        if (!_clubs.TryGetValue(renamedOption.ClubId, out var club))
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

    public ClubMembershipLevel? GetClubMembershipLevel(Guid clubId, int membershipLevelId)
    {
        if (!_clubs.TryGetValue(clubId, out var club))
        {
            return null;
        }

        var level = club.MembershipLevels.FirstOrDefault(x => x.Id == membershipLevelId);

        return level is null ? null : ToModel(clubId, level);
    }

    public IEnumerable<ClubMembershipLevel> GetClubMembershipLevels(Guid clubId)
    {
        if (!_clubs.TryGetValue(clubId, out var club))
        {
            return Enumerable.Empty<ClubMembershipLevel>();
        }

        return club.MembershipLevels.Select(level => ToModel(clubId, level));
    }

    private static ClubMembershipLevel ToModel(Guid clubId, MembershipLevelDetails level)
    {
        var paymentOptions = level.PaymentOptions.Select(o =>
            new ClubMembershipLevelPaymentOption(o.Id, o.Name, o.PaymentType, o.Currency, o.Price, o.DayOfMonthDue, o.PaymentInterval));

        return new ClubMembershipLevel(clubId, level.Id, level.Name, level.PilotPolicyId, level.PolicyVersion, paymentOptions);
    }
}
