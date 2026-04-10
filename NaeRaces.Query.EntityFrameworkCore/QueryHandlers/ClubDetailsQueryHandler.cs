using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class ClubDetailsQueryHandler : IClubDetailsQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubDetailsQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    public Task<bool> DoesClubExist(Guid clubId)=> _dbContext.ClubDetails.AnyAsync(cd => cd.Id == clubId);

    public Task<bool> IsClubFounder(Guid clubId, Guid pilotId) => _dbContext.ClubDetails.AnyAsync(cd => cd.Id == clubId && cd.FounderPilotId == pilotId);

    public async Task<ClubContactDetails?> GetClubContactDetails(Guid clubId)
    {
        var details = await _dbContext.ClubDetails.SingleOrDefaultAsync(cd => cd.Id == clubId);
        return details == null ? null : new ClubContactDetails(details.Id, details.PhoneNumber, details.EmailAddress);
    }
}
