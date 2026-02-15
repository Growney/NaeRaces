using System;

namespace NaeRaces.Query.Models;

public class PilotRegistrationDetails
{
    public Guid PilotId { get; set; }
    public Guid RaceId { get; set; }
    public bool MeetsValidation { get; set; }
    public string? ValidationError { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal BaseCost { get; set; }
    public decimal? BestDiscountAmount { get; set; }
    public Guid? BestDiscountPolicyId { get; set; }
    public long? BestDiscountPolicyVersion { get; set; }
    public decimal FinalCost { get; set; }
}
