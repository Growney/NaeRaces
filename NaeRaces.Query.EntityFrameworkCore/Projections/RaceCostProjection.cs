using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class RaceCostProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceCostProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private async Task When(RaceCostSet e)
    {
        RaceCost? raceCost = await _dbContext.RaceCosts
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.Currency == e.Currency);

        if (raceCost == null)
        {
            raceCost = new RaceCost
            {
                RaceId = e.RaceId,
                Currency = e.Currency,
                Cost = e.Cost
            };
            _dbContext.RaceCosts.Add(raceCost);
        }
        else
        {
            raceCost.Cost = e.Cost;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceCostIncreased e)
    {
        RaceCost? raceCost = await _dbContext.RaceCosts
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.Currency == e.Currency);

        if (raceCost != null)
        {
            raceCost.Cost = e.Cost;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceCostReduced e)
    {
        RaceCost? raceCost = await _dbContext.RaceCosts
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.Currency == e.Currency);

        if (raceCost != null)
        {
            raceCost.Cost = e.Cost;
        }

        await _dbContext.SaveChangesAsync();
    }

    private Task When(RaceDiscountAdded e)
    {
        RaceCostDiscount discount = new()
        {
            RaceId = e.RaceId,
            Name = e.Name,
            Currency = e.Currency,
            RaceDiscountId = e.RaceDiscountId,
            PilotPolicyId = e.PilotPolicyId,
            PolicyVersion = e.PolicyVersion,
            IsPercentage = e.IsPercentage,
            Discount = e.Discount,
            CanBeCombined = e.CanBeCombined
        };

        _dbContext.Set<RaceCostDiscount>().Add(discount);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDiscountRemoved e)
    {
        RaceCostDiscount? discount = await _dbContext.Set<RaceCostDiscount>()
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RaceDiscountId == e.RaceDiscountId);

        if (discount != null)
        {
            _dbContext.Set<RaceCostDiscount>().Remove(discount);
        }

        await _dbContext.SaveChangesAsync();
    }
}
