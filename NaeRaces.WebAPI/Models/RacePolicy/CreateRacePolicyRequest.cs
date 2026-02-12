using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.RacePolicy;

public class CreateRacePolicyRequest
{
    [Required]
    public Guid RacePolicyId { get; set; }

    [Required]
    public Guid ClubId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}
