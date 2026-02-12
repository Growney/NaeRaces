namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class RacePolicyDetails
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long LatestVersion { get; set; }
}
