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

    private Task When(PilotSelectionPolicyMinimumAgeRequirementAdded e)
    {
        _dbContext.PilotPolicyStatements.Add(new PilotPolicyStatement
        {
            PolicyId = e.PilotSelectionPolicyId,
            StatementId = e.PolicyStatementId,
            StatementType = "MinimumAge",
            MinimumAge = e.MinimumAge,
            ValidationPolicy = e.ValidationPolicy
        });

        return _dbContext.SaveChangesAsync();
    }

    private Task When(PilotSelectionPolicyMaximumAgeRequirementAdded e)
    {
        _dbContext.PilotPolicyStatements.Add(new PilotPolicyStatement
        {
            PolicyId = e.PilotSelectionPolicyId,
            StatementId = e.PolicyStatementId,
            StatementType = "MaximumAge",
            MaximumAge = e.MaximumAge,
            ValidationPolicy = e.ValidationPolicy
        });

        return _dbContext.SaveChangesAsync();
    }

    private Task When(PilotSelectionPolicyInsuranceProviderRequirementAdded e)
    {
        _dbContext.PilotPolicyStatements.Add(new PilotPolicyStatement
        {
            PolicyId = e.PilotSelectionPolicyId,
            StatementId = e.PolicyStatementId,
            StatementType = "InsuranceProvider",
            InsuranceProvider = e.InsuranceProvider,
            ValidationPolicy = e.ValidationPolicy
        });

        return _dbContext.SaveChangesAsync();
    }

    private Task When(PilotSelectionPolicyGovernmentDocumentValidationRequirementAdded e)
    {
        _dbContext.PilotPolicyStatements.Add(new PilotPolicyStatement
        {
            PolicyId = e.PilotSelectionPolicyId,
            StatementId = e.PolicyStatementId,
            StatementType = "GovernmentDocument",
            GovernmentDocument = e.GovernmentDocument,
            ValidationPolicy = e.ValidationPolicy
        });

        return _dbContext.SaveChangesAsync();
    }

    private Task When(PilotSelectionPolicyClubRequirementAdded e)
    {
        _dbContext.PilotPolicyStatements.Add(new PilotPolicyStatement
        {
            PolicyId = e.PilotSelectionPolicyId,
            StatementId = e.PolicyStatementId,
            StatementType = "ClubMembership",
            RequiredClubId = e.ClubId
        });

        return _dbContext.SaveChangesAsync();
    }

    private Task When(PilotSelectionPolicyClubMembershipLevelRequirementAdded e)
    {
        _dbContext.PilotPolicyStatements.Add(new PilotPolicyStatement
        {
            PolicyId = e.PilotSelectionPolicyId,
            StatementId = e.PolicyStatementId,
            StatementType = "ClubMembershipLevel",
            RequiredClubId = e.ClubId,
            RequiredMembershipLevelId = e.MembershipLevel
        });

        return _dbContext.SaveChangesAsync();
    }

    private Task When(PilotSelectionPolicyStatementAdded e)
    {
        _dbContext.PilotPolicyStatements.Add(new PilotPolicyStatement
        {
            PolicyId = e.PilotSelectionPolicyId,
            StatementId = e.PolicyStatementId,
            StatementType = "Composite",
            LeftHandStatementId = e.LeftHandStatementId,
            Operand = e.Operand,
            RightHandStatementId = e.RightHandStatementId,
            IsWithinBrackets = e.IsWithinBrackets
        });

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotSelectionPolicyStatementRemoved e)
    {
        var stmt = await _dbContext.PilotPolicyStatements
            .SingleOrDefaultAsync(s => s.PolicyId == e.PilotSelectionPolicyId && s.StatementId == e.PolicyStatementId);

        if (stmt != null)
            _dbContext.PilotPolicyStatements.Remove(stmt);

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotSelectionPolicyRootStatementSet e)
    {
        var policy = await _dbContext.PilotSelectionPolicyDetails
            .SingleOrDefaultAsync(p => p.Id == e.PilotSelectionPolicyId);

        if (policy != null)
            policy.RootStatementId = e.RootPolicyStatementId;

        await _dbContext.SaveChangesAsync();
    }
}
