namespace NaeRaces.Query.Models;

public record PilotClubDetails(Guid ClubId, string Name, string Code, HomeLocation? HomeLocation, string? MembershipLevelName, DateTime? MembershipValidUntil, int TotalMemberCount);
