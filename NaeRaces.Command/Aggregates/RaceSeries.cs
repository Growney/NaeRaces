using EventDbLite.Aggregates;
using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Command.Aggregates;

public class RaceSeries : AggregateRoot<Guid>
{
    private string? _name;
    private readonly HashSet<Guid> _raceIds = [];

    public RaceSeries(Guid raceSeriesId, string name)
    {
        if (Id != default)
            throw new InvalidOperationException("Race series already planned.");

        Raise(new RaceSeriesPlanned(raceSeriesId, name));
    }

    public void AddRaceToSeries(Guid raceId)
    {
        ThrowIfIdNotSet();
        if (_raceIds.Contains(raceId))
            throw new InvalidOperationException($"Race {raceId} is already in the series.");

        Raise(new RaceAddedToSeries(Id, raceId));
    }

    public void RemoveRaceFromSeries(Guid raceId)
    {
        ThrowIfIdNotSet();
        if (!_raceIds.Contains(raceId))
            throw new InvalidOperationException($"Race {raceId} is not in the series.");

        Raise(new RaceRemovedFromSeries(Id, raceId));
    }

    // Event handlers
    private void When(RaceSeriesPlanned e)
    {
        Id = e.RaceSeriesId;
        _name = e.Name;
    }

    private void When(RaceAddedToSeries e)
    {
        _raceIds.Add(e.RaceId);
    }

    private void When(RaceRemovedFromSeries e)
    {
        _raceIds.Remove(e.RaceId);
    }
}
