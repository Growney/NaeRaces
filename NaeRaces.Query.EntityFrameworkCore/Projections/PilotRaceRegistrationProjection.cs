using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class PilotRaceRegistrationProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotRaceRegistrationProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(IndividualPilotRegisteredForRace e)
    {
        _dbContext.PilotRaceRegistrations.Add(new PilotRaceRegistration
        {
            PilotId = e.PilotId,
            RaceId = e.RaceId,
            RegistrationId = e.RegistrationId
        });

        return _dbContext.SaveChangesAsync();
    }

    private Task When(TeamRosterPilotRegisteredForRace e)
    {
        _dbContext.PilotRaceRegistrations.Add(new PilotRaceRegistration
        {
            PilotId = e.PilotId,
            RaceId = e.RaceId,
            RegistrationId = e.RegistrationId
        });

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceRegistrationCancelled e)
    {
        PilotRaceRegistration? registration = await _dbContext.PilotRaceRegistrations
            .SingleOrDefaultAsync(x => x.RegistrationId == e.RegistrationId);

        if (registration != null)
        {
            _dbContext.PilotRaceRegistrations.Remove(registration);
            await _dbContext.SaveChangesAsync();
        }
    }
}
