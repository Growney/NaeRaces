using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Club;

public class UpdateClubContactDetailsRequest
{
    [MaxLength(25)]
    public string? PhoneNumber { get; set; }

    [MaxLength(100)]
    public string? EmailAddress { get; set; }
}
