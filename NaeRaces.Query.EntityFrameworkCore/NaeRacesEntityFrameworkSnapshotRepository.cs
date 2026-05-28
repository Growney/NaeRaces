using EventDbLite.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore;

public class NaeRacesEntityFrameworkSnapshotRepository : ISnapshotRepository
{
    private readonly NaeRacesQueryDbContext _dbContext;

    public NaeRacesEntityFrameworkSnapshotRepository(NaeRacesQueryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    public IAsyncEnumerable<Snapshot> GetSnapshots(string snapshotKey)
    {
        return _dbContext.Snapshots.Where(x => x.SnapshotKey == snapshotKey).AsAsyncEnumerable()
            .OrderByDescending(x => new { x.StreamPosition, x.CommitPosition, x.PreparePosition })
            .Select(x => new Snapshot(x.Data, x.Identifier, new Position(x.CommitPosition, x.PreparePosition), new StreamPosition(x.StreamPosition)));
    }

    public async Task StoreSnapshot(string snapshotKey, Snapshot snapshot)
    {
        try
        {
            await _dbContext.Database.BeginTransactionAsync();

            ulong maxStreamPosition = await _dbContext.Snapshots.Where(x => x.SnapshotKey == snapshotKey).MaxAsync(x => x.StreamPosition);
            ulong maxPrepare = await _dbContext.Snapshots.Where(x => x.SnapshotKey == snapshotKey).MaxAsync(x => x.PreparePosition);
            ulong maxCommit = await _dbContext.Snapshots.Where(x => x.SnapshotKey == snapshotKey).MaxAsync(x => x.CommitPosition);

            ulong maxCurrent = Math.Max(Math.Max(maxStreamPosition, maxPrepare), maxCommit);

            ulong newMax = Math.Max(Math.Max(snapshot.StreamPosition.Position, snapshot.Position.PreparePosition), snapshot.Position.CommitPosition);

            if (newMax <= maxCurrent)
            {
                return;
            }

            _dbContext.Snapshots.Add(new Models.ProjectionSnapshot()
            {
                Id = Guid.NewGuid(),
                SnapshotKey = snapshotKey,
                Data = snapshot.Data,
                Identifier = snapshot.Identifier,
                StreamPosition = snapshot.StreamPosition.Position,
                CommitPosition = snapshot.Position.CommitPosition,
                PreparePosition = snapshot.Position.PreparePosition
            });

            await _dbContext.SaveChangesAsync();
        }
        finally
        {
            await _dbContext.Database.CommitTransactionAsync();
        }
    }
}
