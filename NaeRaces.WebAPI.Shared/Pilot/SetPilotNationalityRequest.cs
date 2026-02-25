using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Pilot;

public class SetPilotNationalityRequest
{
    [Required]
    [MaxLength(50)]
    public string Nationality { get; set; } = string.Empty;
}
