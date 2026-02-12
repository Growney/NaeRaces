using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class PilotValidationQueryHandler : IPilotValidationQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotValidationQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PilotValidationDetails> GetPilotValidationDetails(Guid pilotId)
    {
        var pilotDetails = await _dbContext.PilotDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(pd => pd.Id == pilotId);

        var ageValidations = await _dbContext.PilotAgeValidations
            .AsNoTracking()
            .Where(pv => pv.PilotId == pilotId)
            .Select(v => new PeerAgeValidation(
                v.ValidatedByPilotId,
                v.ValidatedByClubId,
                v.IsOnClubCommittee
            ))
            .ToListAsync();

        var governmentDocumentValidations = await _dbContext.PilotGovernmentDocumentValidations
            .AsNoTracking()
            .Where(pv => pv.PilotId == pilotId)
            .Select(v => new PeerGovernmentDocumentValidation(
                v.GovernmentDocument,
                v.ValidatedByPilotId,
                v.ValidatedByClubId,
                v.IsOnClubCommittee,
                v.ValidUntil
            ))
            .ToListAsync();

        var insuranceProviderValidations = await _dbContext.PilotInsuranceProviderValidations
            .AsNoTracking()
            .Where(pv => pv.PilotId == pilotId)
            .Select(v => new PeerInsuranceProviderValidation(
                v.InsuranceProvider,
                v.ValidatedByPilotId,
                v.ValidatedByClubId,
                v.IsOnClubCommittee,
                v.ValidUntil
            ))
            .ToListAsync();

        return new PilotValidationDetails(
            pilotId,
            pilotDetails?.DateOfBirth,
            ageValidations,
            governmentDocumentValidations,
            insuranceProviderValidations
        );
    }
}
