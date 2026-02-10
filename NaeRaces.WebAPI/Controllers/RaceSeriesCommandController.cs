using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NaeRaces.Command.Aggregates;

namespace NaeRaces.WebAPI.Controllers;

public class RaceSeriesCommandController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;

    public RaceSeriesCommandController(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    [HttpPost("api/raceseries")]
    public async Task<IActionResult> PlanRaceSeriesAsync([FromQuery, BindRequired] Guid raceSeriesId,
        [FromQuery, BindRequired] string name)
    {
        RaceSeries newRaceSeries = _aggregateRepository.CreateNew<RaceSeries>(() => new RaceSeries(raceSeriesId, name));

        await _aggregateRepository.Save(newRaceSeries);

        return Created($"/api/raceseries/{newRaceSeries.Id}", new { RaceSeriesId = newRaceSeries.Id });
    }

    [HttpPost("api/raceseries/{raceSeriesId}/race")]
    public async Task<IActionResult> AddRaceToSeriesAsync([FromRoute] Guid raceSeriesId,
        [FromQuery, BindRequired] Guid raceId)
    {
        RaceSeries? raceSeries = await _aggregateRepository.Get<RaceSeries, Guid>(raceSeriesId);
        if (raceSeries == null)
        {
            return NotFound();
        }

        raceSeries.AddRaceToSeries(raceId);

        await _aggregateRepository.Save(raceSeries);

        return Created($"/api/raceseries/{raceSeriesId}/race/{raceId}", new { RaceId = raceId });
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
