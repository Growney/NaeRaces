using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Models.Race;

public class SetUnconfirmedSlotConsumptionPolicyRequest
{
    [Required]
    public bool IsAllowed { get; set; }
}
