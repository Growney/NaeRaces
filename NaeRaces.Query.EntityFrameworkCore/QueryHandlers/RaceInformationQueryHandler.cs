using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class RaceInformationQueryHandler : IRaceInformationQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceInformationQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Query.Models.RaceInformation?> GetRaceInformation(Guid raceId)
    {
        RaceInformation? info = await _dbContext.RaceInformation.FirstOrDefaultAsync(x => x.Id == raceId);
        if (info == null)
        {
            return null;
        }

        return ToQueryModel(info);
    }

    public IAsyncEnumerable<Query.Models.RaceInformation> GetRaceInformationAfterDate(DateTime afterDate) =>
        _dbContext.RaceInformation
            .Where(x => x.LastRaceDateEnd > afterDate)
            .Select(info => ToQueryModel(info))
            .AsAsyncEnumerable();

    public IAsyncEnumerable<Query.Models.RaceInformation> GetRaceInformationForClub(Guid clubId) =>
        _dbContext.RaceInformation
            .Where(x => x.ClubId == clubId)
            .Select(info => ToQueryModel(info))
            .AsAsyncEnumerable();

    public IAsyncEnumerable<Query.Models.RaceInformation> GetRaceInformationForClubs(IEnumerable<Guid> clubIds) =>
        _dbContext.RaceInformation
            .Where(x => clubIds.Contains(x.ClubId))
            .Select(info => ToQueryModel(info))
            .AsAsyncEnumerable();

    public IAsyncEnumerable<Query.Models.RaceInformation> GetRaceInformationForClubsAfterDate(IEnumerable<Guid> clubIds, DateTime afterDate) =>
        _dbContext.RaceInformation
            .Where(x => clubIds.Contains(x.ClubId) && x.LastRaceDateEnd > afterDate)
            .Select(info => ToQueryModel(info))
            .AsAsyncEnumerable();

    public IAsyncEnumerable<Query.Models.RaceInformation> GetRaceInformationForPilot(Guid pilotId) =>
        _dbContext.PilotRaceRegistrations
            .Where(x => x.PilotId == pilotId)
            .Join(_dbContext.RaceInformation,
                reg => reg.RaceId,
                info => info.Id,
                (reg, info) => info)
            .Select(info => ToQueryModel(info))
            .AsAsyncEnumerable();

    public IAsyncEnumerable<Query.Models.RaceInformation> GetRaceInformationForPilotAfterDate(Guid pilotId, DateTime afterDate) =>
        _dbContext.PilotRaceRegistrations
            .Where(x => x.PilotId == pilotId)
            .Join(_dbContext.RaceInformation,
                reg => reg.RaceId,
                info => info.Id,
                (reg, info) => info)
            .Where(info => info.LastRaceDateEnd > afterDate)
            .Select(info => ToQueryModel(info))
            .AsAsyncEnumerable();

    private static Query.Models.RaceInformation ToQueryModel(RaceInformation info) =>
        new(info.Id, info.Name ?? string.Empty, info.FirstRaceDateStart, info.LastRaceDateEnd, info.ClubName, info.LocationName, info.RegisteredPilotCount, info.MaximumPilots);
}
