using NaeRaces.Query.Models;

namespace NaeRaces.Query.Abstractions;

public interface IClubOverviewQueryHandler
{
    IAsyncEnumerable<ClubOverview> GetAllClubs();
    IAsyncEnumerable<ClubOverview> GetTopClubsByMemberCount(int count);
    IAsyncEnumerable<ClubOverview> GetClubsWithRacesAfter(DateTime date);
    IAsyncEnumerable<ClubOverview> SearchClubs(string searchTerm);
}
