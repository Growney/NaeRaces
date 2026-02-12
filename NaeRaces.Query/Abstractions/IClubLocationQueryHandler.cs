namespace NaeRaces.Query.Abstractions;

public interface IClubLocationQueryHandler
{
    Task<bool> DoesLocationExist(Guid clubId, int locationId);
    Task<bool> IsLocationInUse(Guid clubId, int locationId);
}
