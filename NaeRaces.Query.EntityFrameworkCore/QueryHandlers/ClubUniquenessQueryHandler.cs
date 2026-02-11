using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class ClubUniquenessQueryHandler : IClubUniquenessQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubUniquenessQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<bool> DoesClubCodeExist(string code)
        => _dbContext.ClubUniquenessDetails.AnyAsync(c => c.Code == code);

    public Task<bool> DoesClubNameExist(string name)
        => _dbContext.ClubUniquenessDetails.AnyAsync(c => c.Name == name);
}
