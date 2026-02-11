using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;

namespace NaeRaces.WebAPI.Controllers;

public class RaceCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;

    public RaceCommandController(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    [HttpPost("api/race")]
    public async Task<IActionResult> PlanRaceAsync([FromQuery, BindRequired] Guid raceId,
        [FromQuery, BindRequired] string name,
        [FromQuery, BindRequired] Guid clubId,
        [FromQuery, BindRequired] int locationId)
    {
        if(raceId == Guid.Empty)
        {
            return BadRequest("RaceId must be a non-empty GUID.");
        }

        Name nameValueType = Name.Create(name);

        Race newRace = _aggregateRepository.CreateNew<Race>(() => new Race(raceId, nameValueType, clubId, locationId));

        await _aggregateRepository.Save(newRace);

        return Created($"/api/race/{newRace.Id}", new { RaceId = newRace.Id });
    }

    [HttpPost("api/race/team")]
    public async Task<IActionResult> PlanTeamRaceAsync([FromQuery, BindRequired] Guid raceId,
        [FromQuery, BindRequired] string name,
        [FromQuery, BindRequired] int teamSize)
    {
        if(raceId == Guid.Empty)
        {
            return BadRequest("RaceId must be a non-empty GUID.");
        }

        Name nameValueType = Name.Create(name);

        Race newRace = _aggregateRepository.CreateNew<Race>(() => new Race(raceId, nameValueType, teamSize));

        await _aggregateRepository.Save(newRace);

        return Created($"/api/race/{newRace.Id}", new { RaceId = newRace.Id });
    }

    [HttpPut("api/race/{raceId}/description")]
    public async Task<IActionResult> SetRaceDescriptionAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] string description)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRaceDescription(description);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/validationpolicy")]
    public async Task<IActionResult> SetRaceValidationPolicyAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] Guid validationPolicyId,
        [FromQuery, BindRequired] long version)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRaceValidationPolicy(validationPolicyId, version);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/validationpolicy/migrate")]
    public async Task<IActionResult> MigrateRaceValidationPolicyAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] Guid validationPolicyId,
        [FromQuery, BindRequired] long version)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.MigrateRaceValidationPolicy(validationPolicyId, version);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpDelete("api/race/{raceId}/validationpolicy")]
    public async Task<IActionResult> RemoveRaceValidationPolicyAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RemoveRaceValidationPolicy();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPost("api/race/{raceId}/date")]
    public async Task<IActionResult> ScheduleRaceDateAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] DateTime start,
        [FromQuery, BindRequired] DateTime end)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ScheduleRaceDate(start, end);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/date", null);
    }

    [HttpPut("api/race/{raceId}/date/{raceDateId}")]
    public async Task<IActionResult> RescheduleRaceDateAsync([FromRoute] Guid raceId,
        [FromRoute] int raceDateId,
        [FromQuery, BindRequired] DateTime start,
        [FromQuery, BindRequired] DateTime end)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RescheduleRaceDate(raceDateId, start, end);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpDelete("api/race/{raceId}/date/{raceDateId}")]
    public async Task<IActionResult> CancelRaceDateAsync([FromRoute] Guid raceId, [FromRoute] int raceDateId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.CancelRaceDate(raceDateId);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/cost")]
    public async Task<IActionResult> SetRaceCostAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] string currency,
        [FromQuery, BindRequired] decimal cost)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRaceCost(currency, cost);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/paymentdeadline")]
    public async Task<IActionResult> ScheduleRacePaymentDeadlineAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] DateTime paymentDeadline)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ScheduleRacePaymentDeadline(paymentDeadline);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/permitunpaidregistration")]
    public async Task<IActionResult> PermitUnpaidRegistrationAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.PermitUnpaidRegistration();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/prohibitunpaidregistration")]
    public async Task<IActionResult> ProhibitUnpaidRegistrationAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ProhibitUnpaidRegistration();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/registrationopendate")]
    public async Task<IActionResult> ScheduleRaceRegistrationOpenDateAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] DateTime registrationOpenDate)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ScheduleRaceRegistrationOpenDate(registrationOpenDate);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/membershipleveldiscount")]
    public async Task<IActionResult> SetRaceClubMembershipLevelDiscountAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] Guid clubId,
        [FromQuery, BindRequired] int membershipLevelId,
        [FromQuery, BindRequired] decimal discount)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRaceClubMembershipLevelDiscount(clubId, membershipLevelId, discount);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPost("api/race/{raceId}/publishdetails")]
    public async Task<IActionResult> PublishRaceDetailsAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.PublishRaceDetails();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPost("api/race/{raceId}/publish")]
    public async Task<IActionResult> PublishRaceAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.PublishRace();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPost("api/race/{raceId}/gonogo")]
    public async Task<IActionResult> ScheduleRaceGoNoGoAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] DateTime goNoGoDate)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ScheduleRaceGoNoGo(goNoGoDate);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/gonogo")]
    public async Task<IActionResult> RescheduleRaceGoNoGoAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] DateTime goNoGoDate)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RescheduleRaceGoNoGo(goNoGoDate);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPost("api/race/{raceId}/cancel")]
    public async Task<IActionResult> CancelRaceAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.CancelRace();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPost("api/race/{raceId}/closeregistration")]
    public async Task<IActionResult> CloseRaceRegistrationAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.CloseRaceRegistration();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/minimumattendees")]
    public async Task<IActionResult> SetRaceMinimumAttendeesAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] int minimumAttendees)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRaceMinimumAttendees(minimumAttendees);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/maximumattendees")]
    public async Task<IActionResult> SetRaceMaximumAttendeesAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] int maximumAttendees)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRaceMaximumAttendees(maximumAttendees);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/permitteamattendance")]
    public async Task<IActionResult> PermitTeamAttendanceAtRaceAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.PermitTeamAttendanceAtRace();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/prohibitteamattendance")]
    public async Task<IActionResult> ProhibitTeamAttendanceAtRaceAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ProhibitTeamAttendanceAtRace();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/permitteamsubstitutions")]
    public async Task<IActionResult> PermitTeamSubstitutionsAtRaceAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.PermitTeamSubstitutionsAtRace();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/prohibitteamsubstitutions")]
    public async Task<IActionResult> ProhibitTeamSubstitutionsAtRaceAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ProhibitTeamSubstitutionsAtRace();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/permitindividualpilotattendance")]
    public async Task<IActionResult> PermitIndividualPilotAttendanceAtRaceAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.PermitIndividualPilotAttendanceAtRace();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/prohibitindividualpilotattendance")]
    public async Task<IActionResult> ProhibitIndividualPilotAttendanceAtRaceAsync([FromRoute] Guid raceId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ProhibitIndividualPilotAttendanceAtRace();

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/teamsize")]
    public async Task<IActionResult> SetTeamRaceTeamSizeAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] int teamSize)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetTeamRaceTeamSize(teamSize);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/minimumteamsize")]
    public async Task<IActionResult> SetTeamRaceMinimumTeamSizeAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] int minimumTeamSize)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetTeamRaceMinimumTeamSize(minimumTeamSize);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/maximumteamsize")]
    public async Task<IActionResult> SetTeamRaceMaximumTeamSizeAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] int maximumTeamSize)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetTeamRaceMaximumTeamSize(maximumTeamSize);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/minimumteams")]
    public async Task<IActionResult> SetTeamRaceMinimumTeamsAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] int minimumTeams)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetTeamRaceMinimumTeams(minimumTeams);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/maximumteams")]
    public async Task<IActionResult> SetTeamRaceMaximumTeamsAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] int maximumTeams)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetTeamRaceMaximumTeams(maximumTeams);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPost("api/race/{raceId}/registration/team")]
    public async Task<IActionResult> RegisterTeamRosterForRaceAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] Guid teamId,
        [FromQuery, BindRequired] int rosterId,
        [FromQuery, BindRequired] Guid registrationId,
        [FromQuery, BindRequired] string currency,
        [FromQuery, BindRequired] double basePrice,
        [FromQuery, BindRequired] double discount)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RegisterTeamRosterForRace(teamId, rosterId, registrationId, currency, basePrice, discount);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/registration/{registrationId}", new { RegistrationId = registrationId });
    }

    [HttpPost("api/race/{raceId}/registration/pilot")]
    public async Task<IActionResult> RegisterIndividualPilotForRaceAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] Guid pilotId,
        [FromQuery, BindRequired] Guid registrationId,
        [FromQuery, BindRequired] string currency,
        [FromQuery, BindRequired] double basePrice,
        [FromQuery, BindRequired] double discount)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RegisterIndividualPilotForRace(pilotId, registrationId, currency, basePrice, discount);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/registration/{registrationId}", new { RegistrationId = registrationId });
    }

    [HttpPut("api/race/{raceId}/registration/{registrationId}/confirm")]
    public async Task<IActionResult> ConfirmRaceRegistrationAsync([FromRoute] Guid raceId,
        [FromRoute] Guid registrationId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ConfirmRaceRegistration(registrationId);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpDelete("api/race/{raceId}/registration/{registrationId}")]
    public async Task<IActionResult> CancelRaceRegistrationAsync([FromRoute] Guid raceId,
        [FromRoute] Guid registrationId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.CancelRaceRegistration(registrationId);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPost("api/race/{raceId}/tag/global")]
    public async Task<IActionResult> TagRaceWithGlobalTagAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] string tag)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.TagRaceWithGlobalTag(tag);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/tag/global/{tag}", new { Tag = tag });
    }

    [HttpPost("api/race/{raceId}/tag/club")]
    public async Task<IActionResult> TagRaceWithClubTagAsync([FromRoute] Guid raceId,
        [FromQuery, BindRequired] Guid clubId,
        [FromQuery, BindRequired] string tag)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.TagRaceWithClubTag(clubId, tag);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/tag/club/{tag}", new { ClubId = clubId, Tag = tag });
    }
}
