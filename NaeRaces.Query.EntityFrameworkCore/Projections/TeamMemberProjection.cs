using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class TeamMemberProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public TeamMemberProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(PilotJoinedTeam e)
    {
        TeamMember member = new()
        {
            TeamId = e.TeamId,
            PilotId = e.PilotId
        };

        _dbContext.TeamMembers.Add(member);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotLeftTeam e)
    {
        TeamMember? member = await _dbContext.TeamMembers
            .SingleOrDefaultAsync(x => x.TeamId == e.TeamId && x.PilotId == e.PilotId);
        
        if (member != null)
        {
            _dbContext.TeamMembers.Remove(member);
            await _dbContext.SaveChangesAsync();
        }
    }
}
