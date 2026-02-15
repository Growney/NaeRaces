using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class TeamMemberQueryHandler : ITeamMemberQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public TeamMemberQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<Guid> GetTeamMemberPilotIds(Guid teamId)
    {
        return _dbContext.TeamMembers
            .Where(x => x.TeamId == teamId)
            .Select(x => x.PilotId)
            .AsAsyncEnumerable();
    }

    public IAsyncEnumerable<Guid> GetPilotTeamIds(Guid pilotId)
    {
        return _dbContext.TeamMembers
            .Where(x => x.PilotId == pilotId)
            .Select(x => x.TeamId)
            .AsAsyncEnumerable();
    }

    public Task<bool> IsPilotMemberOfTeam(Guid teamId, Guid pilotId)
    {
        return _dbContext.TeamMembers
            .AnyAsync(x => x.TeamId == teamId && x.PilotId == pilotId);
    }
}
