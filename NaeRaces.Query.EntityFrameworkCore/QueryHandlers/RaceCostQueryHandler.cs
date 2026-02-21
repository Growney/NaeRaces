using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class RaceCostQueryHandler : IRaceCostQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceCostQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<RaceCost?> GetRaceCost(Guid raceId, string currency)
    {
        var dbRaceCost = await _dbContext.RaceCosts
            .Include(x => x.Discounts)
            .SingleOrDefaultAsync(x => x.RaceId == raceId && x.Currency == currency);

        if (dbRaceCost == null)
            return null;

        var discounts = dbRaceCost.Discounts.Select(d => new RaceCostDiscount(
            d.Name,
            d.RaceDiscountId,
            d.PilotPolicyId,
            d.PolicyVersion,
            d.Discount,
            d.IsPercentage,
            d.CanBeCombined
        )).ToList();

        return new RaceCost(
            dbRaceCost.RaceId,
            dbRaceCost.Currency,
            dbRaceCost.Cost,
            discounts
        );
    }

    public IAsyncEnumerable<RaceCost> GetRaceCosts(Guid raceId) =>
        _dbContext.RaceCosts
            .Include(x => x.Discounts)
            .Where(x => x.RaceId == raceId)
            .Select(dbRaceCost => new RaceCost(
                dbRaceCost.RaceId,
                dbRaceCost.Currency,
                dbRaceCost.Cost,
                dbRaceCost.Discounts.Select(d => new RaceCostDiscount(
                    d.Name,
                    d.RaceDiscountId,
                    d.PilotPolicyId,
                    d.PolicyVersion,
                    d.Discount,
                    d.IsPercentage,
                    d.CanBeCombined
                ))
            ))
            .ToAsyncEnumerable();

    public async Task<RaceCostDiscount?> GetRaceDiscount(Guid raceId, string currency, Guid pilotPolicyId)
    {
        var dbDiscount = await _dbContext.Set<Models.RaceCostDiscount>()
            .SingleOrDefaultAsync(x => x.RaceId == raceId && x.Currency == currency && x.PilotPolicyId == pilotPolicyId);

        if (dbDiscount == null)
            return null;

        return new RaceCostDiscount(
            dbDiscount.Name,
            dbDiscount.RaceDiscountId,
            dbDiscount.PilotPolicyId,
            dbDiscount.PolicyVersion,
            dbDiscount.Discount,
            dbDiscount.IsPercentage,
            dbDiscount.CanBeCombined
        );
    }

    public IAsyncEnumerable<RaceCostDiscount> GetRaceDiscounts(Guid raceId, string currency) =>
        _dbContext.Set<Models.RaceCostDiscount>()
            .Where(x => x.RaceId == raceId && x.Currency == currency)
            .Select(dbDiscount => new RaceCostDiscount(
                dbDiscount.Name,
                dbDiscount.RaceDiscountId,
                dbDiscount.PilotPolicyId,
                dbDiscount.PolicyVersion,
                dbDiscount.Discount,
                dbDiscount.IsPercentage,
                dbDiscount.CanBeCombined
            ))
            .ToAsyncEnumerable();
}
