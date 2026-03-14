using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using ModelPilotFollowedClub = NaeRaces.Query.EntityFrameworkCore.Models.PilotFollowedClub;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class PilotFollowedClubProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotFollowedClubProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(PilotFollowedClub e)
    {
        ModelPilotFollowedClub followedClub = new()
        {
            PilotId = e.PilotId,
            ClubId = e.ClubId
        };

        _dbContext.PilotFollowedClubs.Add(followedClub);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotUnfollowedClub e)
    {
        ModelPilotFollowedClub? followedClub = await _dbContext.PilotFollowedClubs
            .SingleOrDefaultAsync(x => x.PilotId == e.PilotId && x.ClubId == e.ClubId);

        if (followedClub != null)
        {
            _dbContext.PilotFollowedClubs.Remove(followedClub);
            await _dbContext.SaveChangesAsync();
        }
    }
}
