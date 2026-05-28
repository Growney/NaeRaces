using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class ProjectionSnapshot
{
    public required Guid Id { get; init; }
    public required string SnapshotKey { get; init; }
    public required byte[] Data { get; init; }
    public required string Identifier { get; init; }
    public required ulong StreamPosition { get; init; }
    public required ulong CommitPosition { get; init; }
    public required ulong PreparePosition { get; init; }
}
