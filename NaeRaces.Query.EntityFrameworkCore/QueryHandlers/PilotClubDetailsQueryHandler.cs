using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class PilotClubDetailsQueryHandler : IPilotClubDetailsQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotClubDetailsQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<PilotClubDetails> GetPilotClubs(Guid pilotId)
    {
        return _dbContext.PilotClubDetails
            .Where(x => x.PilotId == pilotId)
            .Select(x => new PilotClubDetails(
                x.ClubId,
                x.ClubName ?? string.Empty,
                x.ClubCode ?? string.Empty,
                x.HomeLocationName != null
                    ? new HomeLocation(x.HomeLocationName, x.HomeLocationAddressLine1, x.HomeLocationAddressLine2, x.HomeLocationCity, x.HomeLocationPostcode, x.HomeLocationCounty)
                    : null,
                x.MembershipLevelName,
                x.MembershipValidUntil,
                _dbContext.PilotClubDetails.Count(m => m.ClubId == x.ClubId)))
            .AsAsyncEnumerable();
    }
}
