using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models;

public class ChangeClubLocationInformationRequest
{
    [Required]
    [MaxLength(250)]
    public string LocationInformation { get; set; } = string.Empty;
}
