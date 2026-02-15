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
public record RaceCostSet(Guid RaceId, string Currency, decimal Cost);
public record RaceCostIncreased(Guid RaceId, string Currency, decimal Cost);
public record RaceCostReduced(Guid RaceId, string Currency, decimal Cost);
public record RacePaymentDeadlineScheduled(Guid RaceId, DateTime PaymentDeadline);
public record RaceUnconfirmedSlotConsumptionPolicySet(Guid RaceId, bool IsAllowed);
public record RaceProhibitsUnpaidUnconfirmedSlotConsumption(Guid RaceId);
public record RaceRegistrationOpenDateScheduled(Guid RaceId, DateTime RegistrationOpenDate);
public record RaceRegistrationOpenDateRescheduled(Guid RaceId, DateTime RegistrationOpenDate);
public record RaceEarlyRegistrationOpenDateScheduled(Guid RaceId, int EarlyRegistrationId, DateTime RegistrationOpenDate, Guid RacePolicyId, long PolicyVersion);
public record RaceEarlyRegistrationOpenDateRescheduled(Guid RaceId, int EarlyRegistrationId, DateTime RegistrationOpenDate);
public record RaceEarlyRegistrationPolicyChanged(Guid RaceId, int EarlyRegistrationId, Guid RacePolicyId, long PolicyVersion);
public record RaceEarlyRegistrationPolicyRemoved(Guid RaceId, int EarlyRegistrationId);
public record RaceDiscountAdded(Guid RaceId, int RaceDiscountId, Guid RacePolicyId, long PolicyVersion, string Currency, decimal Discount);
public record RaceDiscountAddedRemoved(Guid RaceId, int RaceDiscountId, Guid RacePolicyId, long PolicyVersion, string Currency, decimal Discount);
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

public record TeamRosterRegisteredForRace(Guid RaceId, Guid TeamId, int RosterId, IEnumerable<Guid> PilotIds, Guid RegistrationId);
public record IndividualPilotRegisteredForRace(Guid RaceId, Guid PilotId, Guid RegistrationId);

public record RaceRegistrationConfirmed(Guid RaceId, Guid RegistrationId);
public record RaceRegistrationCancelled(Guid RaceId, Guid RegistrationId);

public record RaceTaggedWithGlobalTag(Guid RaceId, string Tag);
public record RaceTaggedWithClubTag(Guid RaceId, Guid ClubId, string Tag);
public record RaceGlobalTagRemoved(Guid RaceId, string Tag);
public record RaceClubTagRemoved(Guid RaceId, Guid ClubId, string Tag);