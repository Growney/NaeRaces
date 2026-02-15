using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface ITeamMemberQueryHandler
{
    IAsyncEnumerable<Guid> GetTeamMemberPilotIds(Guid teamId);
    IAsyncEnumerable<Guid> GetPilotTeamIds(Guid pilotId);
    Task<bool> IsPilotMemberOfTeam(Guid teamId, Guid pilotId);
}
