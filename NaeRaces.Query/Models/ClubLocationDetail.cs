namespace NaeRaces.Query.Models;

public record ClubLocationDetail(
    int LocationId,
    string Name,
    string Information,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string Postcode,
    string County,
    bool IsHomeLocation);
