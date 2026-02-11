using EventDbLite.Abstractions;
using EventDbLite.Reactions.Abstractions;
using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore;

internal class NaeRacesDbContextConstantReactionPositionStorage : IConstantReactionPositionStorage
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public NaeRacesDbContextConstantReactionPositionStorage(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<StreamPosition?> GetPositionAsync(string reactionKey)
    {
        ReactionPosition? reactionPosition = await _dbContext.ReactionPositions.AsNoTracking().SingleOrDefaultAsync(rp => rp.ReactionKey == reactionKey);
        if(reactionPosition == null)
        {
            return null;
        }

        return StreamPosition.WithGlobalVersion(reactionPosition.GlobalPosition);
    }

    public async Task SetPositionAsync(string reactionKey, long globalPosition)
    {
        ReactionPosition? reactionPosition = await _dbContext.ReactionPositions.SingleOrDefaultAsync(rp => rp.ReactionKey == reactionKey);
        if (reactionPosition == null)
        {
            reactionPosition = new ReactionPosition
            {
                ReactionKey = reactionKey,
                GlobalPosition = globalPosition
            };
            _dbContext.ReactionPositions.Add(reactionPosition);
        }
        else
        {
            reactionPosition.GlobalPosition = globalPosition;
        }

        await _dbContext.SaveChangesAsync();
    }
}
