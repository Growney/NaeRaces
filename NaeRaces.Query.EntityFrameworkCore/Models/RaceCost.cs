using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class RaceDiscount
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
}
