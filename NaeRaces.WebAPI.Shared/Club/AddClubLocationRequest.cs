using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class AddClubLocationRequest
{
    [Required]
    public string LocationName { get; set; } = string.Empty;

    [Required]
    public string LocationInformation { get; set; } = string.Empty;

    [Required]
    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Postcode { get; set; } = string.Empty;

    [Required]
    public string County { get; set; } = string.Empty;
}
