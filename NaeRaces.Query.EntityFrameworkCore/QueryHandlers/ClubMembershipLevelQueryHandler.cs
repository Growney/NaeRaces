using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class ClubMembershipLevelQueryHandler : IClubMembershipLevelQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubMembershipLevelQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ClubMembershipLevel?> GetClubMembershipLevel(Guid clubId, int membershipLevelId)
    {
        var dbLevel = await _dbContext.ClubMembershipLevels
            .Include(x => x.PaymentOptions)
            .SingleOrDefaultAsync(x => x.ClubId == clubId && x.MembershipLevelId == membershipLevelId);

        if (dbLevel == null)
            return null;

        var paymentOptions = dbLevel.PaymentOptions.Select(po => new ClubMembershipLevelPaymentOption(
            po.PaymentOptionId,
            po.Name ?? string.Empty,
            (Query.Models.ClubMembershipLevelPaymentType)po.PaymentType,
            po.Currency ?? string.Empty,
            po.Price,
            po.DayOfMonthDue,
            po.PaymentInterval
        )).ToList();

        return new ClubMembershipLevel(
            dbLevel.ClubId,
            dbLevel.MembershipLevelId,
            dbLevel.Name ?? string.Empty,
            dbLevel.RacePolicyId,
            dbLevel.PolicyVersion,
            paymentOptions
        );
    }

    public IAsyncEnumerable<ClubMembershipLevel> GetClubMembershipLevels(Guid clubId) =>
        _dbContext.ClubMembershipLevels
            .Include(x => x.PaymentOptions)
            .Where(x => x.ClubId == clubId)
            .Select(dbLevel => new ClubMembershipLevel(
                dbLevel.ClubId,
                dbLevel.MembershipLevelId,
                dbLevel.Name ?? string.Empty,
                dbLevel.RacePolicyId,
                dbLevel.PolicyVersion,
                dbLevel.PaymentOptions.Select(po => new ClubMembershipLevelPaymentOption(
                    po.PaymentOptionId,
                    po.Name ?? string.Empty,
                    (Query.Models.ClubMembershipLevelPaymentType)po.PaymentType,
                    po.Currency ?? string.Empty,
                    po.Price,
                    po.DayOfMonthDue,
                    po.PaymentInterval
                ))
            ))
            .ToAsyncEnumerable();
}
