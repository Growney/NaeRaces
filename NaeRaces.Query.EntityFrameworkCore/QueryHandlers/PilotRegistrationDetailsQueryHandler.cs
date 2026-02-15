using EventDbLite.Abstractions;
using EventDbLite.Aggregates;
using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using NaeRaces.Query.Projections;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class PilotRegistrationDetailsQueryHandler : IPilotRegistrationDetailsQueryHandler
{
    private readonly IPilotPolicyValidationQueryHandler _pilotPolicyValidationQueryHandler;
    private readonly IProjectionProvider _projectionProvider;
    private readonly IClubMemberQueryHandler _clubMemberQueryHandler;
    private readonly NaeRacesQueryDbContext _dbContext;

    public PilotRegistrationDetailsQueryHandler(
        IPilotPolicyValidationQueryHandler pilotPolicyValidationQueryHandler,
        IProjectionProvider projectionProvider,
        IClubMemberQueryHandler clubMemberQueryHandler,
        NaeRacesQueryDbContext dbContext)
    {
        _pilotPolicyValidationQueryHandler = pilotPolicyValidationQueryHandler ?? throw new ArgumentNullException(nameof(pilotPolicyValidationQueryHandler));
        _projectionProvider = projectionProvider ?? throw new ArgumentNullException(nameof(projectionProvider));
        _clubMemberQueryHandler = clubMemberQueryHandler ?? throw new ArgumentNullException(nameof(clubMemberQueryHandler));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<PilotRegistrationDetails?> GetPilotRegistrationDetails(Guid pilotId, Guid raceId, string currency, DateTime onDate)
    {
        var raceDetails = await _dbContext.RaceDetails.SingleOrDefaultAsync(x => x.Id == raceId);
        if (raceDetails == null)
        {
            return null;
        }

        var streamName = GetRaceStreamName(raceId);
        var race = await _projectionProvider.Load<Race>(streamName, StreamPosition.End);

        var baseCost = race.Costs.TryGetValue(currency, out var cost) ? cost : 0m;

        var details = new PilotRegistrationDetails
        {
            PilotId = pilotId,
            RaceId = raceId,
            Currency = currency,
            BaseCost = baseCost,
            FinalCost = baseCost
        };

        if (race.ValidationPolicyId.HasValue && race.ValidationPolicyVersion.HasValue)
        {
            var validationError = await _pilotPolicyValidationQueryHandler.ValidatePilotAgainstPolicy(
                pilotId,
                race.ValidationPolicyId.Value,
                race.ValidationPolicyVersion.Value,
                onDate);

            details.MeetsValidation = validationError == null;
            details.ValidationError = validationError;
        }
        else
        {
            details.MeetsValidation = true;
        }

        var bestDiscount = await GetBestDiscount(pilotId, race, currency, onDate);
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
        Race race,
        string currency,
        DateTime onDate)
    {
        (Guid PolicyId, long PolicyVersion, decimal DiscountAmount)? bestDiscount = null;

        foreach (var (RacePolicyId, PolicyVersion, Currency, DiscountAmount) in race.Discounts.Where(d => d.Currency == currency))
        {
            var validationError = await _pilotPolicyValidationQueryHandler.ValidatePilotAgainstPolicy(
                pilotId,
                RacePolicyId,
                PolicyVersion,
                onDate);

            if (validationError == null)
            {
                if (!bestDiscount.HasValue || DiscountAmount > bestDiscount.Value.DiscountAmount)
                {
                    bestDiscount = (RacePolicyId, PolicyVersion, DiscountAmount);
                }
            }
        }

        return bestDiscount;
    }

    private static string GetRaceStreamName(Guid raceId) => $"Race-{raceId}";
}
