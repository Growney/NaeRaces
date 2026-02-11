using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.Query.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace NaeRaces.WebAPI.Controllers;

public class PilotCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;
    private readonly IClubMemberQueryHandler _clubMemberQueryHandler;

    public PilotCommandController(IAggregateRepository aggregateRepository, IClubMemberQueryHandler clubMemberQueryHandler)
    {
        _aggregateRepository = aggregateRepository;
        _clubMemberQueryHandler = clubMemberQueryHandler;
    }

    [HttpPost("api/pilot")]
    public async Task<IActionResult> RegisterPilotAsync([FromQuery, BindRequired] Guid pilotId,
        [FromQuery, BindRequired] string callSign)
    {
        if(pilotId == Guid.Empty)
        {
            return BadRequest("Pilot id must be set");
        }

        CallSign callSignValueType = CallSign.Create(callSign);

        Pilot newPilot = _aggregateRepository.CreateNew<Pilot>(() => new Pilot(pilotId, callSignValueType));

        await _aggregateRepository.Save(newPilot);

        return Created($"/api/pilot/{newPilot.Id}", new { PilotId = newPilot.Id });
    }

    [HttpPut("api/pilot/{pilotId}/callsign")]
    public async Task<IActionResult> ChangePilotCallSignAsync([FromRoute] Guid pilotId,
        [FromQuery, BindRequired] string newCallSign)
    {
        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        CallSign callSignValueType = CallSign.Create(newCallSign);

        pilot.ChangePilotCallSign(callSignValueType);

        await _aggregateRepository.Save(pilot);

        return Ok();
    }

    [HttpPut("api/pilot/{pilotId}/nationality")]
    public async Task<IActionResult> SetPilotNationalityAsync([FromRoute] Guid pilotId,
        [FromQuery, BindRequired] string nationality)
    {
        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        if(nationality.Length > 50)
        {
            return BadRequest("Nationality Too Long");
        }

        pilot.SetPilotNationality(nationality);

        await _aggregateRepository.Save(pilot);

        return Ok();
    }
    [HttpPut("api/pilot/{pilotId}/name")]
    public async Task<IActionResult> SetPilotNameAsync([FromRoute] Guid pilotId,
        [FromQuery, BindRequired] string name)
    {
        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        if (name.Length > 150)
        {
            return BadRequest("Name too Long");
        }

        pilot.SetPilotName(name);

        await _aggregateRepository.Save(pilot);

        return Ok();
    }
    [HttpPut("api/pilot/{pilotId}/dateofbirth")]
    public async Task<IActionResult> SetPilotDateOfBirthAsync([FromRoute] Guid pilotId,
        [FromQuery, BindRequired] DateTime dateOfBirth)
    {
        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        pilot.SetPilotDateOfBirth(dateOfBirth);

        await _aggregateRepository.Save(pilot);

        return Ok();
    }

    [HttpPost("api/pilot/{pilotId}/governmentdocumentvalidation")]
    public async Task<IActionResult> PeerValidatePilotGovernmentDocumentationAsync([FromRoute] Guid pilotId,
        [FromQuery, BindRequired] string governmentDocument,
        [FromQuery, BindRequired] DateTime validUntil,
        [FromQuery, BindRequired] Guid validatedByPilotId)
    {
        if(string.IsNullOrWhiteSpace(governmentDocument))
        {
            return BadRequest("Government document must be provided");
        }

        if(governmentDocument.Length > 50)
        {
            return BadRequest("Government document value too long");
        }

        if (validUntil < DateTime.UtcNow)
        {
            return BadRequest("Invalid 'validUntil' date");
        }

        if(validatedByPilotId == pilotId)
        {
            return BadRequest("Pilot cannot validate themselves");
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        bool hasAtLeastOneValidatingClubMembership = false;
        await foreach(var clubMembership in _clubMemberQueryHandler.GetPilotMembershipDetails(validatedByPilotId))
        {
            pilot.PeerValidatePilotGovernmentDocumentation(governmentDocument, validUntil, clubMembership.ClubId, pilotId, clubMembership.IsOnCommittee);
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
        [FromQuery, BindRequired] string insuranceProvider,
        [FromQuery, BindRequired] DateTime validUntil,
        [FromQuery, BindRequired] Guid validatedByPilotId)
    {
        if (string.IsNullOrWhiteSpace(insuranceProvider))
        {
            return BadRequest("Insurance provider must be provided");
        }

        if (insuranceProvider.Length > 50)
        {
            return BadRequest("Insurance provider value too long");
        }

        if (validUntil < DateTime.UtcNow)
        {
            return BadRequest("Invalid 'validUntil' date");
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
        await foreach (var clubMembership in _clubMemberQueryHandler.GetPilotMembershipDetails(validatedByPilotId))
        {
            pilot.PeerValidatePilotInsurance(insuranceProvider, validUntil, clubMembership.ClubId, pilotId, clubMembership.IsOnCommittee);
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
    public async Task<IActionResult> PeerValidatePilotDateOfBirthAsync([FromRoute] Guid pilotId,
        [FromQuery, BindRequired] Guid validatedByPilotId)
    {
        if(validatedByPilotId == pilotId)
        {
            return BadRequest("Pilot cannot validate themselves");
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        bool hasAtLeastOneValidatingClubMembership = false;
        await foreach (var clubMembership in _clubMemberQueryHandler.GetPilotMembershipDetails(validatedByPilotId))
        {
            pilot.PeerValidatePilotDateOfBirth(clubMembership.ClubId, pilotId, clubMembership.IsOnCommittee);
            hasAtLeastOneValidatingClubMembership = true;
        }

        if (!hasAtLeastOneValidatingClubMembership)
        {
            return BadRequest("Validating pilot must have at least one club membership");
        }

        await _aggregateRepository.Save(pilot);

        return Created($"/api/pilot/{pilotId}/dateofbirthvalidation", null);
    }
}
