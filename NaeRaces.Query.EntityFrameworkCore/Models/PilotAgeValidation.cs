using Microsoft.EntityFrameworkCore;
using System;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

[PrimaryKey(nameof(PilotId), nameof(ValidatedByPilotId), nameof(ValidatedByClubId))]
public class PilotAgeValidation
{
    public Guid PilotId { get; set; }
    public Guid ValidatedByPilotId { get; set; }
    public Guid ValidatedByClubId { get; set; }
    public bool IsOnClubCommittee { get; set; }
    public DateTime ValidatedAt { get; set; }
}
