namespace NaeRaces.WebAPI.Shared.Club;

public class TopClubByMemberCountResponse
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MemberCount { get; set; }
}
