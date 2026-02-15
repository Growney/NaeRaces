using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Events;

public record TeamFormed(Guid TeamId, string Name, Guid CaptainPilotId);
public record PilotJoinedTeam(Guid TeamId, Guid PilotId);
public record PilotLeftTeam(Guid TeamId, Guid PilotId);

