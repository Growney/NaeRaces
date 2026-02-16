using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class ClubMembershipLevel
{
    public Guid ClubId { get; set; }
    public int MembershipLevelId { get; set; }
    public string? Name { get; set; }
    public Guid? RacePolicyId { get; set; }
    public long? PolicyVersion { get; set; }

    public ICollection<ClubMembershipLevelPaymentOption> PaymentOptions { get; set; } = new List<ClubMembershipLevelPaymentOption>();
}

public class ClubMembershipLevelPaymentOption
{
    public Guid ClubId { get; set; }
    public int MembershipLevelId { get; set; }
    public int PaymentOptionId { get; set; }
    public string? Name { get; set; }
    public ClubMembershipLevelPaymentType PaymentType { get; set; }
    public string? Currency { get; set; }
    public decimal Price { get; set; }
    public int? DayOfMonthDue { get; set; }
    public int? PaymentInterval { get; set; }

    public ClubMembershipLevel? MembershipLevel { get; set; }
}

public enum ClubMembershipLevelPaymentType
{
    Annual,
    Monthly,
    Subscription
}
