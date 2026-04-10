using NaeRaces.Query.Models;

namespace NaeRaces.Query.Abstractions;

public interface IClubDetailsQueryHandler
{
    Task<bool> DoesClubExist(Guid clubId);
    Task<bool> IsClubFounder(Guid clubId, Guid pilotId);
    Task<ClubContactDetails?> GetClubContactDetails(Guid clubId);
}
