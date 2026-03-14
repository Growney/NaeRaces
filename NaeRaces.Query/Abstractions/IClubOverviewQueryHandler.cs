using NaeRaces.Query.Models;

namespace NaeRaces.Query.Abstractions;

public interface IClubOverviewQueryHandler
{
    Task<ClubOverview?> GetClubOverview(Guid clubId);
    IAsyncEnumerable<ClubOverview> GetAllClubs();
    IAsyncEnumerable<ClubOverview> GetTopClubsByMemberCount(int count);
    IAsyncEnumerable<ClubOverview> GetClubsWithRacesAfter(DateTime date);
    IAsyncEnumerable<ClubOverview> SearchClubs(string searchTerm);
}
