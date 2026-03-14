namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class ClubMemberRole
{
    public Guid ClubId { get; set; }
    public Guid PilotId { get; set; }
    public string Role { get; set; } = string.Empty;
}
