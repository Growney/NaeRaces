using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class ClubMembershipLevelProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubMembershipLevelProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(ClubMembershipLevelAdded e)
    {
        ClubMembershipLevel level = new()
        {
            ClubId = e.ClubId,
            MembershipLevelId = e.MembershipLevelId,
            Name = e.Name
        };

        _dbContext.ClubMembershipLevels.Add(level);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubMembershipLevelRemoved e)
    {
        ClubMembershipLevel? level = await _dbContext.ClubMembershipLevels
            .Include(x => x.PaymentOptions)
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.MembershipLevelId == e.MembershipLevelId);

        if (level != null)
        {
            _dbContext.ClubMembershipLevels.Remove(level);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubMembershipLevelRenamed e)
    {
        ClubMembershipLevel? level = await _dbContext.ClubMembershipLevels
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.MembershipLevelId == e.MembershipLevelId);

        if (level != null)
        {
            level.Name = e.NewName;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubMembershipLevelPolicySet e)
    {
        ClubMembershipLevel? level = await _dbContext.ClubMembershipLevels
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.MembershipLevelId == e.MembershipLevelId);

        if (level != null)
        {
            level.PilotPolicyId = e.PilotPolicyId;
            level.PolicyVersion = e.PolicyVersion;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubMembershipLevelPolicyCleared e)
    {
        ClubMembershipLevel? level = await _dbContext.ClubMembershipLevels
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.MembershipLevelId == e.MembershipLevelId);

        if (level != null)
        {
            level.PilotPolicyId = null;
            level.PolicyVersion = null;
        }

        await _dbContext.SaveChangesAsync();
    }

    private Task When(ClubMembershipLevelAnnualPaymentOptionAdded e)
    {
        ClubMembershipLevelPaymentOption paymentOption = new()
        {
            ClubId = e.ClubId,
            MembershipLevelId = e.MembershipLevelId,
            PaymentOptionId = e.PaymentOptionId,
            Name = e.Name,
            PaymentType = ClubMembershipLevelPaymentType.Annual,
            Currency = e.Currency,
            Price = e.Price
        };

        _dbContext.Set<ClubMembershipLevelPaymentOption>().Add(paymentOption);

        return _dbContext.SaveChangesAsync();
    }

    private Task When(ClubMembershipLevelMonthlyPaymentOptionAdded e)
    {
        ClubMembershipLevelPaymentOption paymentOption = new()
        {
            ClubId = e.ClubId,
            MembershipLevelId = e.MembershipLevelId,
            PaymentOptionId = e.PaymentOptionId,
            Name = e.Name,
            PaymentType = ClubMembershipLevelPaymentType.Monthly,
            Currency = e.Currency,
            Price = e.Price,
            DayOfMonthDue = e.DayOfMonthDue,
            PaymentInterval = e.PaymentInterval
        };

        _dbContext.Set<ClubMembershipLevelPaymentOption>().Add(paymentOption);

        return _dbContext.SaveChangesAsync();
    }

    private Task When(ClubMembershipLevelSubscriptionPaymentOptionAdded e)
    {
        ClubMembershipLevelPaymentOption paymentOption = new()
        {
            ClubId = e.ClubId,
            MembershipLevelId = e.MembershipLevelId,
            PaymentOptionId = e.PaymentOptionId,
            Name = e.Name,
            PaymentType = ClubMembershipLevelPaymentType.Subscription,
            Currency = e.Currency,
            Price = e.Price,
            PaymentInterval = e.PaymentInterval
        };

        _dbContext.Set<ClubMembershipLevelPaymentOption>().Add(paymentOption);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubMembershipLevelPaymentOptionRemoved e)
    {
        ClubMembershipLevelPaymentOption? paymentOption = await _dbContext.Set<ClubMembershipLevelPaymentOption>()
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.MembershipLevelId == e.MembershipLevelId && x.PaymentOptionId == e.PaymentOptionId);

        if (paymentOption != null)
        {
            _dbContext.Set<ClubMembershipLevelPaymentOption>().Remove(paymentOption);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubMembershipLevelPaymentOptionRenamed e)
    {
        ClubMembershipLevelPaymentOption? paymentOption = await _dbContext.Set<ClubMembershipLevelPaymentOption>()
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.MembershipLevelId == e.MembershipLevelId && x.PaymentOptionId == e.PaymentOptionId);

        if (paymentOption != null)
        {
            paymentOption.Name = e.NewName;
        }

        await _dbContext.SaveChangesAsync();
    }
}
