using EventDbLite.Aggregates;
using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Command.Aggregates;

public class Race : AggregateRoot<Guid>
{
    private ValueTypes.Name? _name;
    private string? _description;
    private Guid? _clubId;
    private ValidationPolicy? _validationPolicy;
    private int? _locationId;
    private bool _isTeamRace;
    private int? _teamSize;
    private readonly Dictionary<int, RaceDate> _raceDates = [];
    private readonly Dictionary<string, decimal> _costs = [];
    private DateTime? _paymentDeadline;
    private bool _permitsUnpaidRegistration;
    private DateTime? _registrationOpenDate;
    private readonly Dictionary<(Guid ClubId, int MembershipLevelId), decimal> _membershipDiscounts = [];
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
    private readonly HashSet<string> _globalTags = [];
    private readonly Dictionary<Guid, HashSet<string>> _clubTags = [];
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

    private class Registration
    {
        public Guid RegistrationId { get; set; }
        public bool IsTeamRegistration { get; set; }
        public Guid? TeamId { get; set; }
        public int? RosterId { get; set; }
        public Guid? PilotId { get; set; }
        public string Currency { get; set; } = string.Empty;
        public double BasePrice { get; set; }
        public double Discount { get; set; }
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

    public Race(Guid raceId, ValueTypes.Name name, int teamSize)
    {
        Raise(new TeamRacePlanned(raceId, name.Value, teamSize));
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

        if(_raceDates.Values.Any(d => start < d.End && end > d.Start && !d.Cancelled))
            throw new InvalidOperationException("Race date conflicts with existing dates.");

        var raceDateId = _raceDates.Count > 0 ? _raceDates.Keys.Max() + 1 : 1;

        Raise(new RaceDateScheduled(Id, raceDateId, start, end));
    }

    public void RescheduleRaceDate(int raceDateId, DateTime start, DateTime end)
    {
        ThrowIfIdNotSet();

        if(_published)
            throw new InvalidOperationException("Cannot reschedule dates of a published race.");

        if (!_raceDates.ContainsKey(raceDateId))
            throw new InvalidOperationException($"Race date {raceDateId} does not exist.");

        if(start >= end)
            throw new ArgumentException("Start date must be before end date.");

        if(_raceDates.Where(d => d.Key != raceDateId).Any(d => start < d.Value.End && end > d.Value.Start && !d.Value.Cancelled))
            throw new InvalidOperationException("Rescheduled race date conflicts with existing dates.");

        if(_registrationOpenDate.HasValue && _registrationOpenDate.Value >= start)
            throw new InvalidOperationException("Rescheduled race date must be after registration open");

        Raise(new RaceDateRescheduled(Id, raceDateId, start, end));
    }

    public void SetRaceCost(string currency, decimal cost)
    {
        ThrowIfIdNotSet();
        
        if(_published)
            throw new InvalidOperationException("Cannot change cost of a published race.");

        if (cost < 0)
            throw new ArgumentException("Cost cannot be negative.");

        if(_costs.TryGetValue(currency, out var existingCost) && existingCost == cost)
            return;
        
        var currentCost = _costs.ContainsKey(currency) ? _costs[currency] : 0m;

        if(currentCost < cost)
            Raise(new RaceCostIncreased(Id, currency, cost));
        else if(currentCost > cost)
            Raise(new RaceCostReduced(Id, currency, cost));
        else
            Raise(new RaceCostSet(Id, currency, cost));
    }

    public void ScheduleRacePaymentDeadline(DateTime paymentDeadline)
    {
        ThrowIfIdNotSet();

        if(_published)
            throw new InvalidOperationException("Cannot change payment deadline of a published race.");

        Raise(new RacePaymentDeadlineScheduled(Id, paymentDeadline));
    }

    public void PermitUnpaidRegistration()
    {
        ThrowIfIdNotSet();

        if(_published) 
            throw new InvalidOperationException("Cannot permit unpaid registration for a published race.");

        if (_permitsUnpaidRegistration)
            return;

        Raise(new RacePermitsUnpaidRegistration(Id));
    }

    public void ProhibitUnpaidRegistration()
    {
        ThrowIfIdNotSet();

        if (_published) 
            throw new InvalidOperationException("Cannot prohibit unpaid registration for a published race.");

        if (!_permitsUnpaidRegistration)
            return;

        Raise(new RaceProhibitsUnpaidRegistration(Id));
    }

    public void ScheduleRaceRegistrationOpenDate(DateTime registrationOpenDate)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change registration open date of a published race.");

        if (_raceDates.Values.Any(d => !d.Cancelled && registrationOpenDate >= d.Start))
            throw new InvalidOperationException("Registration open date must be before all race start dates.");

        if(_registrationOpenDate.HasValue && _registrationOpenDate.Value == registrationOpenDate)
            return;
            

        Raise(new RaceRegistrationOpenDateScheduled(Id, registrationOpenDate));
    }

    public void SetRaceClubMembershipLevelDiscount(Guid clubId, int membershipLevelId, decimal discount)
    {
        ThrowIfIdNotSet();

        if (_published)
            throw new InvalidOperationException("Cannot change membership level discounts of a published race.");

        Raise(new RaceClubMembershipLevelDiscountSet(Id, clubId, membershipLevelId, discount));
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

        if(_published)
            throw new InvalidOperationException("Cannot change minimum attendees of a published race.");

        Raise(new RaceMinimumAttendeesSet(Id, minimumAttendees));
    }

    public void SetRaceMaximumAttendees(int maximumAttendees)
    {
        ThrowIfIdNotSet();

        if(_published)
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

    public void RegisterTeamRosterForRace(Guid teamId, int rosterId, Guid registrationId, string currency, double basePrice, double discount)
    {
        ThrowIfIdNotSet();
        if (_registrations.ContainsKey(registrationId))
            throw new InvalidOperationException($"Registration {registrationId} already exists.");

        Raise(new TeamRosterRegisteredForRace(Id, teamId, rosterId, registrationId, currency, basePrice, discount));
    }

    public void RegisterIndividualPilotForRace(Guid pilotId, Guid registrationId, string currency, double basePrice, double discount)
    {
        ThrowIfIdNotSet();
        if (_registrations.ContainsKey(registrationId))
            throw new InvalidOperationException($"Registration {registrationId} already exists.");

        Raise(new IndividualPilotRegisteredForRace(Id, pilotId, registrationId, currency, basePrice, discount));
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

    public void TagRaceWithGlobalTag(string tag)
    {
        ThrowIfIdNotSet();
        if (_globalTags.Contains(tag))
            throw new InvalidOperationException($"Global tag {tag} already exists.");

        Raise(new RaceTaggedWithGlobalTag(Id, tag));
    }

    public void TagRaceWithClubTag(Guid clubId, string tag)
    {
        ThrowIfIdNotSet();
        if (_clubTags.TryGetValue(clubId, out var tags) && tags.Contains(tag))
            throw new InvalidOperationException($"Club tag {tag} for club {clubId} already exists.");

        Raise(new RaceTaggedWithClubTag(Id, clubId, tag));
    }

    // Event handlers
    private void When(RacePlanned e)
    {
        Id = e.RaceId;
        _name = ValueTypes.Name.Rehydrate(e.Name);
        _clubId = e.ClubId;
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

    private void When(RaceCostSet e)
    {
        _costs[e.Currency] = e.Cost;
    }

    private void When(RaceCostIncreased e)
    {
        _costs[e.Currency] = e.Cost;
    }

    private void When(RaceCostReduced e)
    {
        _costs[e.Currency] = e.Cost;
    }

    private void When(RacePaymentDeadlineScheduled e)
    {
        _paymentDeadline = e.PaymentDeadline;
    }

    private void When(RacePermitsUnpaidRegistration e)
    {
        _permitsUnpaidRegistration = true;
    }

    private void When(RaceProhibitsUnpaidRegistration e)
    {
        _permitsUnpaidRegistration = false;
    }

    private void When(RaceRegistrationOpenDateScheduled e)
    {
        _registrationOpenDate = e.RegistrationOpenDate;
    }

    private void When(RaceClubMembershipLevelDiscountSet e)
    {
        _membershipDiscounts[(e.ClubId, e.MembershipLevelId)] = e.Discount;
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

    private void When(TeamRosterRegisteredForRace e)
    {
        _registrations[e.RegistrationId] = new Registration
        {
            RegistrationId = e.RegistrationId,
            IsTeamRegistration = true,
            TeamId = e.TeamId,
            RosterId = e.RosterId,
            Currency = e.currency,
            BasePrice = e.basePrice,
            Discount = e.discount,
            IsConfirmed = false
        };
    }

    private void When(IndividualPilotRegisteredForRace e)
    {
        _registrations[e.RegistrationId] = new Registration
        {
            RegistrationId = e.RegistrationId,
            IsTeamRegistration = false,
            PilotId = e.PilotId,
            Currency = e.currency,
            BasePrice = e.basePrice,
            Discount = e.discount,
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
        _globalTags.Add(e.Tag);
    }

    private void When(RaceTaggedWithClubTag e)
    {
        if (!_clubTags.ContainsKey(e.ClubId))
        {
            _clubTags[e.ClubId] = [];
        }
        _clubTags[e.ClubId].Add(e.Tag);
    }
}
