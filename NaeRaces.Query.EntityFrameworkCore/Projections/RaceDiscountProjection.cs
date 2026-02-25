using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class RaceDiscountProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RaceDiscountProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    private Task When(RaceDiscountAdded e)
    {
        RaceDiscount discount = new()
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

        _dbContext.Set<RaceDiscount>().Add(discount);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(RaceDiscountRemoved e)
    {
        RaceDiscount? discount = await _dbContext.Set<RaceDiscount>()
            .SingleOrDefaultAsync(x => x.RaceId == e.RaceId && x.RaceDiscountId == e.RaceDiscountId);

        if (discount != null)
        {
            _dbContext.Set<RaceDiscount>().Remove(discount);
        }

        await _dbContext.SaveChangesAsync();
    }
}
