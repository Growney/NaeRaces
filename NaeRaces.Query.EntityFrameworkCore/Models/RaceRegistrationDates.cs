using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class RaceRegistrationDates
{
    public Guid RaceId { get; set; }
    public DateTime? RegistrationOpenDate { get; set; }
    public Guid? RaceValidationPolicyId { get; set; }
    public long? RaceValidationPolicyVersion { get; set; }

    public ICollection<RaceEarlyRegistrationDate> EarlyRegistrationDates { get; set; } = new List<RaceEarlyRegistrationDate>();
}

public class RaceEarlyRegistrationDate
{
    public Guid RaceId { get; set; }
    public int EarlyRegistrationId { get; set; }
    public DateTime RegistrationOpenDate { get; set; }
    public Guid PilotPolicyId { get; set; }
    public long PolicyVersion { get; set; }

    public RaceRegistrationDates? RaceRegistrationDates { get; set; }
}
