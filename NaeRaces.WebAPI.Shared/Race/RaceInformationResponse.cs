namespace NaeRaces.WebAPI.Shared.Race;

public class RaceInformationResponse
{
    public Guid RaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ClubId { get; set; }
    public string? ClubName { get; set; }
    public string? LocationName { get; set; }
    public DateTime FirstRaceDateStart { get; set; }
    public DateTime LastRaceDateEnd { get; set; }
    public int RegisteredPilotCount { get; set; }
    public int? MinimumPilots { get; set; }
    public int? MaximumPilots { get; set; }
    public bool IsPublished { get; set; }
    public string? Description { get; set; }
    public DateTime? PaymentDeadline { get; set; }
    public DateTime? GoNoGoDate { get; set; }
}
