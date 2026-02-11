using EventDbLite.Aggregates;
using NaeRaces.Command.ValueTypes;
using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Command.Aggregates;

public class Pilot : AggregateRoot<Guid>
{
    private CallSign _callSign;
    private string? _name;
    private string? _nationality;
    private DateTime? _dateOfBirth;
    private readonly List<GovernmentDocumentValidation> _governmentDocValidations = [];
    private readonly List<InsuranceValidation> _insuranceValidations = [];
    private readonly List<AgeValidation> _dateOfBirthValidations = [];

    private class GovernmentDocumentValidation
    {
        public string GovernmentDocument { get; set; } = string.Empty;
        public Guid? ClubId { get; set; }
        public Guid? ValidatedByPilotId { get; set; }
        public bool IsValidatingMemberOnCommiteeOfClub { get; set; }
        public DateTime ValidUntil { get; set; }
    }

    private class InsuranceValidation
    {
        public string InsuranceProvider { get; set; } = string.Empty;
        public Guid? ClubId { get; set; }
        public Guid? ValidatedByPilotId { get; set; }
        public bool IsValidatingMemberOnCommiteeOfClub { get; set; }
        public DateTime ValidUntil { get; set; }
    }

    private class AgeValidation
    {
        public Guid? ClubId { get; set; }
        public Guid? ValidatedByPilotId { get; set; }
        public bool IsValidatingMemberOnCommiteeOfClub { get; set; }
    }

    public Pilot()
    {
    }

    public Pilot(Guid pilotId, CallSign callSign)
    {
        Raise(new PilotRegistered(pilotId, callSign.Value));
    }

    public void ChangePilotCallSign(CallSign newCallSign)
    {
        ThrowIfIdNotSet();
        if (_callSign.Equals(newCallSign))
            return;

        Raise(new PilotCallSignChanged(Id, newCallSign.Value));
    }
    public void SetPilotName(string name)
    {
        ThrowIfIdNotSet();
        if(_name == name)
            return;

        Raise(new PilotNameSet(Id, name));
    }

    public void SetPilotNationality(string nationality)
    {
        ThrowIfIdNotSet();
        if (_nationality == nationality)
            return;

        Raise(new PilotNationalitySet(Id, nationality));
    }

    public void SetPilotDateOfBirth(DateTime dateOfBirth)
    {
        ThrowIfIdNotSet();
        if (_dateOfBirth == dateOfBirth)
            return;

        Raise(new PilotDateOfBirthSet(Id, dateOfBirth));
    }

    public void PeerValidatePilotGovernmentDocumentation(string governmentDocument, DateTime validUntil, Guid clubId, Guid validatedByPilotId, bool isValidatingMemberOnCommiteeOfClub)
    {
        ThrowIfIdNotSet();

        if(_governmentDocValidations.Any(x => x.ClubId == clubId 
            && x.ValidatedByPilotId == validatedByPilotId 
            && x.GovernmentDocument == governmentDocument 
            && x.ValidUntil > validUntil))
            return; // Already validated by this pilot for this club

        Raise(new PilotGovernmentDocumentationValidatedByPeer(Id, governmentDocument, validUntil, clubId, validatedByPilotId, isValidatingMemberOnCommiteeOfClub));
    }

    public void PeerValidatePilotInsurance(string insuranceProvider, DateTime validUntil, Guid clubId, Guid validatedByPilotId, bool isValidatingMemberOnCommiteeOfClub)
    {
        ThrowIfIdNotSet();

        if(_insuranceValidations.Any(x => x.ClubId == clubId
            && x.ValidatedByPilotId == validatedByPilotId 
            && x.InsuranceProvider == insuranceProvider 
            && x.ValidUntil  >  validUntil))
            return; // Already validated by this pilot for this club

        Raise(new PilotInsuranceValidatedByPeer(Id, insuranceProvider, validUntil, clubId, validatedByPilotId, isValidatingMemberOnCommiteeOfClub));
    }

    public void PeerValidatePilotDateOfBirth(Guid clubId, Guid validatedByPilotId, bool isValidatingMemberOnCommiteeOfClub)
    {
        ThrowIfIdNotSet();

        if (_dateOfBirthValidations.Any(x => x.ClubId == clubId && x.ValidatedByPilotId == validatedByPilotId))
            return; // Already validated by this pilot for this club

        Raise(new PilotDateOfBirthValidatedByPeer(Id, clubId, validatedByPilotId, isValidatingMemberOnCommiteeOfClub));
    }

    // Event handlers
    private void When(PilotRegistered e)
    {
        Id = e.PilotId;
        _callSign = CallSign.Rehydrate(e.CallSign);
    }

    private void When(PilotNameSet e)
    {
        _name = e.Name;
    }

    private void When(PilotCallSignChanged e)
    {
        _callSign = CallSign.Rehydrate(e.NewCallSign);
    }

    private void When(PilotNationalitySet e)
    {
        _nationality = e.Nationality;
    }

    private void When(PilotDateOfBirthSet e)
    {
        _dateOfBirth = e.DateOfBirth;
    }

    private void When(PilotGovernmentDocumentationValidatedByPeer e)
    {
        // Legacy single validation - could map to a special "system" club or remove if no longer used
        _governmentDocValidations.Add(new GovernmentDocumentValidation()
        {
            ClubId = e.ClubId,
            GovernmentDocument = e.GovernmentDocument,
            ValidUntil = e.ValidUntil,
            ValidatedByPilotId = e.ValidatedByPilotId,
            IsValidatingMemberOnCommiteeOfClub = e.IsValidatingMemberOnCommiteeOfClub
        });
    }

    private void When(PilotInsuranceValidatedByPeer e)
    {
        // Legacy single validation - could map to a special "system" club or remove if no longer used
        _insuranceValidations.Add(new InsuranceValidation()
        {
            ClubId = e.ClubId,
            InsuranceProvider = e.InsuranceProvider,
            ValidUntil = e.ValidUntil,
            ValidatedByPilotId = e.ValidatedByPilotId,
            IsValidatingMemberOnCommiteeOfClub = e.IsValidatingMemberOnCommiteeOfClub
        });
    }
    private void When(PilotDateOfBirthValidatedByPeer e)
    {
        _dateOfBirthValidations.Add(new AgeValidation()
        {
            ClubId = e.ClubId,
            ValidatedByPilotId = e.ValidatedByPilotId,
            IsValidatingMemberOnCommiteeOfClub = e.IsValidatingMemberOnCommiteeOfClub
        });
    }
}
