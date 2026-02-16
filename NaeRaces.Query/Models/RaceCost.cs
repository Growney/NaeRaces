using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record RaceCost(Guid RaceId, string Currency, decimal Cost, IEnumerable<RaceCostDiscount> Discounts);

public record RaceCostDiscount(int RaceDiscountId, Guid PilotPolicyId, long PolicyVersion, decimal Discount);
