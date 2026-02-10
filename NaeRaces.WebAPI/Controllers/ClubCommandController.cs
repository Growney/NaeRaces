using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;

namespace NaeRaces.WebAPI.Controllers;

public class ClubCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;

    public ClubCommandController(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    [HttpPost("api/club")]
    public async Task<IActionResult> CreateClubAsync([FromQuery, BindRequired] Guid clubId, 
        [FromQuery, BindRequired] string code, 
        [FromQuery, BindRequired] string name, 
        [FromQuery, BindRequired] Guid founderPilotId)
    {

        Code codeValueType = Code.Create(code);
        Name nameValueType = Name.Create(name);

        Club newClub = _aggregateRepository.CreateNew<Club>(() => new Club(clubId, codeValueType, nameValueType, founderPilotId));

        await _aggregateRepository.Save(newClub);

        return Created($"/api/club/{newClub.Id}", new { ClubId = newClub.Id });
    }

    [HttpPut("api/club/{clubId}/details")]
    public async Task<IActionResult> UpdateClubDetailsAsync([FromRoute] Guid clubId, 
        [FromQuery, BindRequired] string code, 
        [FromQuery, BindRequired] string name)
    {
        Code codeValueType = Code.Create(code);
        Name nameValueType = Name.Create(name);

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
        [FromQuery, BindRequired] string description)
    {
        
        if(string.IsNullOrWhiteSpace(description))
        {
            return BadRequest("Description cannot be empty.");
        }

        if(description.Length > 500)
        {
            return BadRequest("Description cannot exceed 500 characters.");
        }

        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubDescription(description);

        await _aggregateRepository.Save(club);
        
        return Ok();
    }

    [HttpPut("api/club/{clubId}/contactdetails")]
    public async Task<IActionResult> UpdateClubContactDetailsAsync([FromRoute] Guid clubId,
        [FromQuery] string phoneNumber,
        [FromQuery] string emailAddress)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubContactDetails(phoneNumber, emailAddress);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/location")]
    public async Task<IActionResult> AddClubLocationAsync([FromRoute] Guid clubId,
        [FromQuery, BindRequired] int locationId,
        [FromQuery, BindRequired] string locationName,
        [FromQuery, BindRequired] string locationInformation,
        [FromQuery, BindRequired] string addressLine1,
        [FromQuery] string? addressLine2,
        [FromQuery, BindRequired] string city,
        [FromQuery, BindRequired] string postcode,
        [FromQuery, BindRequired] string county)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        var address = Address.Create(addressLine1, addressLine2, city, postcode, county);
        club.AddClubLocation(locationId, locationName, locationInformation, address);

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

        club.RemoveClubLocation(locationId);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/location/{locationId}/name")]
    public async Task<IActionResult> RenameClubLocationAsync([FromRoute] Guid clubId,
        [FromRoute] int locationId,
        [FromQuery, BindRequired] string newLocationName)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        Name nameValueType = Name.Create(newLocationName);
        club.RenameClubLocation(locationId, nameValueType);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/location/{locationId}/address")]
    public async Task<IActionResult> ChangeClubLocationAddressAsync([FromRoute] Guid clubId,
        [FromRoute] int locationId,
        [FromQuery, BindRequired] string addressLine1,
        [FromQuery] string? addressLine2,
        [FromQuery, BindRequired] string city,
        [FromQuery, BindRequired] string postcode,
        [FromQuery, BindRequired] string county)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        var address = Address.Create(addressLine1, addressLine2, city, postcode, county);
        club.ChangeClubLocationAddress(locationId, address);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/location/{locationId}/information")]
    public async Task<IActionResult> ChangeClubLocationInformationAsync([FromRoute] Guid clubId,
        [FromRoute] int locationId,
        [FromQuery, BindRequired] string newLocationInformation)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.ChangeClubLocationInformation(locationId, newLocationInformation);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/homelocation")]
    public async Task<IActionResult> SetClubHomeLocationAsync([FromRoute] Guid clubId,
        [FromQuery, BindRequired] int locationId)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubHomeLocation(locationId);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/minimumage")]
    public async Task<IActionResult> SetClubMinimumAgeAsync([FromRoute] Guid clubId,
        [FromQuery, BindRequired] int minimumAge,
        [FromQuery, BindRequired] string validationPolicy)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubMinimumAge(minimumAge, validationPolicy);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/maximumage")]
    public async Task<IActionResult> SetClubMaximumAgeAsync([FromRoute] Guid clubId,
        [FromQuery, BindRequired] int maximumAge,
        [FromQuery, BindRequired] string validationPolicy)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubMaximumAge(maximumAge, validationPolicy);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/insurancerequirement")]
    public async Task<IActionResult> SetClubInsuranceRequirementAsync([FromRoute] Guid clubId,
        [FromQuery, BindRequired] bool isRequired,
        [FromQuery] string[]? acceptedInsuranceProviders)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubInsuranceRequirement(isRequired, acceptedInsuranceProviders ?? []);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/governmentdocumentvalidationrequirement")]
    public async Task<IActionResult> SetClubGovernmentDocumentValidationRequirementAsync([FromRoute] Guid clubId,
        [FromQuery, BindRequired] bool isRequired)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubGovernmentDocumentValidationRequirement(isRequired);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/committeemember")]
    public async Task<IActionResult> AddClubCommitteeMemberAsync([FromRoute] Guid clubId,
        [FromQuery, BindRequired] Guid pilotId,
        [FromQuery, BindRequired] string role)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubCommitteeMember(pilotId, role);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/committeemember/{pilotId}", new { PilotId = pilotId });
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
        [FromQuery, BindRequired] string name)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        Name nameValueType = Name.Create(name);
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
        [FromQuery, BindRequired] string newName)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        Name nameValueType = Name.Create(newName);
        club.RenameClubMembershipLevel(membershipLevelId, nameValueType);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/membershiplevel/{membershipLevelId}/minimumage")]
    public async Task<IActionResult> SetClubMembershipLevelAgeAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromQuery, BindRequired] int minimumAge,
        [FromQuery, BindRequired] string validationPolicy)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubMembershipLevelAge(membershipLevelId, minimumAge, validationPolicy);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPut("api/club/{clubId}/membershiplevel/{membershipLevelId}/maximumage")]
    public async Task<IActionResult> SetClubMembershipLevelMaximumAgeAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromQuery, BindRequired] int maximumAge,
        [FromQuery, BindRequired] string validationPolicy)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.SetClubMembershipLevelMaximumAge(membershipLevelId, maximumAge, validationPolicy);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption/annual")]
    public async Task<IActionResult> AddClubMembershipLevelAnnualPaymentOptionAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromQuery, BindRequired] string name,
        [FromQuery, BindRequired] string currency,
        [FromQuery, BindRequired] decimal price)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubMembershipLevelAnnualPaymentOption(membershipLevelId, name, currency, price);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption", null);
    }

    [HttpPost("api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption/monthly")]
    public async Task<IActionResult> AddClubMembershipLevelMonthlyPaymentOptionAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromQuery, BindRequired] string name,
        [FromQuery, BindRequired] int dayOfMonthDue,
        [FromQuery, BindRequired] int paymentInterval,
        [FromQuery, BindRequired] string currency,
        [FromQuery, BindRequired] decimal price)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubMembershipLevelMonthlyPaymentOption(membershipLevelId, name, dayOfMonthDue, paymentInterval, currency, price);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption", null);
    }

    [HttpPost("api/club/{clubId}/membershiplevel/{membershipLevelId}/paymentoption/subscription")]
    public async Task<IActionResult> AddClubMembershipLevelSubscriptionPaymentOptionAsync([FromRoute] Guid clubId,
        [FromRoute] int membershipLevelId,
        [FromQuery, BindRequired] string name,
        [FromQuery, BindRequired] int paymentInterval,
        [FromQuery, BindRequired] string currency,
        [FromQuery, BindRequired] decimal price)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubMembershipLevelSubscriptionPaymentOption(membershipLevelId, name, paymentInterval, currency, price);

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
        [FromQuery, BindRequired] string newName)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RenameClubMembershipLevelPaymentOption(membershipLevelId, paymentOptionId, newName);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    [HttpPost("api/club/{clubId}/pilotmembership")]
    public async Task<IActionResult> RegisterPilotForClubMembershipLevelAsync([FromRoute] Guid clubId,
        [FromQuery, BindRequired] int membershipLevelId,
        [FromQuery, BindRequired] int paymentOptionId,
        [FromQuery, BindRequired] Guid pilotId,
        [FromQuery, BindRequired] Guid registrationId)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RegisterPilotForClubMembershipLevel(membershipLevelId, paymentOptionId, pilotId, registrationId);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/pilotmembership/{pilotId}", new { PilotId = pilotId, RegistrationId = registrationId });
    }

    [HttpPut("api/club/{clubId}/pilotmembership/{pilotId}/confirm")]
    public async Task<IActionResult> ConfirmPilotClubMembershipAsync([FromRoute] Guid clubId,
        [FromRoute] Guid pilotId,
        [FromQuery, BindRequired] int membershipLevelId,
        [FromQuery, BindRequired] int paymentOptionId,
        [FromQuery, BindRequired] Guid registrationId)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.ConfirmPilotClubMembership(membershipLevelId, paymentOptionId, pilotId, registrationId);

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
        [FromQuery, BindRequired] string tag,
        [FromQuery, BindRequired] string colour)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.AddClubRaceTag(tag, colour);

        await _aggregateRepository.Save(club);

        return Created($"/api/club/{clubId}/racetag/{tag}", new { Tag = tag });
    }

    [HttpDelete("api/club/{clubId}/racetag/{tag}")]
    public async Task<IActionResult> RemoveClubRaceTagAsync([FromRoute] Guid clubId, [FromRoute] string tag)
    {
        Club? club = await _aggregateRepository.Get<Club, Guid>(clubId);
        if (club == null)
        {
            return NotFound();
        }

        club.RemoveClubRaceTag(tag);

        await _aggregateRepository.Save(club);

        return Ok();
    }

    public IActionResult Index()
    {
        return View();
    }
}
