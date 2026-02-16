using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record ClubMembershipLevel(Guid ClubId, int MembershipLevelId, string Name, Guid? PilotPolicyId, long? PolicyVersion, IEnumerable<ClubMembershipLevelPaymentOption> PaymentOptions);

public record ClubMembershipLevelPaymentOption(int PaymentOptionId, string Name, ClubMembershipLevelPaymentType PaymentType, string Currency, decimal Price, int? DayOfMonthDue, int? PaymentInterval);

public enum ClubMembershipLevelPaymentType
{
    Annual,
    Monthly,
    Subscription
}
