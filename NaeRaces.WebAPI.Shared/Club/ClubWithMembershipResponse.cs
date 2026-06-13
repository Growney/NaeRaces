using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.WebAPI.Shared.Club;

public class ClubWithMembershipResponse
{
    public Guid ClubId { get; set; }
    public string ClubCode { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;
    public string? MembershipLevelName { get; set; }
    public DateTime? MembershipExpiry { get; set; }
    public bool? IsMembershipConfirmed { get; set; }
    public bool IsFollowing { get; set; }
    public int Members { get; set; }
    public int Followers { get; set; }
}
