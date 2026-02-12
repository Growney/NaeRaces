using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Pilot;

public class SetPilotNationalityRequest
{
    [Required]
    [MaxLength(50)]
    public string Nationality { get; set; } = string.Empty;
}
