using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IClubMemberQueryHandler
{
    Task<bool> HasEverBeenClubMember(Guid clubId, Guid pilotId);
    Task<bool> IsCurrentlyActiveClubMember(Guid clubId, Guid pilotId);
    Task<bool> HasClubMemberRole(Guid clubId, Guid pilotId,params IEnumerable<string> role);
    IAsyncEnumerable<ClubMember> GetPilotMembershipDetails(Guid pilotId);
    IAsyncEnumerable<ClubMember> GetClubMembers(Guid clubId);
    IAsyncEnumerable<ClubMemberRole> GetClubMemberRoles(Guid clubId, Guid pilotId);
}
