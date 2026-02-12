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
    private ValidationPolicy? _minimumAgeValidationPolicy;
    private ValidationPolicy? _maximumAgeValidationPolicy;
    private readonly Dictionary<string, ValidationPolicy> _insuranceProviderRequirements = [];
    private readonly Dictionary<string, ValidationPolicy> _governmentDocumentRequirements = [];
    private readonly Dictionary<Guid, CommitteeMember> _committeeMembers = [];
    private readonly Dictionary<int, MembershipLevel> _membershipLevels = [];
    private readonly Dictionary<Guid, PilotMembership> _pilotMemberships = [];
    private readonly Dictionary<Tag, string> _raceTags = [];

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
        public ValidationPolicy? MinAgeValidationPolicy { get; set; }
        public ValidationPolicy? MaxAgeValidationPolicy { get; set; }
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
    public Club()
    {
        // Parameterless constructor for framework
    }

    public Club(Guid clubId, Code code, Name name, Guid founderPilotId)
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

    public int AddClubLocation(string locationName, string locationInformation, ValueTypes.Address address)
    {
        ThrowIfIdNotSet();

        var locationId = _locations.Any() ? _locations.Keys.Max() + 1 : 1;

        Raise(new ClubLocationAdded(Id, locationId, locationName, locationInformation,
            new Events.ValueObjects.Address(address.AddressLine1, address.AddressLine2, address.City, address.Postcode, address.County)));

        return locationId;
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

        if (_locations[locationId].Name.Equals(newLocationName))
            return;

        Raise(new ClubLocationRenamed(Id, locationId, newLocationName.Value));
    }

    public void ChangeClubLocationAddress(int locationId, ValueTypes.Address newAddress)
    {
        ThrowIfIdNotSet();
        if (!_locations.ContainsKey(locationId))
            throw new InvalidOperationException($"Location {locationId} does not exist.");

        if (_locations[locationId].Address.Equals(newAddress))
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

    public void SetClubMinimumAge(int minimumAge, ValidationPolicy validationPolicy)
    {
        ThrowIfIdNotSet();

        if (_minimumAge == minimumAge && _minimumAgeValidationPolicy.Equals(validationPolicy))
            return;

        Raise(new ClubMinimumAgeRequirementSet(Id, minimumAge, validationPolicy.Value));
    }

    public void RemoveClubMinimumAge()
    {
        ThrowIfIdNotSet();

        if (!_minimumAge.HasValue)
            return;

        Raise(new ClubMinimumAgeRequirementRemoved(Id));
    }

    public void SetClubMaximumAge(int maximumAge, ValidationPolicy validationPolicy)
    {
        ThrowIfIdNotSet();

        if (_maximumAge == maximumAge && _maximumAgeValidationPolicy.Equals(validationPolicy))
            return;

        Raise(new ClubMaximumAgeRequirementSet(Id, maximumAge, validationPolicy.Value));
    }

    public void RemoveClubMaximumAge()
    {
        ThrowIfIdNotSet();

        if (!_maximumAge.HasValue)
            return;

        Raise(new ClubMaximumAgeRequirementRemoved(Id));
    }

    public void AddClubInsuranceProviderRequirement(string insuranceProvider, ValidationPolicy validationPolicy)
    {
        ThrowIfIdNotSet();

        if (_insuranceProviderRequirements.ContainsKey(insuranceProvider))
            throw new InvalidOperationException($"Insurance provider requirement for {insuranceProvider} already exists.");

        Raise(new ClubInsuranceProviderRequirementAdded(Id, insuranceProvider, validationPolicy.Value));
    }

    public void RemoveClubInsuranceProviderRequirement(string insuranceProvider)
    {
        ThrowIfIdNotSet();

        if (!_insuranceProviderRequirements.ContainsKey(insuranceProvider))
            throw new InvalidOperationException($"Insurance provider requirement for {insuranceProvider} does not exist.");

        Raise(new ClubInsuranceProviderRequirementRemoved(Id, insuranceProvider));
    }

    public void AddClubGovernmentDocumentValidationRequirement(string governmentDocument, ValidationPolicy validationPolicy)
    {
        ThrowIfIdNotSet();

        if (_governmentDocumentRequirements.ContainsKey(governmentDocument))
            throw new InvalidOperationException($"Government document requirement for {governmentDocument} already exists.");

        Raise(new ClubGovernmentDocumentValidationRequirementAdded(Id, governmentDocument, validationPolicy.Value));
    }

    public void RemoveClubGovernmentDocumentValidationRequirement(string governmentDocument)
    {
        ThrowIfIdNotSet();

        if (!_governmentDocumentRequirements.ContainsKey(governmentDocument))
            throw new InvalidOperationException($"Government document requirement for {governmentDocument} does not exist.");

        Raise(new ClubGovernmentDocumentValidationRequirementRemoved(Id, governmentDocument));
    }

    public void AddClubCommitteeMember(Guid pilotId, string role)
    {
        ThrowIfIdNotSet();
        if (_committeeMembers.ContainsKey(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is already a committee member.");

        if (!_pilotMemberships.ContainsKey(pilotId) || !_pilotMemberships[pilotId].IsConfirmed)
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

        if (_membershipLevels.Values.Any(ml => ml.Name.Value.Equals(name.Value, StringComparison.OrdinalIgnoreCase)))
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

        if (_membershipLevels[membershipLevelId].Name.Value.Equals(newName.Value, StringComparison.OrdinalIgnoreCase))
            return;

        if (_membershipLevels.Values.Any(ml => ml.Name.Value.Equals(newName.Value, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Membership level with name '{newName}' already exists.");

        Raise(new ClubMembershipLevelRenamed(Id, membershipLevelId, newName.Value));
    }

    public void SetClubMembershipLevelAge(int membershipLevelId, int minimumAge, ValidationPolicy validationPolicy)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        var existingPolicy = _membershipLevels[membershipLevelId];

        if (existingPolicy.MinimumAge == minimumAge
            && existingPolicy.MinAgeValidationPolicy.Equals(validationPolicy))
            return;

        Raise(new ClubMembershipLevelMinimumAgeSet(Id, membershipLevelId, minimumAge, validationPolicy.Value));
    }

    public void SetClubMembershipLevelMaximumAge(int membershipLevelId, int maximumAge, ValidationPolicy validationPolicy)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        if (_membershipLevels[membershipLevelId].MaximumAge == maximumAge
            && _membershipLevels[membershipLevelId].MaxAgeValidationPolicy.Equals(validationPolicy))
            return;

        Raise(new ClubMembershipLevelMaximumAgeSet(Id, membershipLevelId, maximumAge, validationPolicy.Value));
    }

    public void AddClubMembershipLevelAnnualPaymentOption(int membershipLevelId, Name name, string currency, decimal price)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        var paymentOptionId = _membershipLevels[membershipLevelId].PaymentOptions.Any() ? _membershipLevels[membershipLevelId].PaymentOptions.Keys.Max() + 1 : 1;

        Raise(new ClubMembershipLevelAnnualPaymentOptionAdded(Id, membershipLevelId, paymentOptionId, name.Value, currency, price));
    }

    public void AddClubMembershipLevelMonthlyPaymentOption(int membershipLevelId, Name name, int dayOfMonthDue, int paymentInterval, string currency, decimal price)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        var paymentOptionId = _membershipLevels[membershipLevelId].PaymentOptions.Any() ? _membershipLevels[membershipLevelId].PaymentOptions.Keys.Max() + 1 : 1;

        Raise(new ClubMembershipLevelMonthlyPaymentOptionAdded(Id, membershipLevelId, paymentOptionId, name.Value, dayOfMonthDue, paymentInterval, currency, price));
    }

    public void AddClubMembershipLevelSubscriptionPaymentOption(int membershipLevelId, Name name, int paymentInterval, string currency, decimal price)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");

        var paymentOptionId = _membershipLevels[membershipLevelId].PaymentOptions.Any() ? _membershipLevels[membershipLevelId].PaymentOptions.Keys.Max() + 1 : 1;

        Raise(new ClubMembershipLevelSubscriptionPaymentOptionAdded(Id, membershipLevelId, paymentOptionId, name.Value, paymentInterval, currency, price));
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

    public void RenameClubMembershipLevelPaymentOption(int membershipLevelId, int paymentOptionId, Name newName)
    {
        ThrowIfIdNotSet();
        if (!_membershipLevels.ContainsKey(membershipLevelId))
            throw new InvalidOperationException($"Membership level {membershipLevelId} does not exist.");
        if (!_membershipLevels[membershipLevelId].PaymentOptions.ContainsKey(paymentOptionId))
            throw new InvalidOperationException($"Payment option {paymentOptionId} does not exist.");

        Raise(new ClubMembershipLevelPaymentOptionRenamed(Id, membershipLevelId, paymentOptionId, newName.Value));
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

    public void ConfirmPilotClubMembership(int membershipLevelId, int paymentOptionId, Guid pilotId, Guid registrationId, DateTime validUntil)
    {
        ThrowIfIdNotSet();
        if (!_pilotMemberships.TryGetValue(pilotId, out var membership))
            throw new InvalidOperationException($"Pilot {pilotId} is not registered.");
        if (membership.IsConfirmed)
            throw new InvalidOperationException($"Pilot {pilotId} membership is already confirmed.");

        Raise(new PilotClubMembershipConfirmed(Id, membershipLevelId, paymentOptionId, pilotId, registrationId, validUntil));
    }
    public void ManuallyConfirmPilotClubMembership(int membershipLevelId, int paymentOptionId, Guid pilotId, Guid registrationId, Guid confirmedByPilotId, DateTime validUntil)
    {
        ThrowIfIdNotSet();
        if (!_pilotMemberships.TryGetValue(pilotId, out var membership))
            throw new InvalidOperationException($"Pilot {pilotId} is not registered.");
        if (membership.IsConfirmed)
            throw new InvalidOperationException($"Pilot {pilotId} membership is already confirmed.");

        if (!_committeeMembers.ContainsKey(confirmedByPilotId))
            throw new InvalidOperationException($"Pilot {confirmedByPilotId} is not a committee member.");

        Raise(new PilotClubMembershipManuallyConfirmed(Id, membershipLevelId, paymentOptionId, pilotId, registrationId, validUntil, confirmedByPilotId));
    }
    public void CancelPilotClubMembership(Guid pilotId)
    {
        ThrowIfIdNotSet();
        if (!_pilotMemberships.ContainsKey(pilotId))
            throw new InvalidOperationException($"Pilot {pilotId} is not a member.");

        Raise(new PilotClubMembershipCancelled(Id, pilotId));
    }

    public void AddClubRaceTag(Tag tag, string colour)
    {
        ThrowIfIdNotSet();
        if (_raceTags.ContainsKey(tag))
            throw new InvalidOperationException($"Tag {tag} already exists.");

        Raise(new ClubRaceTagAdded(Id, tag.Value, colour));
    }

    public void RemoveClubRaceTag(Tag tag)
    {
        ThrowIfIdNotSet();
        if (!_raceTags.ContainsKey(tag))
            throw new InvalidOperationException($"Tag {tag} does not exist.");

        Raise(new ClubRaceTagRemoved(Id, tag.Value));
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

    private void When(ClubMinimumAgeRequirementSet e)
    {
        _minimumAge = e.MinimumAge;
        _minimumAgeValidationPolicy = ValidationPolicy.Rehydrate(e.ValidationPolicy);
    }

    private void When(ClubMinimumAgeRequirementRemoved e)
    {
        _minimumAge = null;
        _minimumAgeValidationPolicy = null;
    }

    private void When(ClubMaximumAgeRequirementSet e)
    {
        _maximumAge = e.MaximumAge;
        _maximumAgeValidationPolicy = ValidationPolicy.Rehydrate(e.ValidationPolicy);
    }

    private void When(ClubMaximumAgeRequirementRemoved e)
    {
        _maximumAge = null;
        _maximumAgeValidationPolicy = null;
    }

    private void When(ClubInsuranceProviderRequirementAdded e)
    {
        _insuranceProviderRequirements[e.InsuranceProvider] = ValidationPolicy.Rehydrate(e.ValidationPolicy);
    }

    private void When(ClubInsuranceProviderRequirementRemoved e)
    {
        _insuranceProviderRequirements.Remove(e.InsuranceProvider);
    }

    private void When(ClubGovernmentDocumentValidationRequirementAdded e)
    {
        _governmentDocumentRequirements[e.GovernmentDocument] = ValidationPolicy.Rehydrate(e.ValidationPolicy);
    }

    private void When(ClubGovernmentDocumentValidationRequirementRemoved e)
    {
        _governmentDocumentRequirements.Remove(e.GovernmentDocument);
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

    private void When(ClubMembershipLevelMinimumAgeSet e)
    {
        _membershipLevels[e.MembershipLevelId].MinimumAge = e.MinimumAge;
        _membershipLevels[e.MembershipLevelId].MinAgeValidationPolicy = ValidationPolicy.Rehydrate(e.ValidationPolicy);

    }

    private void When(ClubMembershipLevelMaximumAgeSet e)
    {
        _membershipLevels[e.MembershipLevelId].MaximumAge = e.MaximumAge;
        _membershipLevels[e.MembershipLevelId].MaxAgeValidationPolicy = ValidationPolicy.Rehydrate(e.ValidationPolicy);
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
        _raceTags[Tag.Rehydrate(e.Tag)] = e.Colour;
    }

    private void When(ClubRaceTagRemoved e)
    {
        _raceTags.Remove(Tag.Rehydrate(e.Tag));
    }
}
