namespace NaeRaces.Query.Models;

public record RaceInformation(Guid RaceId, string Name, DateTime FirstRaceDateStart, DateTime LastRaceDateEnd, string? ClubName, string? LocationName, int RegisteredPilotCount, int? MaximumPilots);
