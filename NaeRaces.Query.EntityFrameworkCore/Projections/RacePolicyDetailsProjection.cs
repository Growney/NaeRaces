using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class RacePolicyDetailsProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RacePolicyDetailsProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(RacePolicyCreated e)
    {
        RacePolicyDetails policy = new()
        {
            Id = e.RacePolicyId,
            ClubId = e.ClubId,
            Name = e.Name,
            Description = e.Description
        };

        _dbContext.RacePolicyDetails.Add(policy);

        return _dbContext.SaveChangesAsync();
    }
}
