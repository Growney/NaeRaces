using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.Query.Abstractions;
using NaeRaces.WebAPI.Models.Club;

namespace NaeRaces.WebAPI.Controllers;

public class ClubCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;
    private readonly IClubUniquenessQueryHandler _clubUniquenessQueryHandler;
    private readonly IPilotDetailsQueryHandler _pilotDetailsQueryHandler;
    private readonly IClubLocationQueryHandler _clubLocationQueryHandler;

    public ClubCommandController(IAggregateRepository aggregateRepository, IClubUniquenessQueryHandler clubUniquenessQueryHandler, IPilotDetailsQueryHandler pilotDetailsQueryHandler, IClubLocationQueryHandler clubLocationQueryHandler)
    {
        _aggregateRepository = aggregateRepository;
        _clubUniquenessQueryHandler = clubUniquenessQueryHandler;
        _pilotDetailsQueryHandler = pilotDetailsQueryHandler;
        _clubLocationQueryHandler = clubLocationQueryHandler;
    }

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

        Code codeValueType = Code.Create(request.Code);
        Name nameValueType = Name.Create(request.Name);

        if (!await _pilotDetailsQueryHandler.DoesPilotExist(request.FounderPilotId))
        {
            return BadRequest("Founder pilot not found");
        }

        if (await _clubUniquenessQueryHandler.DoesClubCodeExist(request.Code) || await _clubUniquenessQueryHandler.DoesClubNameExist(request.Code))
        {
            return Conflict("A club with the same code or name already exists.");
        }

        Club newClub = _aggregateRepository.CreateNew<Club>(() => new Club(request.ClubId, codeValueType, nameValueType, request.FounderPilotId));

        await _aggregateRepository.Save(newClub);

        return Created($"/api/club/{newClub.Id}", new { ClubId = newClub.Id });
    }

    [HttpPut("api/club/{clubId}/details")]
    public async Task<IActionResult> UpdateClubDetailsAsync([FromRoute] Guid clubId, 
        [FromBody] UpdateClubDetailsRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Code codeValueType = Code.Create(request.Code);
        Name nameValueType = Name.Create(request.Name);

        if (await _clubUniquenessQueryHandler.DoesClubCodeExist(request.Code) || await _clubUniquenessQueryHandler.DoesClubNameExist(request.Code))
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
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        if(await _clubLocationQueryHandler.IsLocationInUse(clubId, locationId))
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

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubHomeLocation(request.LocationId);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/minimumage")]
    public async Task<IActionResult> SetClubMinimumAgeAsync([FromRoute] Guid clubId,
        [FromBody] SetClubAgeRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy policy = ValidationPolicy.Create(request.ValidationPolicy);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubMinimumAge(request.Age, policy);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpDelete("api/club/{clubId}/minimumage")]
    public async Task<IActionResult> RemoveClubMinimumAgeAsync([FromRoute] Guid clubId)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RemoveClubMinimumAge();

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/maximumage")]
    public async Task<IActionResult> SetClubMaximumAgeAsync([FromRoute] Guid clubId,
        [FromBody] SetClubAgeRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy policy = ValidationPolicy.Create(request.ValidationPolicy);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubMaximumAge(request.Age, policy);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpDelete("api/club/{clubId}/maximumage")]
    public async Task<IActionResult> RemoveClubMaximumAgeAsync([FromRoute] Guid clubId)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RemoveClubMaximumAge();

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/insuranceproviderrequirement")]
    public async Task<IActionResult> AddClubInsuranceProviderRequirementAsync([FromRoute] Guid clubId,
        [FromBody] AddClubInsuranceProviderRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy validation = ValidationPolicy.Create(request.ValidationPolicy);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubInsuranceProviderRequirement(request.InsuranceProvider, validation);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/insuranceproviderrequirement/{request.InsuranceProvider}", new { InsuranceProvider = request.InsuranceProvider });
    }

    [HttpDelete("api/club/{clubId}/insuranceproviderrequirement/{insuranceProvider}")]
    public async Task<IActionResult> RemoveClubInsuranceProviderRequirementAsync([FromRoute] Guid clubId,
        [FromRoute] string insuranceProvider)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RemoveClubInsuranceProviderRequirement(insuranceProvider);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/governmentdocumentvalidationrequirement")]
    public async Task<IActionResult> AddClubGovernmentDocumentValidationRequirementAsync([FromRoute] Guid clubId,
        [FromBody] AddClubGovernmentDocumentValidationRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy validation = ValidationPolicy.Create(request.ValidationPolicy);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubGovernmentDocumentValidationRequirement(request.GovernmentDocument, validation);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/governmentdocumentvalidationrequirement/{request.GovernmentDocument}", new { GovernmentDocument = request.GovernmentDocument });
    }

    [HttpDelete("api/club/{clubId}/governmentdocumentvalidationrequirement/{governmentDocument}")]
    public async Task<IActionResult> RemoveClubGovernmentDocumentValidationRequirementAsync([FromRoute] Guid clubId,
        [FromRoute] string governmentDocument)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RemoveClubGovernmentDocumentValidationRequirement(governmentDocument);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/committeemember")]
    public async Task<IActionResult> AddClubCommitteeMemberAsync([FromRoute] Guid clubId,
        [FromBody] AddClubCommitteeMemberRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubCommitteeMember(request.PilotId, request.Role);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/committeemember/{request.PilotId}", new { PilotId = request.PilotId });
    }

    [HttpDelete("api/club/{clubId}/committeemember/{pilotId}")]
    public async Task<IActionResult> RemoveClubCommitteeMemberAsync([FromRoute] Guid clubId, [FromRoute] Guid pilotId)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RemoveClubCommitteeMember(pilotId);

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

    [HttpPut("api/club/{clubId}/membershiplevel/{membershipLevelId}/minimumage")]
    public async Task<IActionResult> SetClubMembershipLevelAgeAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromBody] SetClubMembershipLevelAgeRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy validation = ValidationPolicy.Create(request.ValidationPolicy);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubMembershipLevelAge(membershipLevelId, request.Age, validation);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/membershiplevel/{membershipLevelId}/maximumage")]
    public async Task<IActionResult> SetClubMembershipLevelMaximumAgeAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromBody] SetClubMembershipLevelAgeRequirementRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ValidationPolicy validation = ValidationPolicy.Create(request.ValidationPolicy);

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubMembershipLevelMaximumAge(membershipLevelId, request.Age, validation);

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

    [HttpPost("api/club/{clubId}/pilotmembership")]
    public async Task<IActionResult> RegisterPilotForClubMembershipLevelAsync([FromRoute] Guid clubId,
        [FromBody] RegisterPilotForClubMembershipLevelRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(!await _pilotDetailsQueryHandler.DoesPilotExist(request.PilotId))
        { 
            return BadRequest("Pilot not found"); 
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RegisterPilotForClubMembershipLevel(request.MembershipLevelId, request.PaymentOptionId, request.PilotId, request.RegistrationId);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/pilotmembership/{request.PilotId}", new { PilotId = request.PilotId, RegistrationId = request.RegistrationId });
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

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.ConfirmPilotClubMembership(request.MembershipLevelId, request.PaymentOptionId, pilotId, request.RegistrationId, request.ValidUntil);

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

        club.CancelPilotClubMembership(pilotId);

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
