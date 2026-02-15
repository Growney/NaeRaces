using EventDbLite.Aggregates;
using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Command.Aggregates;

public class Team : AggregateRoot<Guid>
{
    private string? _name;
    private Guid? _captainPilotId;
    private readonly HashSet<Guid> _pilots = [];
    private readonly Dictionary<int, Roster> _rosters = [];

    private class Roster
    {
        public Guid RaceId { get; set; }
        public HashSet<Guid> PilotIds { get; set; } = [];
    }

    public Team()
    {

    }

    public Team(Guid teamId, string name, Guid captainPilotId)
    {
        if (Id != default)
            throw new InvalidOperationException("Team already formed.");

        Raise(new TeamFormed(teamId, name, captainPilotId));
    }

    public void AddPilotToTeam(Guid pilotId)
    {
        ThrowIfIdNotSet();
        if (_pilots.Contains(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is already in the team.");

        Raise(new PilotJoinedTeam(Id, pilotId));
    }

    public void RemovePilotFromTeam(Guid pilotId)
    {
        ThrowIfIdNotSet();
        if (!_pilots.Contains(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is not in the team.");

        Raise(new PilotLeftTeam(Id, pilotId));
    }

    public void PlanTeamRaceRoster(int rosterId, Guid raceId)
    {
        ThrowIfIdNotSet();
        if (_rosters.ContainsKey(rosterId))
            throw new InvalidOperationException($"Roster {rosterId} already exists.");

        Raise(new TeamRaceRosterPlanned(Id, rosterId, raceId));
    }

    public void AddPilotToTeamRoster(int rosterId, Guid pilotId)
    {
        ThrowIfIdNotSet();
        if (!_rosters.ContainsKey(rosterId))
            throw new InvalidOperationException($"Roster {rosterId} does not exist.");
        if (!_pilots.Contains(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is not in the team.");
        if (_rosters[rosterId].PilotIds.Contains(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is already in roster {rosterId}.");

        Raise(new PilotJoinedTeamRoster(Id, rosterId, pilotId));
    }

    public void SubstituteRosterPilot(int rosterId, Guid originalPilotId, Guid substitutePilotId)
    {
        ThrowIfIdNotSet();
        if (!_rosters.ContainsKey(rosterId))
            throw new InvalidOperationException($"Roster {rosterId} does not exist.");
        if (!_rosters[rosterId].PilotIds.Contains(originalPilotId))
            throw new InvalidOperationException($"Pilot {originalPilotId} is not in roster {rosterId}.");
        if (!_pilots.Contains(substitutePilotId))
            throw new InvalidOperationException($"Substitute pilot {substitutePilotId} is not in the team.");

        Raise(new RosterPilotSubstituted(Id, rosterId, originalPilotId, substitutePilotId));
    }

    public void RemovePilotFromRoster(int rosterId, Guid pilotId)
    {
        ThrowIfIdNotSet();
        if (!_rosters.ContainsKey(rosterId))
            throw new InvalidOperationException($"Roster {rosterId} does not exist.");
        if (!_rosters[rosterId].PilotIds.Contains(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is not in roster {rosterId}.");

        Raise(new TeamPilotLeftRoster(Id, rosterId, pilotId));
    }

    // Event handlers
    private void When(TeamFormed e)
    {
        Id = e.TeamId;
        _name = e.Name;
        _captainPilotId = e.CaptainPilotId;
        _pilots.Add(e.CaptainPilotId);
    }

    private void When(PilotJoinedTeam e)
    {
        _pilots.Add(e.PilotId);
    }

    private void When(PilotLeftTeam e)
    {
        _pilots.Remove(e.PilotId);
    }

    private void When(TeamRaceRosterPlanned e)
    {
        _rosters[e.RosterId] = new Roster
        {
            RaceId = e.RaceId
        };
    }

    private void When(PilotJoinedTeamRoster e)
    {
        _rosters[e.RosterId].PilotIds.Add(e.PilotId);
    }

    private void When(RosterPilotSubstituted e)
    {
        _rosters[e.RosterId].PilotIds.Remove(e.OriginalPilotId);
        _rosters[e.RosterId].PilotIds.Add(e.SubstitutePilotId);
    }

    private void When(TeamPilotLeftRoster e)
    {
        _rosters[e.RosterId].PilotIds.Remove(e.PilotId);
    }
}
