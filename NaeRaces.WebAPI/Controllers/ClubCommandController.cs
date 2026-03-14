using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using NaeRaces.Query.Projections;
using NaeRaces.WebAPI.Shared.Club;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace NaeRaces.WebAPI.Controllers;

public class ClubCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;
    private readonly INaeRacesQueryContext _queryContext;

    public ClubCommandController(IAggregateRepository aggregateRepository, INaeRacesQueryContext queryContext)
    {
        _aggregateRepository = aggregateRepository;
        _queryContext = queryContext;
    }

    [Authorize]
    [HttpPost("api/club")]
    public async Task<IActionResult> CreateClubAsync([FromBody] CreateClubRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(request.ClubId == Guid.Empty)
        {
            return BadRequest("Club Id must be set");
        }

        var founderPilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(founderPilotIdClaim, out Guid founderPilotId))
        {
            return Unauthorized();
        }

        Code codeValueType = Code.Create(request.Code);
        Name nameValueType = Name.Create(request.Name);

        if (!await _queryContext.PilotDetails.DoesPilotExist(founderPilotId))
        {
            return BadRequest("Founder pilot not found");
        }

        if (await _queryContext.ClubUniqueness.DoesClubCodeExist(request.Code) || await _queryContext.ClubUniqueness.DoesClubNameExist(request.Code))
        {
            return Conflict("A club with the same code or name already exists.");
        }

        Club newClub = _aggregateRepository.CreateNew<Club>(() => new Club(request.ClubId, codeValueType, nameValueType, founderPilotId));
        newClub.AssignClubMemberRole(founderPilotId, nameof(WebAPI.Shared.Club.ClubMemberRole.Administrator));
        await _aggregateRepository.Save(newClub);

        return Created($"/api/club/{newClub.Id}", new { ClubId = newClub.Id });
    }

    private Task<bool> IsCurrentUserAdmin(Guid clubId)
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Task.FromResult(false);
        }

        return _queryContext.ClubMember.HasClubMemberRole(clubId, pilotId, nameof(WebAPI.Shared.Club.ClubMemberRole.Administrator));
    }

    [Authorize]
    [HttpPut("api/club/{clubId}/details")]
    public async Task<IActionResult> UpdateClubDetailsAsync([FromRoute] Guid clubId,
        [FromBody] UpdateClubDetailsRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Code codeValueType = Code.Create(request.Code);
        Name nameValueType = Name.Create(request.Name);

        if (await _queryContext.ClubUniqueness.DoesClubCodeExist(request.Code) || await _queryContext.ClubUniqueness.DoesClubNameExist(request.Code))
        {
            return Conflict("A club with the same code or name already exists.");
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);

        if (club == null)
        {
            return NotFound();
        }

        club.ChangeClubDetails(codeValueType, nameValueType);
        
        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/description")]
    public async Task<IActionResult> UpdateClubDescriptionAsync([FromRoute] Guid clubId, 
        [FromBody] UpdateClubDescriptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Description cannot be empty.");
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubDescription(request.Description);

        await _aggregateRepository.Save(club);
        
        return Ok();
    }

    [HttpPut("api/club/{clubId}/contactdetails")]
    public async Task<IActionResult> UpdateClubContactDetailsAsync([FromRoute] Guid clubId,
        [FromBody] UpdateClubContactDetailsRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(string.IsNullOrWhiteSpace(request.PhoneNumber) && string.IsNullOrWhiteSpace(request.EmailAddress))
        {
            return BadRequest("Phone number or email address must be provided");
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubContactDetails(request.PhoneNumber ?? string.Empty, request.EmailAddress ?? string.Empty);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/location")]
    public async Task<IActionResult> AddClubLocationAsync([FromRoute] Guid clubId,
        [FromBody] AddClubLocationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (request.LocationInformation.Length > 250)
        {
            return BadRequest("Location information too long");
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        var address = Address.Create(request.AddressLine1, request.AddressLine2, request.City, request.Postcode, request.County);
        var locationId = club.AddClubLocation(request.LocationName, request.LocationInformation, address);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/location/{locationId}", new { LocationId = locationId });
    }

    [HttpDelete("api/club/{clubId}/location/{locationId}")]
    public async Task<IActionResult> RemoveClubLocationAsync([FromRoute] Guid clubId, [FromRoute] int locationId)
    {
        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        if (await _queryContext.ClubLocation.IsLocationInUse(clubId, locationId))
        {
            return BadRequest("Cannot remove location as it is used by a race.");
        }

        club.RemoveClubLocation(locationId);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/location/{locationId}/name")]
    public async Task<IActionResult> RenameClubLocationAsync([FromRoute] Guid clubId,
        [FromRoute] int locationId,
        [FromBody] RenameClubLocationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        Name nameValueType = Name.Create(request.NewLocationName);
        club.RenameClubLocation(locationId, nameValueType);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/location/{locationId}/address")]
    public async Task<IActionResult> ChangeClubLocationAddressAsync([FromRoute] Guid clubId,
        [FromRoute] int locationId,
        [FromBody] ChangeClubLocationAddressRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        var address = Address.Create(request.AddressLine1, request.AddressLine2, request.City, request.Postcode, request.County);
        club.ChangeClubLocationAddress(locationId, address);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/location/{locationId}/information")]
    public async Task<IActionResult> ChangeClubLocationInformationAsync([FromRoute] Guid clubId,
        [FromRoute] int locationId,
        [FromBody] ChangeClubLocationInformationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.ChangeClubLocationInformation(locationId, request.LocationInformation);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/homelocation")]
    public async Task<IActionResult> SetClubHomeLocationAsync([FromRoute] Guid clubId,
        [FromBody] SetClubHomeLocationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubHomeLocation(request.LocationId);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/membershiplevel")]
    public async Task<IActionResult> AddClubMembershipLevelAsync([FromRoute] Guid clubId,
        [FromBody] AddClubMembershipLevelRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        Name nameValueType = Name.Create(request.Name);
        club.AddClubMembershipLevel(nameValueType);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/membershiplevel", null);
    }

    [HttpDelete("api/club/{clubId}/membershiplevel/{membershipLevelId}")]
    public async Task<IActionResult> RemoveClubMembershipLevelAsync([FromRoute] Guid clubId, [FromRoute] int membershipLevelId)
    {
        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RemoveClubMembershipLevel(membershipLevelId);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/membershiplevel/{membershipLevelId}/name")]
    public async Task<IActionResult> RenameClubMembershipLevelAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromBody] RenameClubMembershipLevelRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        Name nameValueType = Name.Create(request.NewName);
        club.RenameClubMembershipLevel(membershipLevelId, nameValueType);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/membershiplevel/{membershipLevelId}/policy")]
    public async Task<IActionResult> SetClubMembershipLevelPolicyAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromBody] SetClubMembershipLevelPolicyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        PilotSelectionPolicyDetails? policyDetails = await _queryContext.PilotSelectionPolicy.GetPolicyDetails(request.PilotSelectionPolicyId, clubId);

        if (policyDetails == null)
        {
            return BadRequest($"Pilot selection policy with ID {request.PilotSelectionPolicyId} does not exist.");
        }

        club.SetClubMembershipLevelPolicy(membershipLevelId, request.PilotSelectionPolicyId, policyDetails.LatestVersion);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpDelete("api/club/{clubId}/membershiplevel/{membershipLevelId}/policy")]
    public async Task<IActionResult> SetClubMembershipLevelMaximumAgeAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.ClearClubMembershipLevelPolicy(membershipLevelId);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption/annual")]
    public async Task<IActionResult> AddClubMembershipLevelAnnualPaymentOptionAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromBody] AddClubMembershipLevelAnnualPaymentOptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Name nameValue = Name.Create(request.Name);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubMembershipLevelAnnualPaymentOption(membershipLevelId, nameValue, request.Currency, request.Price);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption", null);
    }

    [HttpPost("api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption/monthly")]
    public async Task<IActionResult> AddClubMembershipLevelMonthlyPaymentOptionAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromBody] AddClubMembershipLevelMonthlyPaymentOptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Name nameValue = Name.Create(request.Name);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubMembershipLevelMonthlyPaymentOption(membershipLevelId, nameValue, request.DayOfMonthDue, request.PaymentInterval, request.Currency, request.Price);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption", null);
    }

    [HttpPost("api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption/subscription")]
    public async Task<IActionResult> AddClubMembershipLevelSubscriptionPaymentOptionAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromBody] AddClubMembershipLevelSubscriptionPaymentOptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Name nameValue = Name.Create(request.Name);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubMembershipLevelSubscriptionPaymentOption(membershipLevelId, nameValue, request.PaymentInterval, request.Currency, request.Price);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption", null);
    }

    [HttpDelete("api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption/{paymentOptionId}")]
    public async Task<IActionResult> RemoveClubMembershipLevelPaymentOptionAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromRoute] int paymentOptionId)
    {

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RemoveClubMembershipLevelPaymentOption(membershipLevelId, paymentOptionId);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption/{paymentOptionId}/name")]
    public async Task<IActionResult> RenameClubMembershipLevelPaymentOptionAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromRoute] int paymentOptionId,
        [FromBody] RenameClubMembershipLevelPaymentOptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Name nameValue = Name.Create(request.NewName);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RenameClubMembershipLevelPaymentOption(membershipLevelId, paymentOptionId, nameValue);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [Authorize]
    [HttpPost("api/club/{clubId}/pilotmembership")]
    public async Task<IActionResult> RegisterPilotForClubMembershipLevelAsync([FromRoute] Guid clubId,
        [FromBody] RegisterPilotForClubMembershipLevelRequest request)
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

        if(!await _queryContext.PilotDetails.DoesPilotExist(pilotId))
        { 
            return BadRequest("Pilot not found"); 
        }

        var membershipLevel = await _queryContext.ClubMembershipLevel.GetClubMembershipLevel(clubId, request.MembershipLevelId);
        if (membershipLevel == null)
        {
            return BadRequest("Membership level not found");
        }

        if (membershipLevel.PilotPolicyId.HasValue && membershipLevel.PolicyVersion.HasValue)
        {
            var policyValidationResult = await _queryContext.PilotPolicyValidation.ValidatePilotAgainstPolicy(pilotId, membershipLevel.PilotPolicyId.Value, membershipLevel.PolicyVersion.Value, DateTime.UtcNow);

            if (policyValidationResult != null)
            {
                return BadRequest($"Pilot does not meet the requirements for this membership level: {policyValidationResult}");
            }
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RegisterPilotForClubMembershipLevel(request.MembershipLevelId, request.PaymentOptionId, pilotId, request.RegistrationId);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/pilotmembership/{pilotId}", new { PilotId = pilotId, RegistrationId = request.RegistrationId });
    }

    [HttpPut("api/club/{clubId}/pilotmembership/{pilotId}/confirm")]
    public async Task<IActionResult> ConfirmPilotClubMembershipAsync([FromRoute] Guid clubId,
        [FromRoute] Guid pilotId,
        [FromBody] ConfirmPilotClubMembershipRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.ConfirmPilotClubMembership(request.MembershipLevelId, request.PaymentOptionId, pilotId, request.RegistrationId, request.ValidUntil);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/pilotmembership/{pilotId}/manualconfirm")]
    public async Task<IActionResult> ManuallyConfirmPilotClubMembershipAsync([FromRoute] Guid clubId,
        [FromRoute] Guid pilotId,
        [FromBody] ManuallyConfirmPilotClubMembershipRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        if (!await _queryContext.PilotDetails.DoesPilotExist(request.ConfirmedByPilotId))
        {
            return BadRequest("Confirming pilot not found");
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.ManuallyConfirmPilotClubMembership(request.MembershipLevelId, request.PaymentOptionId, pilotId, request.RegistrationId, request.ConfirmedByPilotId, request.ValidUntil);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpDelete("api/club/{clubId}/pilotmembership/{pilotId}")]
    public async Task<IActionResult> CancelPilotClubMembershipAsync([FromRoute] Guid clubId, [FromRoute] Guid pilotId)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        club.CancelPilotClubMembership(pilotId);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/memberrole")]
    public async Task<IActionResult> AssignClubMemberRoleAsync([FromRoute] Guid clubId,
        [FromBody] AssignClubMemberRoleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!Enum.IsDefined<WebAPI.Shared.Club.ClubMemberRole>(request.Role))
        {
            return BadRequest("Invalid role.");
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AssignClubMemberRole(request.PilotId, request.Role.ToString());

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpDelete("api/club/{clubId}/memberrole/{pilotId}/{role}")]
    public async Task<IActionResult> RevokeClubMemberRoleAsync([FromRoute] Guid clubId,
        [FromRoute] Guid pilotId,
        [FromRoute] WebAPI.Shared.Club.ClubMemberRole role)
    {
        if (!Enum.IsDefined<WebAPI.Shared.Club.ClubMemberRole>(role))
        {
            return BadRequest("Invalid role.");
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RevokeClubMemberRole(pilotId, role.ToString());

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/racetag")]
    public async Task<IActionResult> AddClubRaceTagAsync([FromRoute] Guid clubId,
        [FromBody] AddClubRaceTagRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Tag tagValueType = Tag.Create(request.Tag);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubRaceTag(tagValueType, request.Colour);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/racetag/{request.Tag}", new { Tag = request.Tag });
    }

    [HttpDelete("api/club/{clubId}/racetag/{tag}")]
    public async Task<IActionResult> RemoveClubRaceTagAsync([FromRoute] Guid clubId, [FromRoute] string tag)
    {
        if (!await IsCurrentUserAdmin(clubId))
        {
            return Unauthorized();
        }

        Tag tagValueType = Tag.Create(tag);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RemoveClubRaceTag(tagValueType);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    public IActionResult Index()
    {
        return View();
    }
}
