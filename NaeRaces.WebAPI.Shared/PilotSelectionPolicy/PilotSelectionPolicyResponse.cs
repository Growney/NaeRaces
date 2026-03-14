namespace NaeRaces.WebAPI.Shared.PilotSelectionPolicy;

public class PilotSelectionPolicyResponse
{
    public Guid PolicyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Version { get; set; }
    public int? RootStatementId { get; set; }
}
