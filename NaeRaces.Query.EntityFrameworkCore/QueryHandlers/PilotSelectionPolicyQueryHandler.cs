using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class PilotSelectionPolicyQueryHandler : IPilotSelectionPolicyQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotSelectionPolicyQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<PilotSelectionPolicyDetails?> GetPolicyDetails(Guid policyId, Guid clubId)
        => _dbContext.PilotSelectionPolicyDetails
            .Where(rp => rp.Id == policyId && rp.ClubId == clubId)
            .Select(rp => new PilotSelectionPolicyDetails(rp.Id, rp.ClubId,rp.Name, rp.Description, rp.LatestVersion))
            .SingleOrDefaultAsync();
}
