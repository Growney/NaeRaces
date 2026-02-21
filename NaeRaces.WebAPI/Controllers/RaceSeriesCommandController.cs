using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;
using NaeRaces.Query.Abstractions;
using NaeRaces.WebAPI.Models.RaceSeries;

namespace NaeRaces.WebAPI.Controllers;

public class RaceSeriesCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;
    private readonly INaeRacesQueryContext _queryContext;

    public RaceSeriesCommandController(IAggregateRepository aggregateRepository, INaeRacesQueryContext queryContext)
    {
        _aggregateRepository = aggregateRepository;
        _queryContext = queryContext;
    }

    [HttpPost("api/raceseries")]
    public async Task<IActionResult> PlanRaceSeriesAsync([FromBody] PlanRaceSeriesRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(request.RaceSeriesId == Guid.Empty)
        {
            return BadRequest("RaceSeriesId cannot be empty.");
        }

        RaceSeries newRaceSeries = _aggregateRepository.CreateNew<RaceSeries>(() => new RaceSeries(request.RaceSeriesId, request.Name));

        await _aggregateRepository.Save(newRaceSeries);

        return Created($"/api/raceseries/{newRaceSeries.Id}", new { RaceSeriesId = newRaceSeries.Id });
    }

    [HttpPost("api/raceseries/{raceSeriesId}/race")]
    public async Task<IActionResult> AddRaceToSeriesAsync([FromRoute] Guid raceSeriesId,
        [FromBody] AddRaceToSeriesRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _queryContext.RaceDetails.DoesRaceExist(request.RaceId))
        {
            return BadRequest("Race not found");
        }

        RaceSeries? raceSeries = await _aggregateRepository.Get<RaceSeries, Guid>(raceSeriesId);
        if (raceSeries == null)
        {
            return NotFound();
        }

        raceSeries.AddRaceToSeries(request.RaceId);

        await _aggregateRepository.Save(raceSeries);

        return Created($"/api/raceseries/{raceSeriesId}/race/{request.RaceId}", new { RaceId = request.RaceId });
    }

    [HttpDelete("api/raceseries/{raceSeriesId}/race/{raceId}")]
    public async Task<IActionResult> RemoveRaceFromSeriesAsync([FromRoute] Guid raceSeriesId,
        [FromRoute] Guid raceId)
    {
        RaceSeries? raceSeries = await _aggregateRepository.Get<RaceSeries, Guid>(raceSeriesId);
        if (raceSeries == null)
        {
            return NotFound();
        }

        raceSeries.RemoveRaceFromSeries(raceId);

        await _aggregateRepository.Save(raceSeries);

        return Ok();
    }
}
