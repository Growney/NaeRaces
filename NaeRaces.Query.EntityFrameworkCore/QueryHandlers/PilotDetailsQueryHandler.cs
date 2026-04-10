using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class PilotDetailsQueryHandler : IPilotDetailsQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotDetailsQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    public Task<bool> DoesPilotExist(Guid pilotId) => _dbContext.PilotDetails.AnyAsync(x => x.Id == pilotId);

    public async Task<string?> GetPilotDisplayName(Guid pilotId)
    {
        var pilot = await _dbContext.PilotDetails
            .Where(x => x.Id == pilotId)
            .Select(x => new { x.Callsign, x.Name })
            .FirstOrDefaultAsync();

        return pilot?.Callsign ?? pilot?.Name;
    }
}
