using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record RaceRegistrationDates(
    Guid RaceId,
    DateTime? RegistrationOpenDate,
    Guid? RaceValidationPolicyId,
    long? RaceValidationPolicyVersion,
    IEnumerable<RaceEarlyRegistrationDate> EarlyRegistrationDates);

public record RaceEarlyRegistrationDate(
    int EarlyRegistrationId,
    DateTime RegistrationOpenDate,
    Guid PilotPolicyId,
    long PolicyVersion);
