using Microsoft.EntityFrameworkCore;
using NaeRaces.Events;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NaeRaces.Query.EntityFrameworkCore.Projections;

public class ClubMembershipProjection
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public ClubMembershipProjection(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    private Task When(PilotRegisteredForClubMembershipLevel e)
    {
        ClubMember member = new()
        {
            Id = e.RegistrationId,
            ClubId = e.ClubId,
            PilotId = e.PilotId,
            MembershipLevelId = e.MembershipLevelId,
            PaymentOptionId = e.PaymentOptionId,
            IsOnCommittee = false,
            IsRegistrationConfirmed = false
        };

        _dbContext.ClubMembers.Add(member);

        return _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotClubMembershipConfirmed e)
    {
        ClubMember? member = await _dbContext.ClubMembers
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.PilotId == e.PilotId);
        
        if (member != null)
        {
            member.IsRegistrationConfirmed = true;
            member.RegistrationValidUntil = e.ValidUntil;
            member.RegistrationValidatedBy = null;
        }
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotClubMembershipManuallyConfirmed e)
    {
        ClubMember? member = await _dbContext.ClubMembers
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.PilotId == e.PilotId);
        
        if (member != null)
        {
            member.IsRegistrationConfirmed = true;
            member.RegistrationValidUntil = e.ValidUntil;
            member.RegistrationValidatedBy = e.ConfirmedBy;
        }
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotClubMembershipCancelled e)
    {
        ClubMember? member = await _dbContext.ClubMembers
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.PilotId == e.PilotId);
        
        if (member != null)
        {
            _dbContext.ClubMembers.Remove(member);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(PilotClubMembershipRevoked e)
    {
        ClubMember? member = await _dbContext.ClubMembers
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.PilotId == e.PilotId);
        
        if (member != null)
        {
            _dbContext.ClubMembers.Remove(member);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubCommitteeMemberAdded e)
    {
        ClubMember? member = await _dbContext.ClubMembers
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.PilotId == e.PilotId);
        
        if (member != null)
        {
            member.IsOnCommittee = true;
        }
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task When(ClubCommitteeMemberRemoved e)
    {
        ClubMember? member = await _dbContext.ClubMembers
            .SingleOrDefaultAsync(x => x.ClubId == e.ClubId && x.PilotId == e.PilotId);
        
        if (member != null)
        {
            member.IsOnCommittee = false;
        }
        
        await _dbContext.SaveChangesAsync();
    }
}
