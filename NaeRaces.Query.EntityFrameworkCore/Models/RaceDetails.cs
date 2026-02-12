using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class RaceDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsTeamRace { get; set; }
    public DateTime FirstRaceDateStart { get; set; }
    public DateTime LastRaceDateEnd { get; set; }
    public int NumberOfRaceDates { get; set; }
    public bool IsDetailsPublished { get; set; }
    public bool IsPublished { get; set; }
    public bool IsCancelled { get; set; }
    public Guid ClubId { get; set; }
    public int LocationId { get; set; }
}
