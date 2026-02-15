using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Events;

public record TeamFormed(Guid TeamId, string Name, Guid CaptainPilotId);
public record PilotJoinedTeam(Guid TeamId, Guid PilotId);
public record PilotLeftTeam(Guid TeamId, Guid PilotId);

public record TeamRaceRosterPlanned(Guid TeamId, int RosterId, Guid RaceId);
public record PilotJoinedTeamRoster(Guid TeamId, int RosterId, Guid PilotId);
public record RosterPilotSubstituted(Guid TeamId, int RosterId, Guid OriginalPilotId, Guid SubstitutePilotId);
public record TeamPilotLeftRoster(Guid TeamId, int RosterId, Guid PilotId);


