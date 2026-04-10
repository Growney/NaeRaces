using System;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class RaceDate
{
    public Guid RaceId { get; set; }
    public int RaceDateId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool Cancelled { get; set; }
}
