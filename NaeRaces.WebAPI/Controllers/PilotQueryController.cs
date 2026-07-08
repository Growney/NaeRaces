using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaeRaces.Query.Projections;
using NaeRaces.WebAPI.Shared.Pilot;
using OpenIddict.Abstractions;

namespace NaeRaces.WebAPI.Controllers;

public class PilotQueryController : Controller
{
    private readonly IProjectionProvider _projectionProvider;

    public PilotQueryController(IProjectionProvider projectionProvider)
    {
        _projectionProvider = projectionProvider ?? throw new ArgumentNullException(nameof(projectionProvider));
    }

    [Authorize]
    [HttpGet("api/pilot/query/profile/{pilotId:guid}")]
    public async Task<IActionResult> GetProfileAsync(Guid pilotId)
    {
        var projection = await _projectionProvider.CloneAsync<PilotProfile>();
        var profile = projection.Object.GetPilotProfile(pilotId);

        if (profile is null)
        {
            return NotFound();
        }

        var response = new PilotProfileResponse
        {
            PilotId = profile.PilotId,
            CallSign = profile.CallSign,
            Email = profile.Email,
            UserName = profile.UserName,
            Name = profile.Name,
            Nationality = profile.Nationality,
            DateOfBirth = profile.DateOfBirth,
            Roles = profile.Roles.ToList(),
            GovernmentDocumentValidations = profile.GovernmentDocumentValidations.Select(x => new PilotProfileGovernmentDocumentValidationResponse
            {
                GovernmentDocument = x.GovernmentDocument,
                ValidUntil = x.ValidUntil,
                ClubId = x.ClubId,
                ValidatedByPilotId = x.ValidatedByPilotId,
                IsValidatingMemberOnCommiteeOfClub = x.IsValidatingMemberOnCommiteeOfClub
            }).ToList(),
            InsuranceValidations = profile.InsuranceValidations.Select(x => new PilotProfileInsuranceValidationResponse
            {
                InsuranceProvider = x.InsuranceProvider,
                ValidUntil = x.ValidUntil,
                ClubId = x.ClubId,
                ValidatedByPilotId = x.ValidatedByPilotId,
                IsValidatingMemberOnCommiteeOfClub = x.IsValidatingMemberOnCommiteeOfClub
            }).ToList(),
            DateOfBirthValidations = profile.DateOfBirthValidations.Select(x => new PilotProfileDateOfBirthValidationResponse
            {
                ClubId = x.ClubId,
                ValidatedByPilotId = x.ValidatedByPilotId,
                IsValidatingMemberOnCommiteeOfClub = x.IsValidatingMemberOnCommiteeOfClub
            }).ToList(),
            Clubs = profile.Clubs.Select(x => new PilotProfileClubResponse
            {
                ClubId = x.ClubId,
                ClubCode = x.ClubCode,
                ClubName = x.ClubName,
                Relationship = new PilotProfileClubRelationshipResponse
                {
                    IsFollowing = x.Relationship.IsFollowing,
                    Roles = x.Relationship.Roles.ToList(),
                    Membership = x.Relationship.Membership is null ? null : new PilotProfileClubMembershipResponse
                    {
                        MembershipLevelId = x.Relationship.Membership.MembershipLevelId,
                        MembershipLevelName = x.Relationship.Membership.MembershipLevelName,
                        PaymentOptionId = x.Relationship.Membership.PaymentOptionId,
                        PaymentOptionName = x.Relationship.Membership.PaymentOptionName,
                        IsConfirmed = x.Relationship.Membership.IsConfirmed,
                        ValidUntil = x.Relationship.Membership.ValidUntil
                    }
                }
            }).ToList()
        };

        return Ok(response);
    }
}
