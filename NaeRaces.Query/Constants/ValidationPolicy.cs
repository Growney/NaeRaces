using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Constants;

public sealed class ValidationPolicy
{
    public const string None = "NONE";
    public const string Any = "ANY";
    public const string AnyClubMember = "ANY_CLUB_MEMBER";
    public const string AnyClubCommitteeMember = "ANY_CLUB_COMMITTEE_MEMBER";
    public const string PolicyClubMember = "POLICY_CLUB_MEMBER";
    public const string PolicyClubCommitteeMember = "POLICY_CLUB_COMMITTEE_MEMBER";
    public const string PilotClubMember = "PILOT_CLUB_MEMBER";
    public const string PilotClubCommitteeMember = "PILOT_CLUB_COMMITTEE_MEMBER";
}
