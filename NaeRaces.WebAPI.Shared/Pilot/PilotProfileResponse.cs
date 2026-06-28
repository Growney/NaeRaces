namespace NaeRaces.WebAPI.Shared.Pilot;

public class PilotProfileResponse
{
    public Guid PilotId { get; set; }
    public string CallSign { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public List<string> Roles { get; set; } = [];
    public List<PilotProfileGovernmentDocumentValidationResponse> GovernmentDocumentValidations { get; set; } = [];
    public List<PilotProfileInsuranceValidationResponse> InsuranceValidations { get; set; } = [];
    public List<PilotProfileDateOfBirthValidationResponse> DateOfBirthValidations { get; set; } = [];
    public List<PilotProfileClubResponse> Clubs { get; set; } = [];
}
