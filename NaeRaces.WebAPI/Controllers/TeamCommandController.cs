using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;

namespace NaeRaces.WebAPI.Controllers;

public class TeamCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;

    public TeamCommandController(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    [HttpPost("api/team")]
    public async Task<IActionResult> FormTeamAsync([FromQuery, BindRequired] Guid teamId,
        [FromQuery, BindRequired] string name,
        [FromQuery, BindRequired] Guid captainPilotId)
    {
        Team newTeam = _aggregateRepository.CreateNew<Team>(() => new Team(teamId, name, captainPilotId));

        await _aggregateRepository.Save(newTeam);

        return Created($"/api/team/{newTeam.Id}", new { TeamId = newTeam.Id });
    }

    [HttpPost("api/team/{teamId}/pilot")]
    public async Task<IActionResult> AddPilotToTeamAsync([FromRoute] Guid teamId,
        [FromQuery, BindRequired] Guid pilotId)
    {
        Team? team = await _aggregateRepository.Get<Team, Guid>(teamId);
        if (team == null)
        {
            return NotFound();
        }

        team.AddPilotToTeam(pilotId);

        await _aggregateRepository.Save(team);

        return Created($"/api/team/{teamId}/pilot/{pilotId}", new { PilotId = pilotId });
    }

    [HttpDelete("api/team/{teamId}/pilot/{pilotId}")]
    public async Task<IActionResult> RemovePilotFromTeamAsync([FromRoute] Guid teamId,
        [FromRoute] Guid pilotId)
    {
        Team? team = await _aggregateRepository.Get<Team, Guid>(teamId);
        if (team == null)
        {
            return NotFound();
        }

        team.RemovePilotFromTeam(pilotId);

        await _aggregateRepository.Save(team);

        return Ok();
    }

    [HttpPost("api/team/{teamId}/roster")]
    public async Task<IActionResult> PlanTeamRaceRosterAsync([FromRoute] Guid teamId,
        [FromQuery, BindRequired] int rosterId,
        [FromQuery, BindRequired] Guid raceId)
    {
        Team? team = await _aggregateRepository.Get<Team, Guid>(teamId);
        if (team == null)
        {
            return NotFound();
        }

        team.PlanTeamRaceRoster(rosterId, raceId);

        await _aggregateRepository.Save(team);

        return Created($"/api/team/{teamId}/roster/{rosterId}", new { RosterId = rosterId });
    }

    [HttpPost("api/team/{teamId}/roster/{rosterId}/pilot")]
    public async Task<IActionResult> AddPilotToTeamRosterAsync([FromRoute] Guid teamId,
        [FromRoute] int rosterId,
        [FromQuery, BindRequired] Guid pilotId)
    {
        Team? team = await _aggregateRepository.Get<Team, Guid>(teamId);
        if (team == null)
        {
            return NotFound();
        }

        team.AddPilotToTeamRoster(rosterId, pilotId);

        await _aggregateRepository.Save(team);

        return Created($"/api/team/{teamId}/roster/{rosterId}/pilot/{pilotId}", new { PilotId = pilotId });
    }

    [HttpPut("api/team/{teamId}/roster/{rosterId}/substitute")]
    public async Task<IActionResult> SubstituteRosterPilotAsync([FromRoute] Guid teamId,
        [FromRoute] int rosterId,
        [FromQuery, BindRequired] Guid raceId,
        [FromQuery, BindRequired] Guid originalPilotId,
        [FromQuery, BindRequired] Guid substitutePilotId)
    {
        Team? team = await _aggregateRepository.Get<Team, Guid>(teamId);
        if (team == null)
        {
            return NotFound();
        }

        team.SubstituteRosterPilot(rosterId, raceId, originalPilotId, substitutePilotId);

        await _aggregateRepository.Save(team);

        return Ok();
    }

    [HttpDelete("api/team/{teamId}/roster/{rosterId}/pilot/{pilotId}")]
    public async Task<IActionResult> RemovePilotFromRosterAsync([FromRoute] Guid teamId,
        [FromRoute] int rosterId,
        [FromRoute] Guid pilotId)
    {
        Team? team = await _aggregateRepository.Get<Team, Guid>(teamId);
        if (team == null)
        {
            return NotFound();
        }

        team.RemovePilotFromRoster(rosterId, pilotId);

        await _aggregateRepository.Save(team);

        return Ok();
    }
}
