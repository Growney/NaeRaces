using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record RaceDiscount(string Name, int RaceDiscountId, Guid PilotPolicyId, long PolicyVersion, decimal Discount, bool IsPercentage, bool CanBeCombined);
