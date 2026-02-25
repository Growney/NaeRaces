using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class RacePackageProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RacePackageProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(RacePackageAdded e)
    {
        RacePackage package = new()
        {
            RaceId = e.RaceId,
            RacePackageId = e.RacePackageId,
            Name = e.Name,
            Currency = e.Currency,
            Cost = e.Cost,
            ApplyDiscounts = true,
            IsRegistrationManuallyOpened = false,
            
        };

        _dbContext.RacePackages.Add(package);

        return _dbContext.SaveChangesAsync();
    }
    private async Task When(RacePackageRegistrationManuallyClosed e)
    {
        RacePackage? package = await _dbContext.RacePackages
           .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RacePackageId == e.RacePackageId);

        if (package != null)
        {
            package.IsRegistrationManuallyOpened = false;
            await _dbContext.SaveChangesAsync();
        }
    }
    private async Task When(RacePackageRegistrationManuallyOpened e)
    {
        RacePackage? package = await _dbContext.RacePackages
           .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RacePackageId == e.RacePackageId);
        if (package != null)
        {
            package.IsRegistrationManuallyOpened = true;
            await _dbContext.SaveChangesAsync();
        }
    }
    private async Task When(RacePackageDiscountStatusSet e)
    {
        RacePackage? package = await _dbContext.RacePackages
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RacePackageId == e.RacePackageId);

        if (package != null)
        {
            package.ApplyDiscounts = e.ApplyDiscounts;
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task When(RacePackageRegistrationOpenScheduled e)
    {
        RacePackage? package = await _dbContext.RacePackages
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RacePackageId == e.RacePackageId);

        if (package != null)
        {
            package.RegistrationOpenDate = e.RegistrationOpenDate;
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task When(RacePackageRegistrationCloseScheduled e)
    {
        RacePackage? package = await _dbContext.RacePackages
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RacePackageId == e.RacePackageId);

        if (package != null)
        {
            package.RegistrationCloseDate = e.RegistrationCloseDate;
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task When(RacePackagePilotPolicySet e)
    {
        RacePackage? package = await _dbContext.RacePackages
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RacePackageId == e.RacePackageId);

        if (package != null)
        {
            package.PilotPolicyId = e.PilotPolicyId;
            package.PolicyVersion = e.PolicyVersion;
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task When(RacePackageRemoved e)
    {
        RacePackage? package = await _dbContext.RacePackages
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RacePackageId == e.RacePackageId);

        if (package != null)
        {
            _dbContext.RacePackages.Remove(package);
            await _dbContext.SaveChangesAsync();
        }
    }
}
