using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class RacePolicyQueryHandler : IRacePolicyQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public RacePolicyQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<RacePolicyDetails?> GetPolicyDetails(Guid policyId, Guid clubId)
        => _dbContext.RacePolicyDetails
            .Where(rp => rp.Id == policyId && rp.ClubId == clubId)
            .Select(rp => new RacePolicyDetails(rp.Id, rp.ClubId,rp.Name, rp.Description, rp.LatestVersion))
            .SingleOrDefaultAsync();
}
