using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Events;

public record RacePlanned(Guid RaceId, string Name, Guid ClubId, int LocationId);
public record RaceDescriptionSet(Guid RaceId, string Description);
public record TeamRacePlanned(Guid RaceId, string Name, int TeamSize, Guid ClubId, int LocationId);

public record RaceValidationPolicySet(Guid RaceId, Guid PolicyId, long PolicyVersion);
public record RaceValidationPolicyMigratedToVersion(Guid RaceId, Guid PolicyId, long PolicyVersion);
public record RaceValidationPolicyRemoved(Guid RaceId);

public record RaceDateScheduled(Guid RaceId, int RaceDateId, DateTime Start, DateTime End);
public record RaceDateRescheduled(Guid RaceId, int RaceDateId, DateTime Start, DateTime End);

public record RacePackageAdded(Guid RaceId, int RacePackageId, string Name, string Currency, decimal Cost);
public record RacePackagePriceIncreased(Guid RaceId, int RacePackageId, decimal Cost);
public record RacePackagePriceReduced(Guid RaceId, int RacePackageId, decimal Cost);
public record RacePackageDiscountStatusSet(Guid RaceId, int RacePackageId, bool ApplyDiscounts);
public record RacePackageRegistrationOpenScheduled(Guid RaceId, int RacePackageId, DateTime RegistrationOpenDate);
public record RacePackageRegistrationCloseScheduled(Guid RaceId, int RacePackageId, DateTime RegistrationCloseDate);
public record RacePackageRegistrationManuallyOpened(Guid RaceId, int RacePackageId);
public record RacePackageRegistrationManuallyClosed(Guid RaceId, int RacePackageId);
public record RacePackagePilotPolicySet(Guid RaceId, int RacePackageId, Guid PilotPolicyId, long PolicyVersion);
public record RacePackageRemoved(Guid RaceId, int RacePackageId);

public record RacePaymentDeadlineScheduled(Guid RaceId, DateTime PaymentDeadline);
public record RaceUnconfirmedSlotConsumptionPolicySet(Guid RaceId, bool IsAllowed);

public record RaceDiscountAdded(Guid RaceId, int RaceDiscountId, string Name, Guid PilotPolicyId, long PolicyVersion, string Currency, decimal Discount, bool IsPercentage, bool CanBeCombined);
public record RaceDiscountRemoved(Guid RaceId, int RaceDiscountId);
public record RaceDetailsPublished(Guid RaceId);
public record RacePublished(Guid RaceId);

public record RaceDateCancelled(Guid RaceId, int RaceDateId);
public record RaceGoNoGoScheduled(Guid RaceId, DateTime GoNoGoDate);
public record RaceGoNoGoRescheduled(Guid RaceId, DateTime GoNoGoDate);
public record RaceCancelled(Guid RaceId);
public record RaceRegistrationClosed(Guid RaceId);

public record RaceMinimumAttendeesSet(Guid RaceId, int MinimumAttendees);
public record RaceMaximumAttendeesSet(Guid RaceId, int MaximumAttendees);

public record TeamAttendancePermittedAtRace(Guid RaceId);
public record TeamAttendanceProhibitedAtRace(Guid RaceId);
public record TeamSubsitutionsPermittedAtRace(Guid RaceId);
public record TeamSubsitutionsProhibitedAtRace(Guid RaceId);
public record IndividualPilotAttendancePermittedAtRace(Guid RaceId);
public record IndividualPilotAttendanceProhibitedAtRace(Guid RaceId);

public record TeamRaceTeamSizeSet(Guid RaceId, int TeamSize);
public record TeamRaceMinimumTeamSizeSet(Guid RaceId, int MinimumTeamSize);
public record TeamRaceMaximumTeamSizeSet(Guid RaceId, int MaximumTeamSize);
public record TeamRaceMinimumTeamsSet(Guid RaceId, int MinimumTeams);
public record TeamRaceMaximumTeamsSet(Guid RaceId, int MaximumTeams);

public record TeamRosterPilotRegisteredForRace(Guid RaceId, Guid TeamId, int RosterId, Guid PilotId, Guid RegistrationId, int RacePackageId);
public record TeamRosterMemberSubsitutedForRace(Guid RaceId, int RosterId, Guid OutgoingPilotId, Guid IncomingPilotId, Guid RegistrationId, int RacePackageId);
public record IndividualPilotRegisteredForRace(Guid RaceId, Guid PilotId, Guid RegistrationId, int RacePackageId);

public record RaceRegistrationConfirmed(Guid RaceId, Guid RegistrationId);
public record RaceRegistrationCancelled(Guid RaceId, Guid RegistrationId);

public record RaceTaggedWithGlobalTag(Guid RaceId, string Tag);
public record RaceTaggedWithClubTag(Guid RaceId, Guid ClubId, string Tag);
public record RaceGlobalTagRemoved(Guid RaceId, string Tag);
public record RaceClubTagRemoved(Guid RaceId, Guid ClubId, string Tag);