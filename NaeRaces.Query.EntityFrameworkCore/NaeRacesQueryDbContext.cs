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
    public DbSet<RaceDetails> RaceDetails => Set<RaceDetails>();
    public DbSet<ClubLocation> ClubLocations => Set<ClubLocation>();
    public DbSet<RacePolicyDetails> RacePolicyDetails => Set<RacePolicyDetails>();
    public DbSet<PilotAgeValidation> PilotAgeValidations => Set<PilotAgeValidation>();
    public DbSet<PilotGovernmentDocumentValidation> PilotGovernmentDocumentValidations => Set<PilotGovernmentDocumentValidation>();
    public DbSet<PilotInsuranceProviderValidation> PilotInsuranceProviderValidations => Set<PilotInsuranceProviderValidation>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TeamMember>()
            .HasKey(tm => new { tm.TeamId, tm.PilotId });
    }
}
