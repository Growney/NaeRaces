using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class PilotRegistrationDetailsQueryHandler : IPilotRegistrationDetailsQueryHandler
{
    private readonly IPilotPolicyValidationQueryHandler _pilotPolicyValidationQueryHandler;
    private readonly IRaceCostQueryHandler _raceCostQueryHandler;
    private readonly IRaceRegistrationDatesQueryHandler _raceRegistrationDatesQueryHandler;
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotRegistrationDetailsQueryHandler(
        IPilotPolicyValidationQueryHandler pilotPolicyValidationQueryHandler,
        IRaceCostQueryHandler raceCostQueryHandler,
        IRaceRegistrationDatesQueryHandler raceRegistrationDatesQueryHandler,
        NaeRacesQueryDbContext dbContext)
    {
        _pilotPolicyValidationQueryHandler = pilotPolicyValidationQueryHandler ?? throw new ArgumentNullException(nameof(pilotPolicyValidationQueryHandler));
        _raceCostQueryHandler = raceCostQueryHandler ?? throw new ArgumentNullException(nameof(raceCostQueryHandler));
        _raceRegistrationDatesQueryHandler = raceRegistrationDatesQueryHandler ?? throw new ArgumentNullException(nameof(raceRegistrationDatesQueryHandler));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PilotRegistrationDetails?> GetPilotRegistrationDetails(Guid pilotId, Guid raceId, string currency, DateTime onDate)
    {
        var raceDetails = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == raceId);
        if (raceDetails == null)
        {
            return null;
        }

        var raceCost = await _raceCostQueryHandler.GetRaceCost(raceId, currency);
        var baseCost = raceCost?.Cost ?? 0m;

        var details = new PilotRegistrationDetails
        {
            PilotId = pilotId,
            RaceId = raceId,
            Currency = currency,
            BaseCost = baseCost,
            FinalCost = baseCost
        };

        var registrationDates = await _raceRegistrationDatesQueryHandler.GetRaceRegistrationDates(raceId);
        
        if (registrationDates?.RaceValidationPolicyId.HasValue == true && registrationDates.RaceValidationPolicyVersion.HasValue)
        {
            var validationError = await _pilotPolicyValidationQueryHandler.ValidatePilotAgainstPolicy(
                pilotId,
                registrationDates.RaceValidationPolicyId.Value,
                registrationDates.RaceValidationPolicyVersion.Value,
                onDate);

            details.MeetsValidation = validationError == null;
            details.ValidationError = validationError;
        }
        else
        {
            details.MeetsValidation = true;
        }

        var bestDiscount = await GetBestDiscount(pilotId, raceCost, onDate);
        if (bestDiscount.HasValue)
        {
            details.BestDiscountAmount = bestDiscount.Value.DiscountAmount;
            details.BestDiscountPolicyId = bestDiscount.Value.PolicyId;
            details.BestDiscountPolicyVersion = bestDiscount.Value.PolicyVersion;
            details.FinalCost = Math.Max(0, baseCost - bestDiscount.Value.DiscountAmount);
        }

        return details;
    }

    private async Task<(Guid PolicyId, long PolicyVersion, decimal DiscountAmount)?> GetBestDiscount(
        Guid pilotId,
        RaceCost? raceCost,
        DateTime onDate)
    {
        if (raceCost == null)
        {
            return null;
        }

        (Guid PolicyId, long PolicyVersion, decimal DiscountAmount)? bestDiscount = null;

        foreach (var discount in raceCost.Discounts)
        {
            var validationError = await _pilotPolicyValidationQueryHandler.ValidatePilotAgainstPolicy(
                pilotId,
                discount.PilotPolicyId,
                discount.PolicyVersion,
                onDate);

            if (validationError == null)
            {
                if (!bestDiscount.HasValue || discount.Discount > bestDiscount.Value.DiscountAmount)
                {
                    bestDiscount = (discount.PilotPolicyId, discount.PolicyVersion, discount.Discount);
                }
            }
        }

        return bestDiscount;
    }
}
