using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NaeRaces.Query.Projections;

public class Race
{
    private readonly Dictionary<string, decimal> _costs = [];
    private readonly Dictionary<int, RaceDiscount> _discounts = [];
    private Guid? _validationPolicyId;
    private long? _validationPolicyVersion;

    private class RaceDiscount
    {
        public Guid RacePolicyId { get; set; }
        public long PolicyVersion { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
    }

    public Guid? ValidationPolicyId => _validationPolicyId;
    public long? ValidationPolicyVersion => _validationPolicyVersion;
    public IReadOnlyDictionary<string, decimal> Costs => _costs;
    public IEnumerable<(Guid RacePolicyId, long PolicyVersion, string Currency, decimal DiscountAmount)> Discounts 
        => _discounts.Values.Select(d => (d.RacePolicyId, d.PolicyVersion, d.Currency, d.DiscountAmount));

    private void When(RacePlanned e)
    {
    }

    private void When(TeamRacePlanned e)
    {
    }

    private void When(RaceValidationPolicySet e)
    {
        _validationPolicyId = e.PolicyId;
        _validationPolicyVersion = e.PolicyVersion;
    }

    private void When(RaceValidationPolicyMigratedToVersion e)
    {
        _validationPolicyVersion = e.PolicyVersion;
    }

    private void When(RaceValidationPolicyRemoved e)
    {
        _validationPolicyId = null;
        _validationPolicyVersion = null;
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

    private void When(RaceDiscountAdded e)
    {
        _discounts[e.RaceDiscountId] = new RaceDiscount
        {
            RacePolicyId = e.RacePolicyId,
            PolicyVersion = e.PolicyVersion,
            Currency = e.Currency,
            DiscountAmount = e.Discount
        };
    }

    private void When(RaceDiscountAddedRemoved e)
    {
        _discounts.Remove(e.RaceDiscountId);
    }
}
