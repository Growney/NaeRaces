using Microsoft.EntityFrameworkCore;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

[PrimaryKey(nameof(ClubId), nameof(LocationId))]
public class ClubLocation
{
    public Guid ClubId { get; set; }
    public int LocationId { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? LocationInformation { get; set; } = string.Empty;
    public string? AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string? City { get; set; } = string.Empty;
    public string? Postcode { get; set; } = string.Empty;
    public string? County { get; set; } = string.Empty;
    public int? TimezoneOffsetMinutes { get; set; }
    public bool? UseDaylightSavings { get; set; }
    public bool IsHomeLocation { get; set; }
}
