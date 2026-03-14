namespace NaeRaces.WebAPI.Shared.Club;

public class ClubLocationResponse
{
    public int LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Information { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public bool IsHomeLocation { get; set; }
}
