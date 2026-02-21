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
    public async Task<PilotRegistrationDetails?> GetPilotRegistrationDetails(Guid pilotId, Guid raceId, string currency)
    {
        var raceDetails = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == raceId);
        if (raceDetails == null)
        {
            return null;
        }

        var lastRaceDate = raceDetails.LastRaceDateEnd;

        var raceCost = await _raceCostQueryHandler.GetRaceCost(raceId, currency);
        var baseCost = raceCost?.Cost ?? 0m;

        var registrationDates = await _raceRegistrationDatesQueryHandler.GetRaceRegistrationDates(raceId);

        bool meetsValidation = true;
        string? validationError = null;

        if (registrationDates?.RaceValidationPolicyId.HasValue == true && registrationDates.RaceValidationPolicyVersion.HasValue)
        {
            validationError = await _pilotPolicyValidationQueryHandler.ValidatePilotAgainstPolicy(
                pilotId,
                registrationDates.RaceValidationPolicyId.Value,
                registrationDates.RaceValidationPolicyVersion.Value,
                lastRaceDate);

            meetsValidation = validationError == null;
        }

        var (validDiscounts, finalCost) = meetsValidation 
            ? await GetValidDiscounts(pilotId, raceCost, lastRaceDate) 
            : (Enumerable.Empty<RaceDiscount>(), baseCost);

        return new PilotRegistrationDetails(
            pilotId,
            raceId,
            meetsValidation,
            validationError,
            currency,
            baseCost,
            finalCost,
            validDiscounts);
    }

    private async Task<(IEnumerable<RaceDiscount> Discounts, decimal FinalCost)> GetValidDiscounts(
        Guid pilotId,
        RaceCost? raceCost,
        DateTime lastRaceDate)
    {
        if (raceCost == null)
        {
            return (Enumerable.Empty<RaceDiscount>(), 0m);
        }

        var baseCost = raceCost.Cost;

        // First, validate all discounts and get the valid ones
        List<RaceCostDiscount> validDiscounts = new();

        foreach (var discount in raceCost.Discounts)
        {
            var validationError = await _pilotPolicyValidationQueryHandler.ValidatePilotAgainstPolicy(
                pilotId, 
                discount.PilotPolicyId, 
                discount.PolicyVersion, 
                lastRaceDate);

            if (validationError == null)
            {
                validDiscounts.Add(discount);
            }
        }

        if (!validDiscounts.Any())
        {
            return (Enumerable.Empty<RaceDiscount>(), baseCost);
        }

        // Calculate the best combination of discounts with running totals in one pass
        return CalculateBestDiscountCombinationWithRunningTotal(baseCost, validDiscounts);
    }

    private (List<RaceDiscount> Discounts, decimal FinalCost) CalculateBestDiscountCombinationWithRunningTotal(
        decimal baseCost,
        List<RaceCostDiscount> validDiscounts)
    {
        var combinableDiscounts = validDiscounts.Where(d => d.CanBeCombined).ToList();
        var nonCombinableDiscounts = validDiscounts.Where(d => !d.CanBeCombined).ToList();

        decimal lowestCost = baseCost;
        List<RaceDiscount> bestDiscounts = new();

        // Option 1: All combinable discounts together
        if (combinableDiscounts.Any())
        {
            var (discounts, finalCost) = CalculateDiscountsWithRunningTotal(baseCost, combinableDiscounts);
            if (finalCost < lowestCost)
            {
                lowestCost = finalCost;
                bestDiscounts = discounts;
            }
        }

        // Option 2: Each non-combinable discount individually
        foreach (var nonCombinable in nonCombinableDiscounts)
        {
            var (discounts, finalCost) = CalculateDiscountsWithRunningTotal(baseCost, new List<RaceCostDiscount> { nonCombinable });
            if (finalCost < lowestCost)
            {
                lowestCost = finalCost;
                bestDiscounts = discounts;
            }
        }

        return (bestDiscounts, lowestCost);
    }

    private (List<RaceDiscount> Discounts, decimal FinalCost) CalculateDiscountsWithRunningTotal(
        decimal baseCost,
        List<RaceCostDiscount> discounts)
    {
        if (!discounts.Any())
        {
            return (new List<RaceDiscount>(), baseCost);
        }

        var fixedDiscounts = discounts.Where(d => !d.IsPercentage).ToList();
        var percentageDiscounts = discounts.Where(d => d.IsPercentage).OrderByDescending(d => d.Discount).ToList();

        List<RaceDiscount> orderedDiscounts = new();
        decimal runningTotal = baseCost;
        int order = 1;

        // Apply fixed-amount discounts first
        foreach (var discount in fixedDiscounts)
        {
            runningTotal = Math.Max(0, runningTotal - discount.Discount);
            orderedDiscounts.Add(new RaceDiscount(
                discount.Name,
                discount.Discount,
                discount.IsPercentage,
                discount.CanBeCombined,
                order++,
                runningTotal
            ));
        }

        // Apply percentage discounts to the running total
        foreach (var discount in percentageDiscounts)
        {
            var discountAmount = runningTotal * (discount.Discount / 100m);
            runningTotal = Math.Max(0, runningTotal - discountAmount);
            orderedDiscounts.Add(new RaceDiscount(
                discount.Name,
                discount.Discount,
                discount.IsPercentage,
                discount.CanBeCombined,
                order++,
                runningTotal
            ));
        }

        return (orderedDiscounts, runningTotal);
    }
}
