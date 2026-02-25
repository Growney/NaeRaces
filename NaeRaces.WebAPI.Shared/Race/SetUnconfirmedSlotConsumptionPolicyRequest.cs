using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class SetUnconfirmedSlotConsumptionPolicyRequest
{
    [Required]
    public bool IsAllowed { get; set; }
}
