using NaeRaces.Events.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Events;

public record ClubFormed(Guid ClubId, string Code, string Name, Guid FounderPilotId);
public record ClubDetailsChanged(Guid ClubId, string Code, string Name);
public record ClubDescriptionSet(Guid ClubId, string Description);
public record ClubContactDetailsSet(Guid ClubId, string PhoneNumber, string EmailAddress);
public record ClubLocationAdded(Guid ClubId, int LocationId, string LocationName, string LocationInformation, Address Address);
public record ClubLocationRemoved(Guid ClubId, int LocationId);
public record ClubLocationRenamed(Guid ClubId, int LocationId, string NewLocationName);
public record ClubLocationAddressChanged(Guid ClubId, int LocationId, Address NewAddress);
public record ClubLocationInformationChanged(Guid ClubId, int LocationId, string NewLocationInformation);
public record ClubLocationTimezoneOffsetSet(Guid ClubId, int LocationId, int TimezoneOffsetMinutes, bool UseDaylightSavings);
public record ClubHomeLocationSet(Guid ClubId, int LocationId);
public record ClubMinimumAgeSet(Guid ClubId, int MinimumAge, string ValidationPolicy);
public record ClubMaximumAgeSet(Guid ClubId, int MaximumAge, string ValidationPolicy);
public record ClubInsuranceRequirementSet(Guid RaceId, bool IsRequired, IEnumerable<string> AcceptedInsuranceProviders);
public record ClubGovernmentDocumentValidationRequirementSet(Guid RaceId, bool IsRequired);
public record ClubCommitteeMemberAdded(Guid ClubId, Guid PilotId, string Role);
public record ClubCommitteeMemberRemoved(Guid ClubId, Guid PilotId);
public record ClubMembershipLevelAdded(Guid ClubId, int MembershipLevelId, string Name);
public record ClubMembershipLevelRemoved(Guid ClubId, int MembershipLevelId);
public record ClubMembershipLevelRenamed(Guid ClubId, int MembershipLevelId, string NewName);
public record ClubMembershipLevelAgeSet(Guid ClubId, int MembershipLevelId, int MinimumAge, string ValidationPolicy);
public record ClubMembershipLevelMaximumAgeSet(Guid ClubId, int MembershipLevelId, int MaximumAge, string ValidationPolicy);
public record ClubMembershipLevelAnnualPaymentOptionAdded(Guid ClubId, int MembershipLevelId, int PaymentOptionId, string Name,string Currency, decimal Price);
public record ClubMembershipLevelMonthlyPaymentOptionAdded(Guid ClubId, int MembershipLevelId, int PaymentOptionId, string Name, int DayOfMonthDue, int PaymentInterval,string Currency, decimal Price);
public record ClubMembershipLevelSubscriptionPaymentOptionAdded(Guid ClubId, int MembershipLevelId, int PaymentOptionId, string Name, int PaymentInterval,string Currency, decimal Price);
public record ClubMembershipLevelPaymentOptionRemoved(Guid ClubId, int MembershipLevelId, int PaymentOptionId);
public record ClubMembershipLevelPaymentOptionRenamed(Guid ClubId, int MembershipLevelId, int PaymentOptionId, string NewName);

public record PilotRegisteredForClubMembershipLevel(Guid ClubId, int MembershipLevelId, int PaymentOptionId, Guid PilotId, Guid RegistrationId);
public record PilotClubMembershipConfirmed(Guid ClubId, int MembershipLevelId, int PaymentOptionId, Guid PilotId, Guid RegistrationId);
public record PilotClubMembershipCancelled(Guid ClubId, Guid PilotId);

public record ClubRaceTagAdded(Guid ClubId, string Tag, string Colour);
public record ClubRaceTagRemoved(Guid ClubId, string Tag);
