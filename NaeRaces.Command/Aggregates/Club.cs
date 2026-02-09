using EventDbLite.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.Events;
using NaeRaces.Events.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Command.Aggregates;

public class Club : AggregateRoot<Guid>
{
    private Code _code;
    private Name _name;
    private string? _description;
    private string? _phoneNumber;
    private string? _emailAddress;
    private readonly Dictionary<int, Location> _locations = [];
    private int? _homeLocationId;
    private int? _minimumAge;
    private int? _maximumAge;
    private string? _minimumAgeValidationPolicy;
    private string? _maximumAgeValidationPolicy;
    private bool _insuranceRequired;
    private readonly HashSet<string> _acceptedInsuranceProviders = [];
    private bool _governmentDocumentValidationRequired;
    private readonly Dictionary<Guid, CommitteeMember> _committeeMembers = [];
    private readonly Dictionary<int, MembershipLevel> _membershipLevels = [];
    private readonly Dictionary<Guid, PilotMembership> _pilotMemberships = [];
    private readonly Dictionary<string, string> _raceTags = [];

    private class Location
    {
        public Name Name { get; set; }
        public string Information { get; set; } = string.Empty;
        public ValueTypes.Address Address { get; set; }
    }

    private class CommitteeMember
    {
        public Guid PilotId { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    private class MembershipLevel
    {
        public Name Name { get; set; }
        public int? MinimumAge { get; set; }
        public int? MaximumAge { get; set; }
        public string? AgeValidationPolicy { get; set; }
        public Dictionary<int, PaymentOption> PaymentOptions { get; set; } = [];
    }

    private class PaymentOption
    {
        public Name Name { get; set; }
    }

    private class PilotMembership
    {
        public int MembershipLevelId { get; set; }
        public int PaymentOptionId { get; set; }
        public Guid RegistrationId { get; set; }
        public bool IsConfirmed { get; set; }
    }

    public Club(Guid clubId,Code code, Name name, Guid founderPilotId)
    {
        Raise(new ClubFormed(clubId, code.Value, name.Value, founderPilotId));
    }

    public void ChangeClubDetails(Code code, Name name)
    {
        ThrowIfIdNotSet();
        if (_name.Equals(name) && _code.Equals(code))
            return;

        Raise(new ClubDetailsChanged(Id, code.Value, name.Value));
    }

    public void SetClubDescription(string description)
    {
        ThrowIfIdNotSet();
        if (_description == description)
            return;

        Raise(new ClubDescriptionSet(Id, description));
    }

    public void SetClubContactDetails(string phoneNumber, string emailAddress)
    {
        ThrowIfIdNotSet();
        Raise(new ClubContactDetailsSet(Id, phoneNumber, emailAddress));
    }

    public void AddClubLocation(int locationId, string locationName, string locationInformation, ValueTypes.Address address)
    {
        ThrowIfIdNotSet();
        if (_locations.ContainsKey(locationId))
            throw new InvalidOperationException($"Location {locationId} already exists.");

        Raise(new ClubLocationAdded(Id, locationId, locationName, locationInformation, 
            new Events.ValueObjects.Address(address.AddressLine1, address.AddressLine2,address.City, address.Postcode, address.County)));
    }

    public void RemoveClubLocation(int locationId)
    {
        ThrowIfIdNotSet();
        if (!_locations.ContainsKey(locationId))
            throw new InvalidOperationException($"Location {locationId} does not exist.");

        Raise(new ClubLocationRemoved(Id, locationId));
    }

    public void RenameClubLocation(int locationId, Name newLocationName)
    {
        ThrowIfIdNotSet();
        if (!_locations.ContainsKey(locationId))
            throw new InvalidOperationException($"Location {locationId} does not exist.");

        if(_locations[locationId].Name.Equals(newLocationName))
            return;

        Raise(new ClubLocationRenamed(Id, locationId, newLocationName.Value));
    }

    public void ChangeClubLocationAddress(int locationId, ValueTypes.Address newAddress)
    {
        ThrowIfIdNotSet();
        if (!_locations.ContainsKey(locationId))
            throw new InvalidOperationException($"Location {locationId} does not exist.");

        if(_locations[locationId].Address.Equals(newAddress))
            return;

        Raise(new ClubLocationAddressChanged(Id, locationId, 
            new Events.ValueObjects.Address(newAddress.AddressLine1, newAddress.AddressLine2, newAddress.City, newAddress.Postcode, newAddress.County)));
    }

    public void ChangeClubLocationInformation(int locationId, string newLocationInformation)
    {
        ThrowIfIdNotSet();
        if (!_locations.ContainsKey(locationId))
            throw new InvalidOperationException($"Location {locationId} does not exist.");

        Raise(new ClubLocationInformationChanged(Id, locationId, newLocationInformation));
    }

    public void SetClubHomeLocation(int locationId)
    {
        ThrowIfIdNotSet();
        if (!_locations.ContainsKey(locationId))
            throw new InvalidOperationException($"Location {locationId} does not exist.");

        Raise(new ClubHomeLocationSet(Id, locationId));
    }

    public void SetClubMinimumAge(int minimumAge, string validationPolicy)
    {
        ThrowIfIdNotSet();

        if(_minimumAge == minimumAge && _minimumAgeValidationPolicy == validationPolicy)
            return;

        Raise(new ClubMinimumAgeSet(Id, minimumAge, validationPolicy));
    }

    public void SetClubMaximumAge(int maximumAge, string validationPolicy)
    {
        ThrowIfIdNotSet();

        if (_maximumAge == maximumAge && _maximumAgeValidationPolicy == validationPolicy)
            return;

        Raise(new ClubMaximumAgeSet(Id, maximumAge, validationPolicy));
    }

    public void SetClubInsuranceRequirement(bool isRequired, IEnumerable<string> acceptedInsuranceProviders)
    {
        ThrowIfIdNotSet();

        if(_insuranceRequired == isRequired && _acceptedInsuranceProviders.SetEquals(acceptedInsuranceProviders))
            return;

        Raise(new ClubInsuranceRequirementSet(Id, isRequired, acceptedInsuranceProviders));
    }

    public void SetClubGovernmentDocumentValidationRequirement(bool isRequired)
    {
        ThrowIfIdNotSet();

        if(_governmentDocumentValidationRequired == isRequired)
            return;

        Raise(new ClubGovernmentDocumentValidationRequirementSet(Id, isRequired));
    }

    public void AddClubCommitteeMember(Guid pilotId, string role)
    {
        ThrowIfIdNotSet();
        if (_committeeMembers.ContainsKey(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is already a committee member.");

        if(!_pilotMemberships.ContainsKey(pilotId) || !_pilotMemberships[pilotId].IsConfirmed)
            throw new InvalidOperationException($"Pilot {pilotId} must be a confirmed member to be added as a committee member.");

        Raise(new ClubCommitteeMemberAdded(Id, pilotId, role));
    }

    public void RemoveClubCommitteeMember(Guid pilotId)
    {
        ThrowIfIdNotSet();
        if (!_committeeMembers.ContainsKey(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is not a committee member.");

        Raise(new ClubCommitteeMemberRemoved(Id, pilotId));
    }

    public void AddClubMembershipLevel(Name name)
    {
        ThrowIfIdNotSet();

        if(_membershipLevels.Values.Any(ml => ml.Name.Value.Equals(name.Value, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Membership level with name '{name}' already exists.");

        var membershipLevelId = _membershipLevels.Any() ? _membershipLevels.Keys.Max() + 1 : 1;

        Raise(new ClubMembershipLevelAdded(Id, membershipLevelId, name.Value));
    }

    public void RemoveClubMembershipLevel(int membershipLevelId)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        Raise(new ClubMembershipLevelRemoved(Id, membershipLevelId));
    }

    public void RenameClubMembershipLevel(int membershipLevelId, Name newName)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        if(_membershipLevels[membershipLevelId].Name.Value.Equals(newName.Value, StringComparison.OrdinalIgnoreCase))
            return;

        if(_membershipLevels.Values.Any(ml => ml.Name.Value.Equals(newName.Value, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Membership level with name '{newName}' already exists.");

        Raise(new ClubMembershipLevelRenamed(Id, membershipLevelId, newName.Value));
    }

    public void SetClubMembershipLevelAge(int membershipLevelId, int minimumAge, string validationPolicy)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        if(_membershipLevels[membershipLevelId].MinimumAge == minimumAge
            && _membershipLevels[membershipLevelId].AgeValidationPolicy == validationPolicy)
            return;

        Raise(new ClubMembershipLevelAgeSet(Id, membershipLevelId, minimumAge, validationPolicy));
    }

    public void SetClubMembershipLevelMaximumAge(int membershipLevelId, int maximumAge, string validationPolicy)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        if (_membershipLevels[membershipLevelId].MaximumAge == maximumAge
            && _membershipLevels[membershipLevelId].AgeValidationPolicy == validationPolicy)
            return;

        Raise(new ClubMembershipLevelMaximumAgeSet(Id, membershipLevelId, maximumAge, validationPolicy));
    }

    public void AddClubMembershipLevelAnnualPaymentOption(int membershipLevelId, string name, string currency, decimal price)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        var paymentOptionId = _membershipLevels[membershipLevelId].PaymentOptions.Any() ? _membershipLevels[membershipLevelId].PaymentOptions.Keys.Max() + 1 : 1;

        Raise(new ClubMembershipLevelAnnualPaymentOptionAdded(Id, membershipLevelId, paymentOptionId, name, currency, price));
    }

    public void AddClubMembershipLevelMonthlyPaymentOption(int membershipLevelId, string name, int dayOfMonthDue, int paymentInterval, string currency, decimal price)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        var paymentOptionId = _membershipLevels[membershipLevelId].PaymentOptions.Any() ? _membershipLevels[membershipLevelId].PaymentOptions.Keys.Max() + 1 : 1;

        Raise(new ClubMembershipLevelMonthlyPaymentOptionAdded(Id, membershipLevelId, paymentOptionId, name, dayOfMonthDue, paymentInterval, currency, price));
    }

    public void AddClubMembershipLevelSubscriptionPaymentOption(int membershipLevelId, string name, int paymentInterval, string currency, decimal price)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        var paymentOptionId = _membershipLevels[membershipLevelId].PaymentOptions.Any() ? _membershipLevels[membershipLevelId].PaymentOptions.Keys.Max() + 1 : 1;

        Raise(new ClubMembershipLevelSubscriptionPaymentOptionAdded(Id, membershipLevelId, paymentOptionId, name, paymentInterval, currency, price));
    }

    public void RemoveClubMembershipLevelPaymentOption(int membershipLevelId, int paymentOptionId)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");
        if (!_membershipLevels[membershipLevelId].PaymentOptions.ContainsKey(paymentOptionId))
            throw new InvalidOperationException($"Payment option {paymentOptionId} does not exist.");

        Raise(new ClubMembershipLevelPaymentOptionRemoved(Id, membershipLevelId, paymentOptionId));
    }

    public void RenameClubMembershipLevelPaymentOption(int membershipLevelId, int paymentOptionId, string newName)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");
        if (!_membershipLevels[membershipLevelId].PaymentOptions.ContainsKey(paymentOptionId))
            throw new InvalidOperationException($"Payment option {paymentOptionId} does not exist.");

        Raise(new ClubMembershipLevelPaymentOptionRenamed(Id, membershipLevelId, paymentOptionId, newName));
    }

    public void RegisterPilotForClubMembershipLevel(int membershipLevelId, int paymentOptionId, Guid pilotId, Guid registrationId)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");
        if (!_membershipLevels[membershipLevelId].PaymentOptions.ContainsKey(paymentOptionId))
            throw new InvalidOperationException($"Payment option {paymentOptionId} does not exist.");
        if (_pilotMemberships.ContainsKey(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is already registered.");

        Raise(new PilotRegisteredForClubMembershipLevel(Id, membershipLevelId, paymentOptionId, pilotId, registrationId));
    }

    public void ConfirmPilotClubMembership(int membershipLevelId, int paymentOptionId, Guid pilotId, Guid registrationId)
    {
        ThrowIfIdNotSet();
        if (!_pilotMemberships.TryGetValue(pilotId, out var membership))
            throw new InvalidOperationException($"Pilot {pilotId} is not registered.");
        if (membership.IsConfirmed)
            throw new InvalidOperationException($"Pilot {pilotId} membership is already confirmed.");

        Raise(new PilotClubMembershipConfirmed(Id, membershipLevelId, paymentOptionId, pilotId, registrationId));
    }

    public void CancelPilotClubMembership(Guid pilotId)
    {
        ThrowIfIdNotSet();
        if (!_pilotMemberships.ContainsKey(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is not a member.");

        Raise(new PilotClubMembershipCancelled(Id, pilotId));
    }

    public void AddClubRaceTag(string tag, string colour)
    {
        ThrowIfIdNotSet();
        if (_raceTags.ContainsKey(tag))
            throw new InvalidOperationException($"Tag {tag} already exists.");

        Raise(new ClubRaceTagAdded(Id, tag, colour));
    }

    public void RemoveClubRaceTag(string tag)
    {
        ThrowIfIdNotSet();
        if (!_raceTags.ContainsKey(tag))
            throw new InvalidOperationException($"Tag {tag} does not exist.");

        Raise(new ClubRaceTagRemoved(Id, tag));
    }

    // Event handlers
    private void When(ClubFormed e)
    {
        Id = e.ClubId;
        _code = Code.Rehydrate(e.Code);
        _name = Name.Rehydrate(e.Name);
    }

    private void When(ClubDetailsChanged e)
    {
        _code = Code.Rehydrate(e.Code);
        _name = Name.Rehydrate(e.Name);
    }

    private void When(ClubDescriptionSet e)
    {
        _description = e.Description;
    }

    private void When(ClubContactDetailsSet e)
    {
        _phoneNumber = e.PhoneNumber;
        _emailAddress = e.EmailAddress;
    }

    private void When(ClubLocationAdded e)
    {
        _locations[e.LocationId] = new Location
        {
            Name = Name.Rehydrate(e.LocationName),
            Information = e.LocationInformation,
            Address = ValueTypes.Address.Rehydrate(e.Address.AddressLine1, e.Address.AddressLine2, e.Address.City, e.Address.Postcode, e.Address.County)
        };
    }

    private void When(ClubLocationRemoved e)
    {
        _locations.Remove(e.LocationId);
    }

    private void When(ClubLocationRenamed e)
    {
        _locations[e.LocationId].Name = Name.Rehydrate(e.NewLocationName);
    }

    private void When(ClubLocationAddressChanged e)
    {
        _locations[e.LocationId].Address = ValueTypes.Address.Rehydrate(e.NewAddress.AddressLine1, e.NewAddress.AddressLine2, e.NewAddress.City, e.NewAddress.Postcode, e.NewAddress.County);
    }

    private void When(ClubLocationInformationChanged e)
    {
        _locations[e.LocationId].Information = e.NewLocationInformation;
    }

    private void When(ClubHomeLocationSet e)
    {
        _homeLocationId = e.LocationId;
    }

    private void When(ClubMinimumAgeSet e)
    {
        _minimumAge = e.MinimumAge;
        _minimumAgeValidationPolicy = e.ValidationPolicy;
    }

    private void When(ClubMaximumAgeSet e)
    {
        _maximumAge = e.MaximumAge;
        _maximumAgeValidationPolicy = e.ValidationPolicy;
    }

    private void When(ClubInsuranceRequirementSet e)
    {
        _insuranceRequired = e.IsRequired;
        _acceptedInsuranceProviders.Clear();
        foreach (var provider in e.AcceptedInsuranceProviders)
        {
            _acceptedInsuranceProviders.Add(provider);
        }
    }

    private void When(ClubGovernmentDocumentValidationRequirementSet e)
    {
        _governmentDocumentValidationRequired = e.IsRequired;
    }

    private void When(ClubCommitteeMemberAdded e)
    {
        _committeeMembers[e.PilotId] = new CommitteeMember
        {
            PilotId = e.PilotId,
            Role = e.Role
        };
    }

    private void When(ClubCommitteeMemberRemoved e)
    {
        _committeeMembers.Remove(e.PilotId);
    }

    private void When(ClubMembershipLevelAdded e)
    {
        _membershipLevels[e.MembershipLevelId] = new MembershipLevel
        {
            Name = Name.Rehydrate(e.Name)
        };
    }

    private void When(ClubMembershipLevelRemoved e)
    {
        _membershipLevels.Remove(e.MembershipLevelId);
    }

    private void When(ClubMembershipLevelRenamed e)
    {
        _membershipLevels[e.MembershipLevelId].Name = Name.Rehydrate(e.NewName);
    }

    private void When(ClubMembershipLevelAgeSet e)
    {
        _membershipLevels[e.MembershipLevelId].MinimumAge = e.MinimumAge;
    }

    private void When(ClubMembershipLevelMaximumAgeSet e)
    {
        _membershipLevels[e.MembershipLevelId].MaximumAge = e.MaximumAge;
    }

    private void When(ClubMembershipLevelAnnualPaymentOptionAdded e)
    {
        _membershipLevels[e.MembershipLevelId].PaymentOptions[e.PaymentOptionId] = new PaymentOption
        {
            Name = Name.Rehydrate(e.Name)
        };
    }

    private void When(ClubMembershipLevelMonthlyPaymentOptionAdded e)
    {
        _membershipLevels[e.MembershipLevelId].PaymentOptions[e.PaymentOptionId] = new PaymentOption
        {
            Name = Name.Rehydrate(e.Name)
        };
    }

    private void When(ClubMembershipLevelSubscriptionPaymentOptionAdded e)
    {
        _membershipLevels[e.MembershipLevelId].PaymentOptions[e.PaymentOptionId] = new PaymentOption
        {
            Name = Name.Rehydrate(e.Name)
        };
    }

    private void When(ClubMembershipLevelPaymentOptionRemoved e)
    {
        _membershipLevels[e.MembershipLevelId].PaymentOptions.Remove(e.PaymentOptionId);
    }

    private void When(ClubMembershipLevelPaymentOptionRenamed e)
    {
        _membershipLevels[e.MembershipLevelId].PaymentOptions[e.PaymentOptionId].Name = Name.Rehydrate(e.NewName);
    }

    private void When(PilotRegisteredForClubMembershipLevel e)
    {
        _pilotMemberships[e.PilotId] = new PilotMembership
        {
            MembershipLevelId = e.MembershipLevelId,
            PaymentOptionId = e.PaymentOptionId,
            RegistrationId = e.RegistrationId,
            IsConfirmed = false
        };
    }

    private void When(PilotClubMembershipConfirmed e)
    {
        _pilotMemberships[e.PilotId].IsConfirmed = true;
    }

    private void When(PilotClubMembershipCancelled e)
    {
        _pilotMemberships.Remove(e.PilotId);
    }

    private void When(ClubRaceTagAdded e)
    {
        _raceTags[e.Tag] = e.Colour;
    }

    private void When(ClubRaceTagRemoved e)
    {
        _raceTags.Remove(e.Tag);
    }
}
