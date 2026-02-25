using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using NaeRaces.WebAPI.Models.Race;
using static NaeRaces.Query.Abstractions.IPilotRegistrationQueryHandler;
using static NaeRaces.WebAPI.Models.Race.RegisterTeamRosterForRaceRequest;

namespace NaeRaces.WebAPI.Controllers;

public class RaceCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;
    private readonly INaeRacesQueryContext _queryContext;

    public RaceCommandController(IAggregateRepository aggregateRepository, INaeRacesQueryContext queryContext)
    {
        _aggregateRepository = aggregateRepository;
        _queryContext = queryContext;
    }

    [HttpPost("api/race")]
    public async Task<IActionResult> PlanRaceAsync([FromBody] PlanRaceRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (request.RaceId == Guid.Empty)
        {
            return BadRequest("RaceId must be a non-empty GUID.");
        }

        if (!await _queryContext.ClubDetails.DoesClubExist(request.ClubId))
        {
            return BadRequest($"Club with ID {request.ClubId} does not exist.");
        }

        if (!await _queryContext.ClubLocation.DoesLocationExist(request.ClubId, request.LocationId))
        {
            return BadRequest($"Location with ID {request.LocationId} does not exist.");
        }

        Name nameValueType = Name.Create(request.Name);

        Race newRace = _aggregateRepository.CreateNew<Race>(() => new Race(request.RaceId, nameValueType, request.ClubId, request.LocationId));

        await _aggregateRepository.Save(newRace);

        return Created($"/api/race/{newRace.Id}", new { RaceId = newRace.Id });
    }

    [HttpPost("api/race/team")]
    public async Task<IActionResult> PlanTeamRaceAsync([FromBody] PlanTeamRaceRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (request.RaceId == Guid.Empty)
        {
            return BadRequest("RaceId must be a non-empty GUID.");
        }

        if (!await _queryContext.ClubDetails.DoesClubExist(request.ClubId))
        {
            return BadRequest($"Club with ID {request.ClubId} does not exist.");
        }

        if (!await _queryContext.ClubLocation.DoesLocationExist(request.ClubId, request.LocationId))
        {
            return BadRequest($"Location with ID {request.LocationId} does not exist.");
        }

        Name nameValueType = Name.Create(request.Name);

        Race newRace = _aggregateRepository.CreateNew<Race>(() => new Race(request.RaceId, nameValueType, request.TeamSize, request.ClubId, request.LocationId));

        await _aggregateRepository.Save(newRace);

        return Created($"/api/race/{newRace.Id}", new { RaceId = newRace.Id });
    }

    [HttpPut("api/race/{raceId}/description")]
    public async Task<IActionResult> SetRaceDescriptionAsync([FromRoute] Guid raceId,
        [FromBody] SetRaceDescriptionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRaceDescription(request.Description);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/validationpolicy")]
    public async Task<IActionResult> SetRaceValidationPolicyAsync([FromRoute] Guid raceId,
        [FromBody] SetRaceValidationPolicyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        if (race.ClubId == null)
        {
            return BadRequest("Race club not set. Cannot set validation policy without club context.");
        }

        PilotSelectionPolicyDetails? policyDetails = await _queryContext.PilotSelectionPolicy.GetPolicyDetails(request.ValidationPolicyId, race.ClubId.Value);

        if (policyDetails == null)
        {
            return BadRequest($"Pilot selection policy with ID {request.ValidationPolicyId} does not exist.");
        }

        race.SetRaceValidationPolicy(request.ValidationPolicyId, policyDetails.LatestVersion);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/validationpolicy/migrate")]
    public async Task<IActionResult> MigrateRaceValidationPolicyAsync([FromRoute] Guid raceId,
        [FromBody] MigrateRaceValidationPolicyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.MigrateRaceValidationPolicy(request.ValidationPolicyId, request.Version);

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
        [FromBody] ScheduleRaceDateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ScheduleRaceDate(request.Start, request.End);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/date", null);
    }

    [HttpPut("api/race/{raceId}/date/{raceDateId}")]
    public async Task<IActionResult> RescheduleRaceDateAsync([FromRoute] Guid raceId,
        [FromRoute] int raceDateId,
        [FromBody] ScheduleRaceDateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RescheduleRaceDate(raceDateId, request.Start, request.End);

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

    [HttpPost("api/race/{raceId}/package")]
    public async Task<IActionResult> AddRacePackageAsync([FromRoute] Guid raceId,
        [FromBody] AddRacePackageRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        int packageId = race.AddRacePackage(Name.Create(request.Name), request.Currency, request.Cost);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/package/{packageId}", new { PackageId = packageId });
    }

    [HttpPut("api/race/{raceId}/package/{packageId}/discountstatus")]
    public async Task<IActionResult> SetRacePackageDiscountStatusAsync([FromRoute] Guid raceId,
        [FromRoute] int packageId,
        [FromBody] SetRacePackageDiscountStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRacePackageDiscountStatus(packageId, request.ApplyDiscounts);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/paymentdeadline")]
    public async Task<IActionResult> ScheduleRacePaymentDeadlineAsync([FromRoute] Guid raceId,
        [FromBody] ScheduleDateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ScheduleRacePaymentDeadline(request.Date);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/unconfirmedslotconsumption")]
    public async Task<IActionResult> SetUnconfirmedSlotConsumptionPolicyAsync([FromRoute] Guid raceId,
        [FromBody] SetUnconfirmedSlotConsumptionPolicyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetUnconfirmedSlotConsumptionPolicy(request.IsAllowed);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/package/{packageId}/registrationopendate")]
    public async Task<IActionResult> ScheduleRacePackageRegistrationOpenAsync([FromRoute] Guid raceId,
        [FromRoute] int packageId,
        [FromBody] ScheduleDateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ScheduleRacePackageRegistrationOpen(packageId, request.Date);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/package/{packageId}/registrationclosedate")]
    public async Task<IActionResult> ScheduleRacePackageRegistrationCloseAsync([FromRoute] Guid raceId,
        [FromRoute] int packageId,
        [FromBody] ScheduleDateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ScheduleRacePackageRegistrationClose(packageId, request.Date);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/package/{packageId}/registrationstatus")]
    public async Task<IActionResult> SetRacePackageRegistrationStatusAsync([FromRoute] Guid raceId,
        [FromRoute] int packageId,
        [FromBody] SetRacePackageRegistrationStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRacePackageRegistrationStatus(packageId, request.IsOpen);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/package/{packageId}/pilotpolicy")]
    public async Task<IActionResult> SetRacePackagePilotPolicyAsync([FromRoute] Guid raceId,
        [FromRoute] int packageId,
        [FromBody] SetEarlyRegistrationPolicyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        if (race.ClubId == null)
        {
            return BadRequest("Race club not set. Cannot set package pilot policy without club context.");
        }

        PilotSelectionPolicyDetails? policyDetails = await _queryContext.PilotSelectionPolicy.GetPolicyDetails(request.PilotPolicyId, race.ClubId.Value);

        if (policyDetails == null)
        {
            return BadRequest($"Pilot selection policy with ID {request.PilotPolicyId} does not exist.");
        }

        race.SetRacePackagePilotPolicy(packageId, request.PilotPolicyId, policyDetails.LatestVersion);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpDelete("api/race/{raceId}/package/{packageId}")]
    public async Task<IActionResult> RemoveRacePackageAsync([FromRoute] Guid raceId,
        [FromRoute] int packageId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RemoveRacePackage(packageId);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPost("api/race/{raceId}/discount")]
    public async Task<IActionResult> AddRaceDiscountAsync([FromRoute] Guid raceId,
        [FromBody] AddRaceDiscountRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Name name = Name.Create(request.Name);

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        if (race.ClubId == null)
        {
            return BadRequest("Race club not set. Cannot add discount without club context.");
        }

        PilotSelectionPolicyDetails? policyDetails = await _queryContext.PilotSelectionPolicy.GetPolicyDetails(request.PilotPolicyId, race.ClubId.Value);

        if (policyDetails == null)
        {
            return BadRequest($"Pilot selection policy with ID {request.PilotPolicyId} does not exist.");
        }

        int discountId = race.AddRaceDiscount(name, request.PilotPolicyId, policyDetails.LatestVersion, request.Currency, request.Discount, request.IsPercentage, request.CanBeCombined);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/discount/{discountId}", new { DiscountId = discountId });
    }

    [HttpDelete("api/race/{raceId}/discount/{discountId}")]
    public async Task<IActionResult> RemoveRaceDiscountAsync([FromRoute] Guid raceId,
        [FromRoute] int discountId)
    {
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RemoveRaceDiscount(discountId);

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
        [FromBody] ScheduleDateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.ScheduleRaceGoNoGo(request.Date);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/gonogo")]
    public async Task<IActionResult> RescheduleRaceGoNoGoAsync([FromRoute] Guid raceId,
        [FromBody] ScheduleDateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RescheduleRaceGoNoGo(request.Date);

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
        [FromBody] SetAttendeesRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRaceMinimumAttendees(request.Attendees);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/maximumattendees")]
    public async Task<IActionResult> SetRaceMaximumAttendeesAsync([FromRoute] Guid raceId,
        [FromBody] SetAttendeesRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetRaceMaximumAttendees(request.Attendees);

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
        [FromBody] SetTeamSizeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetTeamRaceTeamSize(request.TeamSize);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/minimumteamsize")]
    public async Task<IActionResult> SetTeamRaceMinimumTeamSizeAsync([FromRoute] Guid raceId,
        [FromBody] SetTeamSizeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetTeamRaceMinimumTeamSize(request.TeamSize);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/maximumteamsize")]
    public async Task<IActionResult> SetTeamRaceMaximumTeamSizeAsync([FromRoute] Guid raceId,
        [FromBody] SetTeamSizeRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetTeamRaceMaximumTeamSize(request.TeamSize);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/minimumteams")]
    public async Task<IActionResult> SetTeamRaceMinimumTeamsAsync([FromRoute] Guid raceId,
        [FromBody] SetTeamsRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetTeamRaceMinimumTeams(request.Teams);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPut("api/race/{raceId}/maximumteams")]
    public async Task<IActionResult> SetTeamRaceMaximumTeamsAsync([FromRoute] Guid raceId,
        [FromBody] SetTeamsRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.SetTeamRaceMaximumTeams(request.Teams);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpPost("api/race/{raceId}/registration/team")]
    public async Task<IActionResult> RegisterTeamRosterForRaceAsync([FromRoute] Guid raceId,
        [FromBody] RegisterTeamRosterForRaceRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        foreach (Guid pilotId in request.Pilots.Select(x=>x.PilotId))
        {
            if (!await _queryContext.TeamMember.IsPilotMemberOfTeam(request.TeamId, pilotId))
            {
                return BadRequest($"Pilot with ID {pilotId} is not a member of team with ID {request.TeamId}.");
            }
        }

        var currentDate = DateTime.UtcNow;

        foreach (TeamPilot teamPilot in request.Pilots)
        {
            var registrationStatus = await _queryContext.PilotRegistration.GetPilotPotentialRegistrationStatusForRace(teamPilot.PilotId, raceId, teamPilot.RacePackageId);

            if (registrationStatus != RegistrationStatus.Success)
            {
                return BadRequest($"Pilot with ID {teamPilot} cannot register for race: {registrationStatus}");
            }
        }

        Guid registrationId = Guid.NewGuid();

        race.RegisterTeamRosterForRace(request.TeamId, request.Pilots.Select(x=> TeamMemberRegistration.Create(x.PilotId, registrationId,x.RacePackageId)));

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/registration/{registrationId}", new { RegistrationId = registrationId });
    }

    [HttpPost("api/race/{raceId}/registration/pilot")]
    public async Task<IActionResult> RegisterIndividualPilotForRaceAsync([FromRoute] Guid raceId,
        [FromBody] RegisterIndividualPilotForRaceRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var registrationStatus = await _queryContext.PilotRegistration.GetPilotPotentialRegistrationStatusForRace(request.PilotId, raceId, request.RacePackageId);

        if (registrationStatus != RegistrationStatus.Success)
        {
            return BadRequest($"Pilot cannot register for race: {registrationStatus}");
        }
        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RegisterIndividualPilotForRace(request.PilotId, request.RegistrationId, request.RacePackageId);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/registration/{request.RegistrationId}", new { RegistrationId = request.RegistrationId });
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
        [FromBody] TagRaceRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Tag tagValueType = Tag.Create(request.Tag);

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.TagRaceWithGlobalTag(tagValueType);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/tag/global/{request.Tag}", new { Tag = request.Tag });
    }

    [HttpPost("api/race/{raceId}/tag/club")]
    public async Task<IActionResult> TagRaceWithClubTagAsync([FromRoute] Guid raceId,
        [FromBody] TagRaceWithClubTagRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Tag tagValueType = Tag.Create(request.Tag);

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.TagRaceWithClubTag(request.ClubId, tagValueType);

        await _aggregateRepository.Save(race);

        return Created($"/api/race/{raceId}/tag/club/{request.Tag}", new { ClubId = request.ClubId, Tag = request.Tag });
    }

    [HttpDelete("api/race/{raceId}/tag/global/{tag}")]
    public async Task<IActionResult> RemoveGlobalTagFromRaceAsync([FromRoute] Guid raceId,
        [FromRoute] string tag)
    {
        Tag tagValueType = Tag.Create(tag);

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RemoveGlobalTagFromRace(tagValueType);

        await _aggregateRepository.Save(race);

        return Ok();
    }

    [HttpDelete("api/race/{raceId}/tag/club/{clubId}/{tag}")]
    public async Task<IActionResult> RemoveClubTagFromRaceAsync([FromRoute] Guid raceId,
        [FromRoute] Guid clubId,
        [FromRoute] string tag)
    {
        Tag tagValueType = Tag.Create(tag);

        Race? race = await _aggregateRepository.Get<Race, Guid>(raceId);
        if (race == null)
        {
            return NotFound();
        }

        race.RemoveClubTagFromRace(clubId, tagValueType);

        await _aggregateRepository.Save(race);

        return Ok();
    }
}
