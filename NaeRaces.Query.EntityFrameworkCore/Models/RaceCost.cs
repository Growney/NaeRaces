using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class RaceCost
{
    public Guid RaceId { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Cost { get; set; }

    public ICollection<RaceCostDiscount> Discounts { get; set; } = new List<RaceCostDiscount>();
}

public class RaceCostDiscount
{
    public Guid RaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public int RaceDiscountId { get; set; }
    public Guid PilotPolicyId { get; set; }
    public long PolicyVersion { get; set; }
    public decimal Discount { get; set; }
    public bool IsPercentage { get; set; }
    public bool CanBeCombined { get; set; }

    public RaceCost? RaceCost { get; set; }
}
