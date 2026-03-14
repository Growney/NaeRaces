namespace NaeRaces.Query.Models;

public record HomeLocation(
    string? Name,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Postcode,
    string? County);
