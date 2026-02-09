using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Events;

public record PilotRegistered(Guid PilotId, string CallSign);
public record PilotCallSignChanged(Guid PilotId, string NewCallSign);
public record PilotNationalitySet(Guid Pilot,string Nationality);
public record PilotDateOfBirthSet(Guid PilotId, DateTime DateOfBirth);

public record PilotGovernmentDocumentationValidatedByPeer(Guid PilotId, string GovernmentDocument, DateTime ValidUntil, Guid ClubId, Guid ValidatedByPilotId, bool IsValidatingMemberOnCommiteeOfClub);
public record PilotInsuranceValidatedByPeer(Guid PilotId, string InsuranceProvider, DateTime ValidUntil, Guid ClubId, Guid ValidatedByPilotId, bool IsValidatingMemberOnCommiteeOfClub);
public record PilotDateOfBirthValidatedByPeer(Guid PilotId, Guid ClubId, Guid ValidatedByPilotId, bool IsValidatingMemberOnCommiteeOfClub);