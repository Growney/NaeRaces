namespace NaeRaces.WebAPI.Shared.Club;

public class CurrentUserClubMembershipResponse
{
    public Guid RegistrationId { get; set; }
    public int MembershipLevelId { get; set; }
    public int PaymentOptionId { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTime? ValidUntil { get; set; }
}
