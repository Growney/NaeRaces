namespace NaeRaces.Command.ValueTypes;

public record ClubMemberRole
{
    private enum ValidRoles
    {
        Administrator,
        RaceOrganiser,
        Trustee
    }

    private static readonly string[] _validRoles = Enum.GetNames<ValidRoles>();

    public static readonly ClubMemberRole Administrator = new(_validRoles[(int)ValidRoles.Administrator]);
    public static readonly ClubMemberRole RaceOrganiser = new(_validRoles[(int)ValidRoles.RaceOrganiser]);
    public static readonly ClubMemberRole Trustee = new(_validRoles[(int)ValidRoles.Trustee]);

    private readonly string _roleName;

    private ClubMemberRole(string roleName)
    {
        _roleName = roleName;
    }

    public static ClubMemberRole Create(string role)
    {
        if (!_validRoles.Contains(role))
        {
            throw new ArgumentException("Invalid role");
        }

        return new ClubMemberRole(role);
    }

    public static ClubMemberRole Rehydrate(string role) => new ClubMemberRole(role);

    public static implicit operator string(ClubMemberRole role) => role._roleName;
}
