using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models;

public class ChangeClubLocationAddressRequest
{
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
