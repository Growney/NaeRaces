using EventDbLite.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore;

public class NaeRacesEntityFrameworkSnapshotRepository : ISnapshotRepository
{
    private readonly NaeRacesQueryDbContext _dbContext;
    private readonly IEventSerializer _eventSerializer;

    private class EntityFrameworkSnapshot : IReadSnapshot
    {
        public required byte[] Data { get; set; }
        public required string Identifier { get; set; }
        public required Position Position { get; set; }
        public required StreamPosition StreamPosition { get; set; }
    }

    public NaeRacesEntityFrameworkSnapshotRepository(NaeRacesQueryDbContext dbContext, IEventSerializer eventSerializer)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
    }
    public IAsyncEnumerable<IReadSnapshot> GetSnapshots(string snapshotKey)
    {
        return _dbContext.Snapshots.Where(x => x.SnapshotKey == snapshotKey)
            .OrderByDescending(x => x.StreamPosition)
            .ThenByDescending(x => x.CommitPosition)
            .ThenByDescending(x => x.PreparePosition)
            .AsAsyncEnumerable()
            .Select(x => new EntityFrameworkSnapshot()
            {
                Data = x.Data,
                Identifier = x.Identifier,
                Position = new Position(x.CommitPosition, x.PreparePosition),
                StreamPosition = new StreamPosition(x.StreamPosition)
            });
    }

    public async Task StoreSnapshot(string snapshotKey, object data, string identifier, Position position, StreamPosition streamPosition)
    {
        try
        {
            await _dbContext.Database.BeginTransactionAsync();

            if(await _dbContext.Snapshots.Where(x=>x.SnapshotKey == snapshotKey).AnyAsync())
            {
                ulong maxStreamPosition = await _dbContext.Snapshots.Where(x => x.SnapshotKey == snapshotKey).MaxAsync(x => x.StreamPosition);
                ulong maxPrepare = await _dbContext.Snapshots.Where(x => x.SnapshotKey == snapshotKey).MaxAsync(x => x.PreparePosition);
                ulong maxCommit = await _dbContext.Snapshots.Where(x => x.SnapshotKey == snapshotKey).MaxAsync(x => x.CommitPosition);

                ulong maxCurrent = Math.Max(Math.Max(maxStreamPosition, maxPrepare), maxCommit);

                ulong newMax = Math.Max(Math.Max(streamPosition.Position, position.PreparePosition), position.CommitPosition);

                if (newMax <= maxCurrent)
                {
                    return;
                }
            }

            byte[] serialized = _eventSerializer.SerializeEvent(data);

            _dbContext.Snapshots.Add(new Models.ProjectionSnapshot()
            {
                Id = Guid.NewGuid(),
                SnapshotKey = snapshotKey,
                Data = serialized,
                Identifier = identifier,
                StreamPosition = streamPosition.Position,
                CommitPosition = position.CommitPosition,
                PreparePosition = position.PreparePosition
            });

            await _dbContext.SaveChangesAsync();
        }
        finally
        {
            await _dbContext.Database.CommitTransactionAsync();
        }
    }

    public object? DeserializeSnapshot(IReadSnapshot snapshot, Type targetType)
    {
        if(snapshot is not EntityFrameworkSnapshot entityFrameworkSnapshot)
        {
            throw new NotSupportedException("Snapshot type not supported");
        }

        return _eventSerializer.DeserializeEvent(entityFrameworkSnapshot.Data, targetType);
    }
}
