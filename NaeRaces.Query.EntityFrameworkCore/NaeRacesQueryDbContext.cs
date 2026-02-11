using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore;

public class NaeRacesQueryDbContext : DbContext
{
    public NaeRacesQueryDbContext(DbContextOptions<NaeRacesQueryDbContext> options) : base(options)
    {
    }
    public DbSet<ReactionPosition> ReactionPositions => Set<ReactionPosition>();
    public DbSet<ClubUniquenessDetails> ClubUniquenessDetails => Set<ClubUniquenessDetails>();
    public DbSet<ClubDetails> ClubDetails => Set<ClubDetails>();
    public DbSet<PilotDetails> PilotDetails => Set<PilotDetails>();
    public DbSet<ClubMember> ClubMembers => Set<ClubMember>();
}
