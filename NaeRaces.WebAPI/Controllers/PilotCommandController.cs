using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.Query.Abstractions;
using NaeRaces.WebAPI.Shared.Club;
using NaeRaces.WebAPI.Shared.Pilot;
using OpenIddict.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace NaeRaces.WebAPI.Controllers;

[Authorize]
public class PilotCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;
    private readonly INaeRacesQueryContext _queryContext;

    public PilotCommandController(IAggregateRepository aggregateRepository, INaeRacesQueryContext queryContext)
    {
        _aggregateRepository = aggregateRepository;
        _queryContext = queryContext;
    }

    [HttpPut("api/pilot/callsign")]
    public async Task<IActionResult> ChangePilotCallSignAsync([FromBody] ChangePilotCallSignRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        CallSign callSignValueType = CallSign.Create(request.NewCallSign);

        pilot.ChangePilotCallSign(callSignValueType);

        await _aggregateRepository.Save(pilot);

        return Ok();
    }

    [HttpPut("api/pilot/nationality")]
    public async Task<IActionResult> SetPilotNationalityAsync([FromBody] SetPilotNationalityRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        pilot.SetPilotNationality(request.Nationality);

        await _aggregateRepository.Save(pilot);

        return Ok();
    }
    [HttpPut("api/pilot/name")]
    public async Task<IActionResult> SetPilotNameAsync([FromBody] SetPilotNameRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        pilot.SetPilotName(request.Name);

        await _aggregateRepository.Save(pilot);

        return Ok();
    }
    [HttpPut("api/pilot/dateofbirth")]
    public async Task<IActionResult> SetPilotDateOfBirthAsync([FromBody] SetPilotDateOfBirthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        pilot.SetPilotDateOfBirth(request.DateOfBirth);

        await _aggregateRepository.Save(pilot);

        return Ok();
    }

    [HttpPost("api/pilot/{pilotId}/governmentdocumentvalidation")]
    public async Task<IActionResult> PeerValidatePilotGovernmentDocumentationAsync([FromRoute] Guid pilotId,
        [FromBody] PeerValidatePilotGovernmentDocumentationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(string.IsNullOrWhiteSpace(request.GovernmentDocument))
        {
            return BadRequest("Government document must be provided");
        }

        if (request.ValidUntil < DateTime.UtcNow)
        {
            return BadRequest("Invalid 'validUntil' date");
        }

        var validatedByPilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(validatedByPilotIdClaim, out Guid validatedByPilotId))
        {
            return Unauthorized();
        }

        if (validatedByPilotId == pilotId)
        {
            return BadRequest("Pilot cannot validate themselves");
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        bool hasAtLeastOneValidatingClubMembership = false;
        await foreach(var clubMembership in _queryContext.ClubMember.GetPilotMembershipDetails(validatedByPilotId))
        {
            bool isTrustee = await _queryContext.ClubMember.HasClubMemberRole(clubMembership.ClubId, validatedByPilotId, nameof(ClubMemberRole.Trustee));
            pilot.PeerValidatePilotGovernmentDocumentation(request.GovernmentDocument, request.ValidUntil, clubMembership.ClubId, pilotId, isTrustee);
            hasAtLeastOneValidatingClubMembership = true;
        }

        if (!hasAtLeastOneValidatingClubMembership)
        {
            return BadRequest("Validating pilot must have at least one club membership");
        }

        await _aggregateRepository.Save(pilot);

        return Created($"/api/pilot/{pilotId}/governmentdocumentvalidation", null);
    }

    [HttpPost("api/pilot/{pilotId}/insurancevalidation")]
    public async Task<IActionResult> PeerValidatePilotInsuranceAsync([FromRoute] Guid pilotId,
        [FromBody] PeerValidatePilotInsuranceRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrWhiteSpace(request.InsuranceProvider))
        {
            return BadRequest("Insurance provider must be provided");
        }

        if (request.ValidUntil < DateTime.UtcNow)
        {
            return BadRequest("Invalid 'validUntil' date");
        }

        var validatedByPilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(validatedByPilotIdClaim, out Guid validatedByPilotId))
        {
            return Unauthorized();
        }

        if (validatedByPilotId == pilotId)
        {
            return BadRequest("Pilot cannot validate themselves");
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        bool hasAtLeastOneValidatingClubMembership = false;
        await foreach (var clubMembership in _queryContext.ClubMember.GetPilotMembershipDetails(validatedByPilotId))
        {
            bool isTrustee = await _queryContext.ClubMember.HasClubMemberRole(clubMembership.ClubId, validatedByPilotId, nameof(ClubMemberRole.Trustee));
            pilot.PeerValidatePilotInsurance(request.InsuranceProvider, request.ValidUntil, clubMembership.ClubId, pilotId, isTrustee);
            hasAtLeastOneValidatingClubMembership = true;
        }

        if (!hasAtLeastOneValidatingClubMembership)
        {
            return BadRequest("Validating pilot must have at least one club membership");
        }

        await _aggregateRepository.Save(pilot);

        return Created($"/api/pilot/{pilotId}/insurancevalidation", null);
    }

    [HttpPost("api/pilot/{pilotId}/dateofbirthvalidation")]
    public async Task<IActionResult> PeerValidatePilotDateOfBirthAsync([FromRoute] Guid pilotId)
    {
        var validatedByPilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(validatedByPilotIdClaim, out Guid validatedByPilotId))
        {
            return Unauthorized();
        }

        if (validatedByPilotId == pilotId)
        {
            return BadRequest("Pilot cannot validate themselves");
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        bool hasAtLeastOneValidatingClubMembership = false;
        await foreach (var clubMembership in _queryContext.ClubMember.GetPilotMembershipDetails(validatedByPilotId))
        {
            bool isTrustee = await _queryContext.ClubMember.HasClubMemberRole(clubMembership.ClubId, validatedByPilotId, nameof(ClubMemberRole.Trustee));
            pilot.PeerValidatePilotDateOfBirth(clubMembership.ClubId, pilotId, isTrustee);
            hasAtLeastOneValidatingClubMembership = true;
        }

        if (!hasAtLeastOneValidatingClubMembership)
        {
            return BadRequest("Validating pilot must have at least one club membership");
        }

        await _aggregateRepository.Save(pilot);

        return Created($"/api/pilot/{pilotId}/dateofbirthvalidation", null);
    }

    [HttpPost("api/pilot/follow/{clubId}")]
    public async Task<IActionResult> FollowClubAsync([FromRoute] Guid clubId)
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        if (!await _queryContext.ClubDetails.DoesClubExist(clubId))
        {
            return NotFound();
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        pilot.FollowClub(clubId);

        await _aggregateRepository.Save(pilot);

        return Ok();
    }

    [HttpDelete("api/pilot/follow/{clubId}")]
    public async Task<IActionResult> UnfollowClubAsync([FromRoute] Guid clubId)
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        pilot.UnfollowClub(clubId);

        await _aggregateRepository.Save(pilot);

        return Ok();
    }
}
