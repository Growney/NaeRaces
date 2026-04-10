using System;

namespace NaeRaces.Query.Models;

public record RaceDate(
    Guid RaceId,
    int RaceDateId,
    DateTime Start,
    DateTime End,
    bool Cancelled);
