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

    public async Task<Position?> GetPositionAsync(string reactionKey)
    {
        ReactionPosition? reactionPosition = await _dbContext.ReactionPositions.AsNoTracking().SingleOrDefaultAsync(rp => rp.ReactionKey == reactionKey);
        if(reactionPosition == null)
        {
            return null;
        }

        return new Position(reactionPosition.CommitPosition, reactionPosition.PreparePosition);
    }

    public async Task SetPositionAsync(string reactionKey, Position position)
    {
        ReactionPosition? reactionPosition = await _dbContext.ReactionPositions.SingleOrDefaultAsync(rp => rp.ReactionKey == reactionKey);
        if (reactionPosition == null)
        {
            reactionPosition = new ReactionPosition
            {
                ReactionKey = reactionKey,
                CommitPosition = position.CommitPosition,
                PreparePosition = position.PreparePosition
            };
            _dbContext.ReactionPositions.Add(reactionPosition);
        }
        else
        {
            reactionPosition.CommitPosition = position.CommitPosition;
            reactionPosition.PreparePosition = position.PreparePosition;
        }

        await _dbContext.SaveChangesAsync();
    }
}
