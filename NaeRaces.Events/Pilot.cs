using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Events;

public record PilotRegistered(Guid PilotId, string CallSign, string Email, string UserName, string PasswordHash);
public record PilotCallSignChanged(Guid PilotId, string NewCallSign);
public record PilotNameSet(Guid PilotId, string Name);
public record PilotNationalitySet(Guid PilotId, string Nationality);
public record PilotDateOfBirthSet(Guid PilotId, DateTime DateOfBirth);

public record PilotGovernmentDocumentationValidatedByPeer(Guid PilotId, string GovernmentDocument, DateTime ValidUntil, Guid ClubId, Guid ValidatedByPilotId, bool IsValidatingMemberOnCommiteeOfClub);
public record PilotInsuranceValidatedByPeer(Guid PilotId, string InsuranceProvider, DateTime ValidUntil, Guid ClubId, Guid ValidatedByPilotId, bool IsValidatingMemberOnCommiteeOfClub);
public record PilotDateOfBirthValidatedByPeer(Guid PilotId, Guid ClubId, Guid ValidatedByPilotId, bool IsValidatingMemberOnCommiteeOfClub);

public record PilotPasswordChanged(Guid PilotId, string PasswordHash);
public record PilotRoleAssigned(Guid PilotId, string Role);
public record PilotRoleRemoved(Guid PilotId, string Role);

public record PilotFollowedClub(Guid PilotId, Guid ClubId);
public record PilotUnfollowedClub(Guid PilotId, Guid ClubId);