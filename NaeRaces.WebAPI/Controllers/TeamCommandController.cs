using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.WebAPI.Shared.Team;

namespace NaeRaces.WebAPI.Controllers;

public class TeamCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;

    public TeamCommandController(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    [HttpPost("api/team")]
    public async Task<IActionResult> FormTeamAsync([FromBody] FormTeamRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(request.TeamId == Guid.Empty)
        {
            return BadRequest("TeamId cannot be empty.");
        }

        Team newTeam = _aggregateRepository.CreateNew<Team>(() => new Team(request.TeamId, request.Name, request.CaptainPilotId));

        await _aggregateRepository.Save(newTeam);

        return Created($"/api/team/{newTeam.Id}", new { TeamId = newTeam.Id });
    }

    [HttpPost("api/team/{teamId}/pilot")]
    public async Task<IActionResult> AddPilotToTeamAsync([FromRoute] Guid teamId,
        [FromBody] AddPilotToTeamRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Team? team = await _aggregateRepository.Get<Team, Guid>(teamId);
        if (team == null)
        {
            return NotFound();
        }

        team.AddPilotToTeam(request.PilotId);

        await _aggregateRepository.Save(team);

        return Created($"/api/team/{teamId}/pilot/{request.PilotId}", new { PilotId = request.PilotId });
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
        [FromBody] PlanTeamRaceRosterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Team? team = await _aggregateRepository.Get<Team, Guid>(teamId);
        if (team == null)
        {
            return NotFound();
        }

        team.PlanTeamRaceRoster(request.RosterId, request.RaceId);

        await _aggregateRepository.Save(team);

        return Created($"/api/team/{teamId}/roster/{request.RosterId}", new { RosterId = request.RosterId });
    }

    [HttpPost("api/team/{teamId}/roster/{rosterId}/pilot")]
    public async Task<IActionResult> AddPilotToTeamRosterAsync([FromRoute] Guid teamId,
        [FromRoute] int rosterId,
        [FromBody] AddPilotToTeamRosterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Team? team = await _aggregateRepository.Get<Team, Guid>(teamId);
        if (team == null)
        {
            return NotFound();
        }

        team.AddPilotToTeamRoster(rosterId, request.PilotId);

        await _aggregateRepository.Save(team);

        return Created($"/api/team/{teamId}/roster/{rosterId}/pilot/{request.PilotId}", new { PilotId = request.PilotId });
    }

    [HttpPut("api/team/{teamId}/roster/{rosterId}/substitute")]
    public async Task<IActionResult> SubstituteRosterPilotAsync([FromRoute] Guid teamId,
        [FromRoute] int rosterId,
        [FromBody] SubstituteRosterPilotRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Team? team = await _aggregateRepository.Get<Team, Guid>(teamId);
        if (team == null)
        {
            return NotFound();
        }

        team.SubstituteRosterPilot(rosterId, request.OriginalPilotId, request.SubstitutePilotId);

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
