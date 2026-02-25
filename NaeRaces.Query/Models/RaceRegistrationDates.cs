using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

/// <summary>
/// Tracks race-level validation policy. 
/// Registration dates are now tracked per package - see RacePackage model.
/// </summary>
public record RaceRegistrationDates(
    Guid RaceId,
    Guid? RaceValidationPolicyId,
    long? RaceValidationPolicyVersion);
