namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class RaceInformation
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime FirstRaceDateStart { get; set; }
    public DateTime LastRaceDateEnd { get; set; }
    public int NumberOfRaceDates { get; set; }
    public Guid ClubId { get; set; }
    public string? ClubName { get; set; }
    public int LocationId { get; set; }
    public string? LocationName { get; set; }
    public int RegisteredPilotCount { get; set; }
    public int? MaximumPilots { get; set; }
}
