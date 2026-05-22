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

    public Task<bool> DoesClubCodeExist(string code, Guid ignoreClubId)
        => _dbContext.ClubUniquenessDetails.AnyAsync(c => c.Code == code && c.Id != ignoreClubId);

    public Task<bool> DoesClubNameExist(string name, Guid ignoreClubId)
        => _dbContext.ClubUniquenessDetails.AnyAsync(c => c.Name == name && c.Id != ignoreClubId);
}
