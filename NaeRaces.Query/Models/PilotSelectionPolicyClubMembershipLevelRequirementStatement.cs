using NaeRaces.Query.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Models;

public record PilotSelectionPolicyClubMembershipLevelRequirementStatement(Guid ClubId, int MembershipLevel) : IPilotSelectionPolicyStatement
{
    public string? IsValidForPilot(PilotValidationDetails pilotValidationDetails, DateTime OnDate)
    {
        var clubMembership = pilotValidationDetails.PilotClubs.FirstOrDefault(x => x.ClubId == ClubId);
        if(clubMembership == null)
        {
            return "PILOT_NOT_MEMBER_OF_REQUIRED_CLUB"; // Pilot is not a member of the required club
        }
        if(clubMembership.MembershipLevel != MembershipLevel)
        {
            return "PILOT_DOES_NOT_HAVE_REQUIRED_CLUB_MEMBERSHIP_LEVEL"; // Pilot's membership level does not meet the requirement
        }
        return null;

    }
}
