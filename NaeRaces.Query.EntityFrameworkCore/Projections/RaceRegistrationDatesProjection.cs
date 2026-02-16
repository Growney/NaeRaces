using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class RaceRegistrationDatesProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceRegistrationDatesProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private async Task When(RacePlanned e)
    {
        RaceRegistrationDates registrationDates = new()
        {
            RaceId = e.RaceId
        };

        _dbContext.RaceRegistrationDates.Add(registrationDates);

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(TeamRacePlanned e)
    {
        RaceRegistrationDates registrationDates = new()
        {
            RaceId = e.RaceId
        };

        _dbContext.RaceRegistrationDates.Add(registrationDates);

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceValidationPolicySet e)
    {
        RaceRegistrationDates? registrationDates = await _dbContext.RaceRegistrationDates
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId);

        if (registrationDates != null)
        {
            registrationDates.RaceValidationPolicyId = e.PolicyId;
            registrationDates.RaceValidationPolicyVersion = e.PolicyVersion;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceValidationPolicyMigratedToVersion e)
    {
        RaceRegistrationDates? registrationDates = await _dbContext.RaceRegistrationDates
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId);

        if (registrationDates != null)
        {
            registrationDates.RaceValidationPolicyId = e.PolicyId;
            registrationDates.RaceValidationPolicyVersion = e.PolicyVersion;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceValidationPolicyRemoved e)
    {
        RaceRegistrationDates? registrationDates = await _dbContext.RaceRegistrationDates
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId);

        if (registrationDates != null)
        {
            registrationDates.RaceValidationPolicyId = null;
            registrationDates.RaceValidationPolicyVersion = null;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceRegistrationOpenDateScheduled e)
    {
        RaceRegistrationDates? registrationDates = await _dbContext.RaceRegistrationDates
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId);

        if (registrationDates != null)
        {
            registrationDates.RegistrationOpenDate = e.RegistrationOpenDate;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceRegistrationOpenDateRescheduled e)
    {
        RaceRegistrationDates? registrationDates = await _dbContext.RaceRegistrationDates
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId);

        if (registrationDates != null)
        {
            registrationDates.RegistrationOpenDate = e.RegistrationOpenDate;
        }

        await _dbContext.SaveChangesAsync();
    }

    private Task When(RaceEarlyRegistrationOpenDateScheduled e)
    {
        RaceEarlyRegistrationDate earlyRegistrationDate = new()
        {
            RaceId = e.RaceId,
            EarlyRegistrationId = e.EarlyRegistrationId,
            RegistrationOpenDate = e.RegistrationOpenDate,
            PilotPolicyId = e.PilotPolicyId,
            PolicyVersion = e.PolicyVersion
        };

        _dbContext.Set<RaceEarlyRegistrationDate>().Add(earlyRegistrationDate);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceEarlyRegistrationOpenDateRescheduled e)
    {
        RaceEarlyRegistrationDate? earlyRegistrationDate = await _dbContext.Set<RaceEarlyRegistrationDate>()
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.EarlyRegistrationId == e.EarlyRegistrationId);

        if (earlyRegistrationDate != null)
        {
            earlyRegistrationDate.RegistrationOpenDate = e.RegistrationOpenDate;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceEarlyRegistrationPolicyChanged e)
    {
        RaceEarlyRegistrationDate? earlyRegistrationDate = await _dbContext.Set<RaceEarlyRegistrationDate>()
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.EarlyRegistrationId == e.EarlyRegistrationId);

        if (earlyRegistrationDate != null)
        {
            earlyRegistrationDate.PilotPolicyId = e.PilotPolicyId;
            earlyRegistrationDate.PolicyVersion = e.PolicyVersion;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceEarlyRegistrationPolicyRemoved e)
    {
        RaceEarlyRegistrationDate? earlyRegistrationDate = await _dbContext.Set<RaceEarlyRegistrationDate>()
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.EarlyRegistrationId == e.EarlyRegistrationId);

        if (earlyRegistrationDate != null)
        {
            _dbContext.Set<RaceEarlyRegistrationDate>().Remove(earlyRegistrationDate);
        }

        await _dbContext.SaveChangesAsync();
    }
}
