using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.QueryHandlers;

public class ClubMemberQueryHandler : IClubMemberQueryHandler
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubMemberQueryHandler(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IAsyncEnumerable<ClubMember> GetClubMembers(Guid clubId) => _dbContext.ClubMembers
        .Where(x => x.ClubId == clubId).Select(x => new ClubMember(x.Id, x.ClubId, x.PilotId, x.MembershipLevelId, x.PaymentOptionId, x.IsOnCommittee, x.IsRegistrationConfirmed, x.RegistrationValidatedBy, x.RegistrationValidUntil)).ToAsyncEnumerable<ClubMember>();

    public IAsyncEnumerable<ClubMember> GetPilotMembershipDetails(Guid pilotId) => _dbContext.ClubMembers
        .Where(x => x.PilotId == pilotId).Select(x => new ClubMember(x.Id, x.ClubId, x.PilotId, x.MembershipLevelId, x.PaymentOptionId, x.IsOnCommittee, x.IsRegistrationConfirmed, x.RegistrationValidatedBy, x.RegistrationValidUntil)).ToAsyncEnumerable<ClubMember>();


    public Task<bool> HasEverBeenClubMember(Guid clubId, Guid pilotId) => _dbContext.ClubMembers
            .AnyAsync(x => x.ClubId == clubId && x.PilotId == pilotId);

    public Task<bool> IsCurrentlyActiveClubMember(Guid clubId, Guid pilotId) => _dbContext.ClubMembers
            .AnyAsync(x => x.ClubId == clubId && x.PilotId == pilotId && x.IsRegistrationConfirmed);

    public Task<bool> IsOnClubCommittee(Guid clubId, Guid pilotId) => _dbContext.ClubMembers
            .AnyAsync(x => x.ClubId == clubId && x.PilotId == pilotId && x.IsOnCommittee);

}
