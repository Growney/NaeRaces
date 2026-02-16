using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class PilotSelectionPolicyDetailsProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotSelectionPolicyDetailsProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(PilotSelectionPolicyCreated e)
    {
        PilotSelectionPolicyDetails policy = new()
        {
            Id = e.PilotSelectionPolicyId,
            ClubId = e.ClubId,
            Name = e.Name,
            Description = e.Description
        };

        _dbContext.PilotSelectionPolicyDetails.Add(policy);

        return _dbContext.SaveChangesAsync();
    }
}
