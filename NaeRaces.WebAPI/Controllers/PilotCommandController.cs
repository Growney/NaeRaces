using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;

namespace NaeRaces.WebAPI.Controllers;

public class PilotCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;

    public PilotCommandController(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    [HttpPost("api/pilot")]
    public async Task<IActionResult> RegisterPilotAsync([FromQuery, BindRequired] Guid pilotId,
        [FromQuery, BindRequired] string callSign)
    {
        Pilot newPilot = _aggregateRepository.CreateNew<Pilot>(() => new Pilot(pilotId, callSign));

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

        pilot.ChangePilotCallSign(newCallSign);

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

        pilot.SetPilotNationality(nationality);

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
        [FromQuery, BindRequired] Guid clubId,
        [FromQuery, BindRequired] Guid validatedByPilotId,
        [FromQuery, BindRequired] bool isValidatingMemberOnCommiteeOfClub)
    {
        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        pilot.PeerValidatePilotGovernmentDocumentation(governmentDocument, validUntil, clubId, validatedByPilotId, isValidatingMemberOnCommiteeOfClub);

        await _aggregateRepository.Save(pilot);

        return Created($"/api/pilot/{pilotId}/governmentdocumentvalidation", null);
    }

    [HttpPost("api/pilot/{pilotId}/insurancevalidation")]
    public async Task<IActionResult> PeerValidatePilotInsuranceAsync([FromRoute] Guid pilotId,
        [FromQuery, BindRequired] string insuranceProvider,
        [FromQuery, BindRequired] DateTime validUntil,
        [FromQuery, BindRequired] Guid clubId,
        [FromQuery, BindRequired] Guid validatedByPilotId,
        [FromQuery, BindRequired] bool isValidatingMemberOnCommiteeOfClub)
    {
        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        pilot.PeerValidatePilotInsurance(insuranceProvider, validUntil, clubId, validatedByPilotId, isValidatingMemberOnCommiteeOfClub);

        await _aggregateRepository.Save(pilot);

        return Created($"/api/pilot/{pilotId}/insurancevalidation", null);
    }

    [HttpPost("api/pilot/{pilotId}/dateofbirthvalidation")]
    public async Task<IActionResult> PeerValidatePilotDateOfBirthAsync([FromRoute] Guid pilotId,
        [FromQuery, BindRequired] Guid clubId,
        [FromQuery, BindRequired] Guid validatedByPilotId,
        [FromQuery, BindRequired] bool isValidatingMemberOnCommiteeOfClub)
    {
        Pilot? pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);
        if (pilot == null)
        {
            return NotFound();
        }

        pilot.PeerValidatePilotDateOfBirth(clubId, validatedByPilotId, isValidatingMemberOnCommiteeOfClub);

        await _aggregateRepository.Save(pilot);

        return Created($"/api/pilot/{pilotId}/dateofbirthvalidation", null);
    }
}
