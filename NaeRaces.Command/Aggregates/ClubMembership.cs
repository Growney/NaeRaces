using EventDbLite.Aggregates;
using NaeRaces.Events;
using System;

namespace NaeRaces.Command.Aggregates;

public class ClubMembership : AggregateRoot<Guid>
{
    private Guid _clubId;
    private Guid _pilotId;
    private int _membershipLevelId;
    private int _paymentOptionId;
    private bool _isConfirmed;
    private bool _isActive;
    private bool _autoRenew;

    public ClubMembership()
    {
        // Parameterless constructor for framework
    }

    public ClubMembership(Guid registrationId, Guid clubId, int membershipLevelId, int paymentOptionId, Guid pilotId)
    {
        Raise(new PilotRegisteredForClubMembershipLevel(clubId, membershipLevelId, paymentOptionId, pilotId, registrationId));
    }

    public void Confirm(DateTime validUntil)
    {
        ThrowIfIdNotSet();

        if (_isConfirmed)
            throw new InvalidOperationException($"Membership {Id} is already confirmed.");

        Raise(new PilotClubMembershipConfirmed(_clubId, _membershipLevelId, _paymentOptionId, _pilotId, Id, validUntil));
    }

    public void ManuallyConfirm(Guid confirmedByPilotId, DateTime validUntil)
    {
        ThrowIfIdNotSet();

        if (_isConfirmed)
            throw new InvalidOperationException($"Membership {Id} is already confirmed.");

        Raise(new PilotClubMembershipManuallyConfirmed(_clubId, _membershipLevelId, _paymentOptionId, _pilotId, Id, validUntil, confirmedByPilotId));
    }

    public void Cancel()
    {
        ThrowIfIdNotSet();

        if (!_isActive && _isConfirmed)
            return;

        Raise(new PilotClubMembershipCancelled(_clubId, _pilotId, Id));
    }

    public void Revoke(Guid revokedBy)
    {
        ThrowIfIdNotSet();

        if (!_isActive && _isConfirmed)
            return;

        Raise(new PilotClubMembershipRevoked(_clubId, _pilotId, Id, revokedBy));
    }

    public void SetAutoRenewal(bool autoRenew)
    {
        ThrowIfIdNotSet();

        if (!_isConfirmed)
            throw new InvalidOperationException($"Membership {Id} is not confirmed.");
        if (_autoRenew == autoRenew)
            return;

        Raise(new PilotClubMembershipAutoRenewalSet(_clubId, _pilotId, Id, autoRenew));
    }

    public void Renew(DateTime newValidUntil)
    {
        ThrowIfIdNotSet();

        if (!_isConfirmed)
            throw new InvalidOperationException($"Membership {Id} is not confirmed.");

        Raise(new PilotClubMembershipRenewed(_clubId, _pilotId, Id, newValidUntil));
    }

    public void FailRenewal()
    {
        ThrowIfIdNotSet();

        if (!_isConfirmed)
            throw new InvalidOperationException($"Membership {Id} is not confirmed.");

        Raise(new PilotClubMembershipRenewalFailed(_clubId, _pilotId, Id));
    }

    public void Expire()
    {
        ThrowIfIdNotSet();

        if (!_isActive)
            return;

        Raise(new PilotClubMembershipExpired(_clubId, _pilotId, Id));
    }

    // Event handlers
    private void When(PilotRegisteredForClubMembershipLevel e)
    {
        Id = e.RegistrationId;
        _clubId = e.ClubId;
        _pilotId = e.PilotId;
        _membershipLevelId = e.MembershipLevelId;
        _paymentOptionId = e.PaymentOptionId;
        _isConfirmed = false;
        _isActive = false;
        _autoRenew = false;
    }

    private void When(PilotClubMembershipConfirmed e)
    {
        _isConfirmed = true;
        _isActive = true;
    }

    private void When(PilotClubMembershipManuallyConfirmed e)
    {
        _isConfirmed = true;
        _isActive = true;
    }

    private void When(PilotClubMembershipCancelled e)
    {
        _isActive = false;
    }

    private void When(PilotClubMembershipRevoked e)
    {
        _isActive = false;
    }

    private void When(PilotClubMembershipAutoRenewalSet e)
    {
        _autoRenew = e.AutoRenew;
    }

    private void When(PilotClubMembershipRenewed e)
    {
        _isActive = true;
    }

    private void When(PilotClubMembershipRenewalFailed e)
    {
        _isActive = false;
    }

    private void When(PilotClubMembershipExpired e)
    {
        _isActive = false;
    }
}
