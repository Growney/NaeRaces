using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class PilotValidationProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotValidationProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private async Task When(PilotGovernmentDocumentationValidatedByPeer e)
    {
        PilotGovernmentDocumentValidation validation = new()
        {
            PilotId = e.PilotId,
            GovernmentDocument = e.GovernmentDocument,
            ValidatedByPilotId = e.ValidatedByPilotId,
            ValidatedByClubId = e.ClubId,
            IsOnClubCommittee = e.IsValidatingMemberOnCommiteeOfClub,
            ValidUntil = e.ValidUntil,
            ValidatedAt = DateTime.UtcNow
        };

        _dbContext.PilotGovernmentDocumentValidations.Add(validation);

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotInsuranceValidatedByPeer e)
    {
        PilotInsuranceProviderValidation validation = new()
        {
            PilotId = e.PilotId,
            InsuranceProvider = e.InsuranceProvider,
            ValidatedByPilotId = e.ValidatedByPilotId,
            ValidatedByClubId = e.ClubId,
            IsOnClubCommittee = e.IsValidatingMemberOnCommiteeOfClub,
            ValidUntil = e.ValidUntil,
            ValidatedAt = DateTime.UtcNow
        };

        _dbContext.PilotInsuranceProviderValidations.Add(validation);

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotDateOfBirthValidatedByPeer e)
    {
        PilotAgeValidation validation = new()
        {
            PilotId = e.PilotId,
            ValidatedByPilotId = e.ValidatedByPilotId,
            ValidatedByClubId = e.ClubId,
            IsOnClubCommittee = e.IsValidatingMemberOnCommiteeOfClub,
            ValidatedAt = DateTime.UtcNow
        };

        _dbContext.PilotAgeValidations.Add(validation);

        await _dbContext.SaveChangesAsync();
    }

    
}
