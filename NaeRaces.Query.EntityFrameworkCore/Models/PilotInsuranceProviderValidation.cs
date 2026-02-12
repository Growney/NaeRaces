using Microsoft.EntityFrameworkCore;
using System;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

[PrimaryKey(nameof(PilotId), nameof(InsuranceProvider), nameof(ValidatedByPilotId), nameof(ValidatedByClubId))]
public class PilotInsuranceProviderValidation
{
    public Guid Id { get; set; }
    public Guid PilotId { get; set; }
    public string InsuranceProvider { get; set; } = string.Empty;
    public Guid ValidatedByPilotId { get; set; }
    public Guid ValidatedByClubId { get; set; }
    public bool IsOnClubCommittee { get; set; }
    public DateTime ValidUntil { get; set; }
    public DateTime ValidatedAt { get; set; }
}
