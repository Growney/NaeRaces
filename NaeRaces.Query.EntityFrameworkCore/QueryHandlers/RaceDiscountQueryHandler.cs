using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class RaceDiscountQueryHandler : IRaceDiscountQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceDiscountQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<RaceDiscount> GetRaceDiscounts(Guid raceId, string currency) =>
        _dbContext.Set<Models.RaceDiscount>()
            .Where(x => x.RaceId == raceId && x.Currency == currency)
            .Select(dbDiscount => new RaceDiscount(
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
