using EventDbLite.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Command.Aggregates;

public class Race : AggregateRoot<Guid>
{
    public Guid? ClubId { get; private set; }

    private ValueTypes.Name? _name;
    private string? _description;
    private ValidationPolicy? _validationPolicy;
    private int? _locationId;
    private bool _isTeamRace;
    private int? _teamSize;
    private readonly Dictionary<int, RaceDate> _raceDates = [];
    private readonly Dictionary<int, RacePackage> _racePackages = [];
    private DateTime? _paymentDeadline;
    private bool _unconfirmedSlotConsumptionIsAllowed;
    private readonly Dictionary<int, Discount> _discounts = [];
    private bool _detailsPublished;
    private bool _published;
    private DateTime? _goNoGoDate;
    private bool _cancelled;
    private bool _registrationClosed;
    private int? _minimumAttendees;
    private int? _maximumAttendees;
    private bool _teamAttendancePermitted;
    private bool _teamSubstitutionsPermitted;
    private bool _individualPilotAttendancePermitted;
    private int? _minimumTeamSize;
    private int? _maximumTeamSize;
    private int? _minimumTeams;
    private int? _maximumTeams;
    private readonly Dictionary<Guid, Registration> _registrations = [];
    private readonly HashSet<Tag> _globalTags = [];
    private readonly Dictionary<Guid, HashSet<Tag>> _clubTags = [];
    private class ValidationPolicy
    {
        public Guid ValidationPolicyId { get; init; }
        public long ValidationPolicyVersion { get; init; }
    }
    private class RaceDate
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool Cancelled { get; set; }
    }

    private class ClubRequirement
    {
        public Guid ClubId { get; set; }
        public HashSet<int> MembershipLevelIds { get; set; } = [];
        public bool AllowAnyMember { get; set; }
    }

    private class RacePackage
    {
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public bool ApplyDiscounts { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public bool IsRegistrationOpen { get; set; }
        public DateTime? RegistrationCloseDate { get; set; }
        public Guid? PilotPolicyId { get; set; }
        public long? PolicyVersion { get; set; }
    }

    private class Discount
    {
        public Guid PilotPolicyId { get; set; }
        public long PolicyVersion { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public bool CanBeCombined { get; set; }
    }

    private class Registration
    {
        public Guid RegistrationId { get; set; }
        public bool IsTeamRegistration { get; set; }
        public Guid? TeamId { get; set; }
        public int? RosterId { get; set; }
        public int? RacePackageId { get; set; }
        public Guid PilotId { get; set; }
        public bool IsConfirmed { get; set; }
    }
    public Race()
    {

    }
    public Race(Guid raceId, ValueTypes.Name name, Guid clubId, int locationId)
    {
        Raise(new RacePlanned(raceId, name.Value, clubId, locationId));
    }

    public void SetRaceDescription(string description)
    {
        ThrowIfIdNotSet();
        if (_description == description)
            return;

        Raise(new RaceDescriptionSet(Id, description));
    }

    public Race(Guid raceId, ValueTypes.Name name, int teamSize, Guid clubId, int locationId)
    {
        Raise(new TeamRacePlanned(raceId, name.Value, teamSize, clubId, locationId));
    }

    public void SetRaceValidationPolicy(Guid validationPolicyId, long version)
    {
        ThrowIfIdNotSet();

        if (_validationPolicy != null && _validationPolicy.ValidationPolicyId == validationPolicyId && _validationPolicy.ValidationPolicyVersion == version)
            return;

        if (_published)
            throw new InvalidOperationException("Cannot change validation policy of a published race.");

        if (_validationPolicy != null && _validationPolicy.ValidationPolicyId == validationPolicyId)
            throw new InvalidOperationException("Migration required to change validation policy version");

        Raise(new RaceValidationPolicySet(Id, validationPolicyId, version));
    }

    public void MigrateRaceValidationPolicy(Guid validationPolicyId, long version)
    {
        ThrowIfIdNotSet();
        if (_validationPolicy == null || _validationPolicy.ValidationPolicyId != validationPolicyId)
            throw new InvalidOperationException("Validation policy must be set before it can be migrated.");
        if (_published)
            throw new InvalidOperationException("Cannot change validation policy of a published race.");

        Raise(new RaceValidationPolicyMigratedToVersion(Id, validationPolicyId, version));
    }

    public void RemoveRaceValidationPolicy()
    {
        ThrowIfIdNotSet();

        Raise(new RaceValidationPolicyRemoved(Id));
    }
    public void ScheduleRaceDate(DateTime start, DateTime end)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot schedule dates of a published race");

        if (start >= end)
            throw new ArgumentException("Start date must be before end date.");

        if (_raceDates.Values.Any(d => start < d.End && end > d.Start && !d.Cancelled))
            throw new InvalidOperationException("Race date conflicts with existing dates.");

        var raceDateId = _raceDates.Count > 0 ? _raceDates.Keys.Max() + 1 : 1;

        Raise(new RaceDateScheduled(Id, raceDateId, start, end));
    }

    public void RescheduleRaceDate(int raceDateId, DateTime start, DateTime end)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot reschedule dates of a published race.");

        if (!_raceDates.ContainsKey(raceDateId))
            throw new InvalidOperationException($"Race date {raceDateId} does not exist.");

        if (start >= end)
            throw new ArgumentException("Start date must be before end date.");

        if (_raceDates.Where(d => d.Key != raceDateId).Any(d => start < d.Value.End && end > d.Value.Start && !d.Value.Cancelled))
            throw new InvalidOperationException("Rescheduled race date conflicts with existing dates.");

        if (_racePackages.Values.Any(p => p.RegistrationOpenDate.HasValue && p.RegistrationOpenDate.Value >= start))
            throw new InvalidOperationException("Rescheduled race date must be after all package registration open dates");

        Raise(new RaceDateRescheduled(Id, raceDateId, start, end));
    }

    public int AddRacePackage(Name name, string currency, decimal cost)
    {
        ThrowIfIdNotSet();

        if (cost < 0)
            throw new ArgumentException("Cost cannot be negative.");

        var racePackageId = _racePackages.Count > 0 ? _racePackages.Keys.Max() + 1 : 1;

        Raise(new RacePackageAdded(Id, racePackageId, name.Value, currency, cost));

        return racePackageId;
    }

    public void SetRacePackagePrice(int racePackageId, decimal cost)
    {
        ThrowIfIdNotSet();

        if (!_racePackages.ContainsKey(racePackageId))
            throw new InvalidOperationException($"Race package {racePackageId} does not exist.");

        var package = _racePackages[racePackageId];
        if (package.Cost == cost)
            return;

        if (cost < package.Cost)
            Raise(new RacePackagePriceReduced(Id, racePackageId, cost));
        else
            Raise(new RacePackagePriceIncreased(Id, racePackageId, cost));
    }

    public void SetRacePackageDiscountStatus(int racePackageId, bool applyDiscounts)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change race package discount status of a published race.");

        if (!_racePackages.ContainsKey(racePackageId))
            throw new InvalidOperationException($"Race package {racePackageId} does not exist.");

        if (_racePackages[racePackageId].ApplyDiscounts == applyDiscounts)
            return;

        Raise(new RacePackageDiscountStatusSet(Id, racePackageId, applyDiscounts));
    }

    public void ScheduleRacePaymentDeadline(DateTime paymentDeadline)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change payment deadline of a published race.");

        Raise(new RacePaymentDeadlineScheduled(Id, paymentDeadline));
    }

    public void SetUnconfirmedSlotConsumptionPolicy(bool isAllowed)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot permit unpaid registration for a published race.");

        if (_unconfirmedSlotConsumptionIsAllowed == isAllowed)
            return;

        Raise(new RaceUnconfirmedSlotConsumptionPolicySet(Id, isAllowed));
    }

    public void ScheduleRacePackageRegistrationOpen(int racePackageId, DateTime registrationOpenDate)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change package registration open date of a published race.");

        if (!_racePackages.ContainsKey(racePackageId))
            throw new InvalidOperationException($"Race package {racePackageId} does not exist.");

        if (_raceDates.Values.Any(d => !d.Cancelled && registrationOpenDate >= d.Start))
            throw new InvalidOperationException("Registration open date must be before all race start dates.");

        if (_racePackages[racePackageId].RegistrationOpenDate == registrationOpenDate)
            return;

        Raise(new RacePackageRegistrationOpenScheduled(Id, racePackageId, registrationOpenDate));
    }

    public void ScheduleRacePackageRegistrationClose(int racePackageId, DateTime registrationCloseDate)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change package registration close date of a published race.");

        if (!_racePackages.ContainsKey(racePackageId))
            throw new InvalidOperationException($"Race package {racePackageId} does not exist.");

        var package = _racePackages[racePackageId];
        if (package.RegistrationOpenDate.HasValue && registrationCloseDate <= package.RegistrationOpenDate.Value)
            throw new InvalidOperationException("Registration close date must be after registration open date.");

        if (_racePackages[racePackageId].RegistrationCloseDate == registrationCloseDate)
            return;

        Raise(new RacePackageRegistrationCloseScheduled(Id, racePackageId, registrationCloseDate));
    }

    public void SetRacePackageRegistrationStatus(int racePackageId, bool isOpen)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change package registration status of a published race.");

        if (!_racePackages.ContainsKey(racePackageId))
            throw new InvalidOperationException($"Race package {racePackageId} does not exist.");

        if(_racePackages[racePackageId].IsRegistrationOpen == isOpen)
            return;

        if(isOpen)
            Raise(new RacePackageRegistrationManuallyOpened(Id, racePackageId));
        else
            Raise(new RacePackageRegistrationManuallyClosed(Id, racePackageId));
    }

    public void SetRacePackagePilotPolicy(int racePackageId, Guid pilotPolicyId, long policyVersion)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change package pilot policy of a published race.");

        if (!_racePackages.ContainsKey(racePackageId))
            throw new InvalidOperationException($"Race package {racePackageId} does not exist.");

        var package = _racePackages[racePackageId];
        if (package.PilotPolicyId == pilotPolicyId && package.PolicyVersion == policyVersion)
            return;

        Raise(new RacePackagePilotPolicySet(Id, racePackageId, pilotPolicyId, policyVersion));
    }

    public void RemoveRacePackage(int racePackageId)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot remove race packages from a published race.");

        if (!_racePackages.ContainsKey(racePackageId))
            throw new InvalidOperationException($"Race package {racePackageId} does not exist.");

        Raise(new RacePackageRemoved(Id, racePackageId));
    }

    public int AddRaceDiscount(Name name, Guid pilotPolicyId, long policyVersion, string currency, decimal discount, bool isPercentage, bool canBeCombined)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot add discounts to a published race.");

        if (discount <= 0)
            throw new ArgumentException("Discount must be between 0 and 1.");

        var discountId = _discounts.Count > 0 ? _discounts.Keys.Max() + 1 : 1;

        Raise(new RaceDiscountAdded(Id, discountId, name.Value, pilotPolicyId, policyVersion, currency, discount, isPercentage, canBeCombined));

        return discountId;
    }

    public void RemoveRaceDiscount(int discountId)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot remove discounts from a published race.");

        if (!_discounts.ContainsKey(discountId))
            throw new InvalidOperationException($"Discount {discountId} does not exist.");

        Raise(new RaceDiscountRemoved(Id, discountId));
    }

    public void PublishRaceDetails()
    {
        ThrowIfIdNotSet();
        if (_detailsPublished)
            return;

        Raise(new RaceDetailsPublished(Id));
    }

    public void PublishRace()
    {
        ThrowIfIdNotSet();
        if (_published)
            return;

        Raise(new RacePublished(Id));
    }

    public void CancelRaceDate(int raceDateId)
    {
        ThrowIfIdNotSet();
        if (!_raceDates.ContainsKey(raceDateId))
            throw new InvalidOperationException($"Race date {raceDateId} does not exist.");
        if (_raceDates[raceDateId].Cancelled)
            throw new InvalidOperationException($"Race date {raceDateId} is already cancelled.");

        Raise(new RaceDateCancelled(Id, raceDateId));
    }

    public void ScheduleRaceGoNoGo(DateTime goNoGoDate)
    {
        ThrowIfIdNotSet();

        Raise(new RaceGoNoGoScheduled(Id, goNoGoDate));
    }

    public void RescheduleRaceGoNoGo(DateTime goNoGoDate)
    {
        ThrowIfIdNotSet();
        if (!_goNoGoDate.HasValue)
            throw new InvalidOperationException("Go/No-Go date has not been scheduled.");

        Raise(new RaceGoNoGoRescheduled(Id, goNoGoDate));
    }

    public void CancelRace()
    {
        ThrowIfIdNotSet();
        if (_cancelled)
            return;

        Raise(new RaceCancelled(Id));
    }

    public void CloseRaceRegistration()
    {
        ThrowIfIdNotSet();
        if (_registrationClosed)
            return;

        Raise(new RaceRegistrationClosed(Id));
    }

    public void SetRaceMinimumAttendees(int minimumAttendees)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change minimum attendees of a published race.");

        Raise(new RaceMinimumAttendeesSet(Id, minimumAttendees));
    }

    public void SetRaceMaximumAttendees(int maximumAttendees)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change maximum attendees of a published race.");

        Raise(new RaceMaximumAttendeesSet(Id, maximumAttendees));
    }

    public void PermitTeamAttendanceAtRace()
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change team attendance permission of a published race.");

        if (_teamAttendancePermitted)
            return;

        Raise(new TeamAttendancePermittedAtRace(Id));
    }

    public void ProhibitTeamAttendanceAtRace()
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change team attendance permission of a published race.");

        if (!_teamAttendancePermitted)
            return;

        Raise(new TeamAttendanceProhibitedAtRace(Id));
    }

    public void PermitTeamSubstitutionsAtRace()
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change team substitutions permission of a published race.");

        if (_teamSubstitutionsPermitted)
            return;

        Raise(new TeamSubsitutionsPermittedAtRace(Id));
    }

    public void ProhibitTeamSubstitutionsAtRace()
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change team substitutions permission of a published race.");

        if (!_teamSubstitutionsPermitted)
            return;

        Raise(new TeamSubsitutionsProhibitedAtRace(Id));
    }

    public void PermitIndividualPilotAttendanceAtRace()
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change individual pilot attendance permission of a published race.");

        if (_individualPilotAttendancePermitted)
            return;

        Raise(new IndividualPilotAttendancePermittedAtRace(Id));
    }

    public void ProhibitIndividualPilotAttendanceAtRace()
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change individual pilot attendance permission of a published race.");

        if (!_individualPilotAttendancePermitted)
            return;

        Raise(new IndividualPilotAttendanceProhibitedAtRace(Id));
    }

    public void SetTeamRaceTeamSize(int teamSize)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change team size of a published race");

        if (!_isTeamRace)
            throw new InvalidOperationException("Race is not a team race.");

        Raise(new TeamRaceTeamSizeSet(Id, teamSize));
    }

    public void SetTeamRaceMinimumTeamSize(int minimumTeamSize)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change minimum team size of a published race");

        if (!_isTeamRace)
            throw new InvalidOperationException("Race is not a team race.");

        Raise(new TeamRaceMinimumTeamSizeSet(Id, minimumTeamSize));
    }

    public void SetTeamRaceMaximumTeamSize(int maximumTeamSize)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change maximum team size of a published race");

        if (!_isTeamRace)
            throw new InvalidOperationException("Race is not a team race.");

        Raise(new TeamRaceMaximumTeamSizeSet(Id, maximumTeamSize));
    }

    public void SetTeamRaceMinimumTeams(int minimumTeams)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change minimum teams of a published race");

        if (!_isTeamRace)
            throw new InvalidOperationException("Race is not a team race.");

        Raise(new TeamRaceMinimumTeamsSet(Id, minimumTeams));
    }

    public void SetTeamRaceMaximumTeams(int maximumTeams)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change maximum teams of a published race");

        if (!_isTeamRace)
            throw new InvalidOperationException("Race is not a team race.");

        Raise(new TeamRaceMaximumTeamsSet(Id, maximumTeams));
    }
    public void RegisterTeamRosterForRace(Guid teamId, IEnumerable<TeamMemberRegistration> teamMemberRegistrations)
    {
        ThrowIfIdNotSet();

        if (_registrations.Keys.Any(x => teamMemberRegistrations.Select(x => x.RegistrationId).Contains(x)))
            throw new InvalidOperationException($"One or more registrations already exist.");
        if (!_teamAttendancePermitted)
            throw new InvalidOperationException("Team attendance is not permitted for this race");

        IEnumerable<Guid> existingPilotIds = _registrations.Values.Select(r => r.PilotId);

        if (teamMemberRegistrations.Select(x => x.PilotId).Any(pid => existingPilotIds.Contains(pid)))
            throw new InvalidOperationException("One or more pilots in the roster are already registered");

        if (_registrations.Values.Any(r => r.IsTeamRegistration && r.TeamId == teamId))
            throw new InvalidOperationException($"Team {teamId} already has a roster registered for this race");

        // Count registrations based on slot consumption policy
        var registrationsToCount = _unconfirmedSlotConsumptionIsAllowed
            ? _registrations.Values
            : _registrations.Values.Where(r => r.IsConfirmed);

        int registeredTeamCount = registrationsToCount.Count(r => r.IsTeamRegistration);

        if (_maximumTeams.HasValue && registeredTeamCount >= _maximumTeams.Value)
            throw new InvalidOperationException("Maximum number of teams already registered for this race");

        int registeredPilotsCount = registrationsToCount.Select(r => r.PilotId).Count();

        if (_maximumAttendees.HasValue && registeredPilotsCount + teamMemberRegistrations.Count() > _maximumAttendees.Value)
            throw new InvalidOperationException("Registering this roster would exceed the maximum number of attendees");

        if (_teamSize.HasValue && teamMemberRegistrations.Count() > _teamSize.Value)
            throw new InvalidOperationException($"Team roster size cannot exceed the defined team size of {_teamSize.Value} for this race");

        if (_minimumTeamSize.HasValue && teamMemberRegistrations.Count() < _minimumTeamSize.Value)
            throw new InvalidOperationException($"Team roster size cannot be less than the defined minimum team size of {_minimumTeamSize.Value} for this race");

        int rosterId = (_registrations.Values.Where(r => r.IsTeamRegistration && r.TeamId == teamId).Select(r => r.RosterId).DefaultIfEmpty(0).Max() + 1) ?? 0;

        foreach (var memberRegistration in teamMemberRegistrations)
        {
            Raise(new TeamRosterPilotRegisteredForRace(Id, teamId, rosterId, memberRegistration.PilotId, memberRegistration.RegistrationId, memberRegistration.RacePackageId));
        }
    }

    public void RegisterIndividualPilotForRace(Guid pilotId, Guid registrationId, int racePackageId)
    {
        ThrowIfIdNotSet();
        if (_registrations.ContainsKey(registrationId))
            throw new InvalidOperationException($"Registration {registrationId} already exists.");

        if (!_individualPilotAttendancePermitted)
            throw new InvalidOperationException("Individual pilot attendance is not permitted for this race and cannot be registered.");

        if (_registrations.Values.Any(r => r.PilotId == pilotId))
            throw new InvalidOperationException("Pilot has already been registered");

        // Count registrations based on slot consumption policy
        var registrationsToCount = _unconfirmedSlotConsumptionIsAllowed
            ? _registrations.Values
            : _registrations.Values.Where(r => r.IsConfirmed);

        int registeredPilotsCount = registrationsToCount.Select(r => r.PilotId).Count();

        if (_maximumAttendees.HasValue && registeredPilotsCount >= _maximumAttendees.Value)
            throw new InvalidOperationException("Maximum number of attendees already registered for this race");

        Raise(new IndividualPilotRegisteredForRace(Id, pilotId, registrationId, racePackageId));
    }

    public void ConfirmRaceRegistration(Guid registrationId)
    {
        ThrowIfIdNotSet();
        if (!_registrations.ContainsKey(registrationId))
            throw new InvalidOperationException($"Registration {registrationId} does not exist.");
        if (_registrations[registrationId].IsConfirmed)
            throw new InvalidOperationException($"Registration {registrationId} is already confirmed.");

        Raise(new RaceRegistrationConfirmed(Id, registrationId));
    }

    public void CancelRaceRegistration(Guid registrationId)
    {
        ThrowIfIdNotSet();
        if (!_registrations.ContainsKey(registrationId))
            throw new InvalidOperationException($"Registration {registrationId} does not exist.");

        Raise(new RaceRegistrationCancelled(Id, registrationId));
    }

    public void TagRaceWithGlobalTag(Tag tag)
    {
        ThrowIfIdNotSet();
        if (_globalTags.Contains(tag))
            throw new InvalidOperationException($"Global tag {tag} already exists.");

        Raise(new RaceTaggedWithGlobalTag(Id, tag.Value));
    }

    public void TagRaceWithClubTag(Guid clubId, Tag tag)
    {
        ThrowIfIdNotSet();
        if (_clubTags.TryGetValue(clubId, out var tags) && tags.Contains(tag))
            throw new InvalidOperationException($"Club tag {tag} for club {clubId} already exists.");

        Raise(new RaceTaggedWithClubTag(Id, clubId, tag.Value));
    }

    public void RemoveGlobalTagFromRace(Tag tag)
    {
        ThrowIfIdNotSet();
        if (!_globalTags.Contains(tag))
            throw new InvalidOperationException($"Global tag {tag} does not exist.");
        Raise(new RaceGlobalTagRemoved(Id, tag.Value));
    }
    public void RemoveClubTagFromRace(Guid clubId, Tag tag)
    {
        ThrowIfIdNotSet();
        if (!_clubTags.ContainsKey(clubId) || !_clubTags[clubId].Contains(tag))
            throw new InvalidOperationException($"Club tag {tag} for club {clubId} does not exist.");
        Raise(new RaceClubTagRemoved(Id, clubId, tag.Value));
    }

    // Event handlers
    private void When(RacePlanned e)
    {
        Id = e.RaceId;
        _name = ValueTypes.Name.Rehydrate(e.Name);
        ClubId = e.ClubId;
        _locationId = e.LocationId;
        _isTeamRace = false;
    }

    private void When(RaceDescriptionSet e)
    {
        _description = e.Description;
    }

    private void When(TeamRacePlanned e)
    {
        Id = e.RaceId;
        _name = ValueTypes.Name.Rehydrate(e.Name);
        _teamSize = e.TeamSize;
        _isTeamRace = true;
    }

    private void When(RaceDateScheduled e)
    {
        _raceDates[e.RaceDateId] = new RaceDate
        {
            Start = e.Start,
            End = e.End,
            Cancelled = false
        };
    }

    private void When(RaceDateRescheduled e)
    {
        _raceDates[e.RaceDateId].Start = e.Start;
        _raceDates[e.RaceDateId].End = e.End;
    }

    private void When(RacePackageAdded e)
    {
        _racePackages[e.RacePackageId] = new RacePackage
        {
            Name = e.Name,
            Currency = e.Currency,
            Cost = e.Cost,
            ApplyDiscounts = true
        };
    }
    private void When(RacePackagePriceIncreased e)
    {
        _racePackages[e.RacePackageId].Cost = e.Cost;
    }
    private void When(RacePackagePriceReduced e)
    {
        _racePackages[e.RacePackageId].Cost = e.Cost;
    }

    private void When(RacePackageDiscountStatusSet e)
    {
        _racePackages[e.RacePackageId].ApplyDiscounts = e.ApplyDiscounts;
    }

    private void When(RacePackageRegistrationOpenScheduled e)
    {
        _racePackages[e.RacePackageId].RegistrationOpenDate = e.RegistrationOpenDate;
    }

    private void When(RacePackageRegistrationCloseScheduled e)
    {
        _racePackages[e.RacePackageId].RegistrationCloseDate = e.RegistrationCloseDate;
    }

    private void When(RacePackagePilotPolicySet e)
    {
        _racePackages[e.RacePackageId].PilotPolicyId = e.PilotPolicyId;
        _racePackages[e.RacePackageId].PolicyVersion = e.PolicyVersion;
    }

    private void When(RacePackageRemoved e)
    {
        _racePackages.Remove(e.RacePackageId);
    }

    private void When(RacePaymentDeadlineScheduled e)
    {
        _paymentDeadline = e.PaymentDeadline;
    }

    private void When(RaceUnconfirmedSlotConsumptionPolicySet e)
    {
        _unconfirmedSlotConsumptionIsAllowed = e.IsAllowed;
    }

    private void When(RaceDiscountAdded e)
    {
        _discounts[e.RaceDiscountId] = new Discount
        {
            PilotPolicyId = e.PilotPolicyId,
            PolicyVersion = e.PolicyVersion,
            Currency = e.Currency,
            DiscountAmount = e.Discount
        };
    }

    private void When(RaceDiscountRemoved e)
    {
        _discounts.Remove(e.RaceDiscountId);
    }

    private void When(RaceDetailsPublished e)
    {
        _detailsPublished = true;
    }

    private void When(RacePublished e)
    {
        _published = true;
    }

    private void When(RaceDateCancelled e)
    {
        _raceDates[e.RaceDateId].Cancelled = true;
    }

    private void When(RaceGoNoGoScheduled e)
    {
        _goNoGoDate = e.GoNoGoDate;
    }

    private void When(RaceGoNoGoRescheduled e)
    {
        _goNoGoDate = e.GoNoGoDate;
    }

    private void When(RaceCancelled e)
    {
        _cancelled = true;
    }

    private void When(RaceRegistrationClosed e)
    {
        _registrationClosed = true;
    }

    private void When(RaceMinimumAttendeesSet e)
    {
        _minimumAttendees = e.MinimumAttendees;
    }

    private void When(RaceMaximumAttendeesSet e)
    {
        _maximumAttendees = e.MaximumAttendees;
    }

    private void When(TeamAttendancePermittedAtRace e)
    {
        _teamAttendancePermitted = true;
    }

    private void When(TeamAttendanceProhibitedAtRace e)
    {
        _teamAttendancePermitted = false;
    }

    private void When(TeamSubsitutionsPermittedAtRace e)
    {
        _teamSubstitutionsPermitted = true;
    }

    private void When(TeamSubsitutionsProhibitedAtRace e)
    {
        _teamSubstitutionsPermitted = false;
    }

    private void When(IndividualPilotAttendancePermittedAtRace e)
    {
        _individualPilotAttendancePermitted = true;
    }

    private void When(IndividualPilotAttendanceProhibitedAtRace e)
    {
        _individualPilotAttendancePermitted = false;
    }

    private void When(TeamRaceTeamSizeSet e)
    {
        _teamSize = e.TeamSize;
    }

    private void When(TeamRaceMinimumTeamSizeSet e)
    {
        _minimumTeamSize = e.MinimumTeamSize;
    }

    private void When(TeamRaceMaximumTeamSizeSet e)
    {
        _maximumTeamSize = e.MaximumTeamSize;
    }

    private void When(TeamRaceMinimumTeamsSet e)
    {
        _minimumTeams = e.MinimumTeams;
    }

    private void When(TeamRaceMaximumTeamsSet e)
    {
        _maximumTeams = e.MaximumTeams;
    }

    private void When(IndividualPilotRegisteredForRace e)
    {
        _registrations[e.RegistrationId] = new Registration
        {
            RegistrationId = e.RegistrationId,
            IsTeamRegistration = false,
            RacePackageId = e.RacePackageId,
            PilotId = e.PilotId,
            IsConfirmed = false
        };
    }

    private void When(TeamRosterPilotRegisteredForRace e)
    {
        _registrations[e.RegistrationId] = new Registration
        {
            RegistrationId = e.RegistrationId,
            IsTeamRegistration = true,
            TeamId = e.TeamId,
            RosterId = e.RosterId,
            RacePackageId = e.RacePackageId,
            PilotId = e.PilotId,
            IsConfirmed = false
        };
    }

    private void When(RaceRegistrationConfirmed e)
    {
        _registrations[e.RegistrationId].IsConfirmed = true;
    }

    private void When(RaceRegistrationCancelled e)
    {
        _registrations.Remove(e.RegistrationId);
    }

    private void When(RaceTaggedWithGlobalTag e)
    {
        _globalTags.Add(Tag.Rehydrate(e.Tag));
    }

    private void When(RaceTaggedWithClubTag e)
    {
        if (!_clubTags.ContainsKey(e.ClubId))
        {
            _clubTags[e.ClubId] = [];
        }
        _clubTags[e.ClubId].Add(Tag.Rehydrate(e.Tag));
    }

    private void When(RaceGlobalTagRemoved e)
    {
        _globalTags.Remove(Tag.Rehydrate(e.Tag));
    }

    private void When(RaceClubTagRemoved e)
    {
        if (_clubTags.ContainsKey(e.ClubId))
        {
            _clubTags[e.ClubId].Remove(Tag.Rehydrate(e.Tag));
        }
    }
}
