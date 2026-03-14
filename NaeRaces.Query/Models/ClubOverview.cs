namespace NaeRaces.Query.Models;

public record ClubOverview(Guid ClubId, string Name, string Code, HomeLocation? HomeLocation, int TotalMemberCount);
