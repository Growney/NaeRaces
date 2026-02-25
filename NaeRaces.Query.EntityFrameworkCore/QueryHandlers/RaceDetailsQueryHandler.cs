using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.EntityFrameworkCore.Models;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class RaceDetailsQueryHandler : IRaceDetailsQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceDetailsQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<bool> DoesRaceExist(Guid raceId)
    {
        return await _dbContext.RaceDetails.AnyAsync(x => x.Id == raceId);
    }

    public async Task<Query.Models.RaceRegistrationDetails?> GetRaceRegistrationDetails(Guid raceId)
    {
        Models.RaceDetails? raceDetails = await _dbContext.RaceDetails.FirstOrDefaultAsync(x => x.Id == raceId);
        if (raceDetails == null)
        {
            return null;
        }

        return new Query.Models.RaceRegistrationDetails(raceDetails.FirstRaceDateStart, raceDetails.LastRaceDateEnd, raceDetails.IsCancelled, raceDetails.PilotPolicyId, raceDetails.PilotPolicyVersion);
    }
}
