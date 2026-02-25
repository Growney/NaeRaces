using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class PilotRegistrationQueryHandler : IPilotRegistrationQueryHandler
{
    private readonly IPilotPolicyValidationQueryHandler _pilotPolicyValidationQueryHandler;
    private readonly IRaceDiscountQueryHandler _raceDiscountQueryHandler;
    private readonly IRaceDetailsQueryHandler _raceDetailsQueryHandler;
    private readonly IRacePackageQueryHandler _racePackageQueryHandler;
    private readonly IClock _clock;

    public PilotRegistrationQueryHandler(IPilotPolicyValidationQueryHandler pilotPolicyValidationQueryHandler, IRaceDiscountQueryHandler raceDiscountHandler, IRaceDetailsQueryHandler raceDetailsQueryHandler, IRacePackageQueryHandler racePackageQueryHandler, IClock clock)
    {
        _pilotPolicyValidationQueryHandler = pilotPolicyValidationQueryHandler ?? throw new ArgumentNullException(nameof(pilotPolicyValidationQueryHandler));
        _raceDiscountQueryHandler = raceDiscountHandler ?? throw new ArgumentNullException(nameof(raceDiscountHandler));
        _raceDetailsQueryHandler = raceDetailsQueryHandler ?? throw new ArgumentNullException(nameof(raceDetailsQueryHandler));
        _racePackageQueryHandler = racePackageQueryHandler ?? throw new ArgumentNullException(nameof(racePackageQueryHandler));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task<IPilotRegistrationQueryHandler.RegistrationStatus> GetPilotPotentialRegistrationStatusForRace(Guid pilotId, Guid raceId, int racePackageId)
    {
        RaceRegistrationDetails? raceDetails = await _raceDetailsQueryHandler.GetRaceRegistrationDetails(raceId);

        if (raceDetails == null)
            throw new InvalidOperationException("Unable to find race details");

        if (raceDetails.PolicyId.HasValue && raceDetails.PolicyVersion.HasValue)
        {
            var policyValidationResult = await _pilotPolicyValidationQueryHandler.ValidatePilotAgainstPolicy(pilotId, raceDetails.PolicyId.Value, raceDetails.PolicyVersion.Value, raceDetails.LastRaceDate);
            if (policyValidationResult != null)
            {
                return IPilotRegistrationQueryHandler.RegistrationStatus.PilotDoesntMeetRaceRequirements;
            }
        }

        var racePackage = await _racePackageQueryHandler.GetRacePackage(raceId, racePackageId);

        if (racePackage == null)
            throw new InvalidOperationException("Unable to find race package");

        if (racePackage.PilotPolicyId.HasValue && racePackage.PolicyVersion.HasValue)
        {
            var policyValidationResult = await _pilotPolicyValidationQueryHandler.ValidatePilotAgainstPolicy(pilotId, racePackage.PilotPolicyId.Value, racePackage.PolicyVersion.Value, raceDetails.LastRaceDate);
            if (policyValidationResult != null)
            {
                return IPilotRegistrationQueryHandler.RegistrationStatus.PilotDoesntMeetRaceRequirements;
            }
        }

        DateTime now = _clock.UtcNow;
        if (!racePackage.IsRegistrationManuallyOpened && (racePackage.RegistrationOpenDate > now || racePackage.RegistrationCloseDate < now))
        {
            return IPilotRegistrationQueryHandler.RegistrationStatus.PackageRegistrationNotOpen;
        }

        return IPilotRegistrationQueryHandler.RegistrationStatus.Success;
    }

    public async Task<PilotRegistrationDetails?> GetPilotRegistrationDetails(Guid pilotId, Guid raceId, int racePackageId)
    {
        var racePackage = await _racePackageQueryHandler.GetRacePackage(raceId, racePackageId);

        if (racePackage == null)
            throw new InvalidOperationException("Unable to find race package");

        List<PilotRaceDiscount> bestDiscounts = new();

        if (racePackage.ApplyDiscounts)
        {

        }

        return new PilotRegistrationDetails(pilotId, raceId, racePackageId, racePackage.Currency, racePackage.Cost, 0, bestDiscounts);
    }
}
