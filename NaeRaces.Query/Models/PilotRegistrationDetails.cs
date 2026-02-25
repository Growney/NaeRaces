using System;

namespace NaeRaces.Query.Models;

public record PilotRegistrationDetails(
    Guid PilotId,
    Guid RaceId,
    int RacePackageId,
    string Currency,
    decimal BaseCost,
    decimal FinalCost,
    IEnumerable<PilotRaceDiscount> ValidDiscounts);

public record PilotRaceDiscount(string Name, decimal Amount, bool IsPercentage, bool CanBeCombined, int Order, decimal RunningTotal);