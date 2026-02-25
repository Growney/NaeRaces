using System;

namespace NaeRaces.Query.Models;

public record RacePackage(
    Guid RaceId,
    int RacePackageId,
    string Name,
    string Currency,
    decimal Cost,
    bool ApplyDiscounts,
    DateTime? RegistrationOpenDate,
    DateTime? RegistrationCloseDate,
    bool IsRegistrationManuallyOpened,
    Guid? PilotPolicyId,
    long? PolicyVersion);
