using System;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class RacePackage
{
    public Guid RaceId { get; set; }
    public int RacePackageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public bool ApplyDiscounts { get; set; }
    public DateTime? RegistrationOpenDate { get; set; }
    public DateTime? RegistrationCloseDate { get; set; }
    public Guid? PilotPolicyId { get; set; }
    public long? PolicyVersion { get; set; }
    public bool IsRegistrationManuallyOpened { get; set; }
}
