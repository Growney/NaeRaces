using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Abstractions;

public interface IClubMembershipLevelQueryHandler
{
    Task<ClubMembershipLevel?> GetClubMembershipLevel(Guid clubId, int membershipLevelId);
    IAsyncEnumerable<ClubMembershipLevel> GetClubMembershipLevels(Guid clubId);
}
