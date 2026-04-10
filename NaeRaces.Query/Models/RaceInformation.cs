namespace NaeRaces.Query.Models;

public record RaceInformation(Guid RaceId, string Name, Guid ClubId, DateTime FirstRaceDateStart, DateTime LastRaceDateEnd, string? ClubName, string? LocationName, int RegisteredPilotCount, int? MinimumPilots, int? MaximumPilots, bool IsPublished, string? Description, DateTime? PaymentDeadline, DateTime? GoNoGoDate);
