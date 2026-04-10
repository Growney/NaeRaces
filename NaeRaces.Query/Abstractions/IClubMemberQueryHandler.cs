using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IClubMemberQueryHandler
{
    Task<bool> HasEverBeenClubMember(Guid clubId, Guid pilotId);
    Task<bool> IsCurrentlyActiveClubMember(Guid clubId, Guid pilotId);
    Task<bool> HasActiveOrPendingMembership(Guid clubId, Guid pilotId);
    Task<bool> HasClubMemberRole(Guid clubId, Guid pilotId,params IEnumerable<string> role);
    IAsyncEnumerable<ClubMember> GetPilotMembershipDetails(Guid pilotId);
    IAsyncEnumerable<ClubMember> GetClubMembers(Guid clubId);
    IAsyncEnumerable<ClubMemberRole> GetClubMemberRoles(Guid clubId, Guid pilotId);
    IAsyncEnumerable<Guid> GetClubIdsWithRoles(Guid pilotId, params IEnumerable<string> roles);
    IAsyncEnumerable<ClubMember> GetMembershipsExpiringBefore(DateTime expiryDate);
}
