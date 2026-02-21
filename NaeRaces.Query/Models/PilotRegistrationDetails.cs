using System;

namespace NaeRaces.Query.Models;

public record PilotRegistrationDetails(
    Guid PilotId,
    Guid RaceId,
    bool MeetsValidation,
    string? ValidationError,
    string Currency,
    decimal BaseCost,
    decimal FinalCost,
    IEnumerable<RaceDiscount> ValidDiscounts);

public record RaceDiscount(string Name, decimal Amount, bool IsPercentage, bool CanBeCombined, int Order, decimal RunningTotal);