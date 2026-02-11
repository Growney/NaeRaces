using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class ClubDetailsQueryHandler : IClubDetailsQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubDetailsQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    public Task<bool> DoesClubExist(Guid clubId)=> _dbContext.ClubDetails.AnyAsync(cd => cd.Id == clubId);
}
