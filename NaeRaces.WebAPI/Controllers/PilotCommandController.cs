using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.Query.Abstractions;
using NaeRaces.WebAPI.Models.Pilot;
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
    public async Task<IActionResult> RegisterPilotAsync([FromBody] RegisterPilotRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(request.PilotId == Guid.Empty)
        {
            return BadRequest("Pilot id must be set");
        }

        CallSign callSignValueType = CallSign.Create(request.CallSign);

        Pilot newPilot = _aggregateRepository.CreateNew<Pilot>(() => new Pilot(request.PilotId, callSignValueType));

        await _aggregateRepository.Save(newPilot);

        return Created($"/api/pilot/{newPilot.Id}", new { PilotId = newPilot.Id });
    }

    [HttpPut("api/pilot/{pilotId}/callsign")]
    public async Task<IActionResult> ChangePilotCallSignAsync([FromRoute] Guid pilotId,
        [FromBody] ChangePilotCallSignRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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

    [HttpPut("api/pilot/{pilotId}/nationality")]
    public async Task<IActionResult> SetPilotNationalityAsync([FromRoute] Guid pilotId,
        [FromBody] SetPilotNationalityRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
    [HttpPut("api/pilot/{pilotId}/name")]
    public async Task<IActionResult> SetPilotNameAsync([FromRoute] Guid pilotId,
        [FromBody] SetPilotNameRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
    [HttpPut("api/pilot/{pilotId}/dateofbirth")]
    public async Task<IActionResult> SetPilotDateOfBirthAsync([FromRoute] Guid pilotId,
        [FromBody] SetPilotDateOfBirthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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

        if(request.ValidatedByPilotId == pilotId)
        {
            return BadRequest("Pilot cannot validate themselves");
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        bool hasAtLeastOneValidatingClubMembership = false;
        await foreach(var clubMembership in _clubMemberQueryHandler.GetPilotMembershipDetails(request.ValidatedByPilotId))
        {
            pilot.PeerValidatePilotGovernmentDocumentation(request.GovernmentDocument, request.ValidUntil, clubMembership.ClubId, pilotId, clubMembership.IsOnCommittee);
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

        if (request.ValidatedByPilotId == pilotId)
        {
            return BadRequest("Pilot cannot validate themselves");
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        bool hasAtLeastOneValidatingClubMembership = false;
        await foreach (var clubMembership in _clubMemberQueryHandler.GetPilotMembershipDetails(request.ValidatedByPilotId))
        {
            pilot.PeerValidatePilotInsurance(request.InsuranceProvider, request.ValidUntil, clubMembership.ClubId, pilotId, clubMembership.IsOnCommittee);
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
        [FromBody] PeerValidatePilotDateOfBirthRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(request.ValidatedByPilotId == pilotId)
        {
            return BadRequest("Pilot cannot validate themselves");
        }

        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        bool hasAtLeastOneValidatingClubMembership = false;
        await foreach (var clubMembership in _clubMemberQueryHandler.GetPilotMembershipDetails(request.ValidatedByPilotId))
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
