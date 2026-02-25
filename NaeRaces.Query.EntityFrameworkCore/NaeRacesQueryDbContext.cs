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
    public DbSet<PilotSelectionPolicyDetails> PilotSelectionPolicyDetails => Set<PilotSelectionPolicyDetails>();
    public DbSet<PilotAgeValidation> PilotAgeValidations => Set<PilotAgeValidation>();
    public DbSet<PilotGovernmentDocumentValidation> PilotGovernmentDocumentValidations => Set<PilotGovernmentDocumentValidation>();
    public DbSet<PilotInsuranceProviderValidation> PilotInsuranceProviderValidations => Set<PilotInsuranceProviderValidation>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<ClubMembershipLevel> ClubMembershipLevels => Set<ClubMembershipLevel>();
    public DbSet<RacePackage> RacePackages => Set<RacePackage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TeamMember>()
            .HasKey(tm => new { tm.TeamId, tm.PilotId });

        modelBuilder.Entity<ClubMembershipLevel>()
            .HasKey(cml => new { cml.ClubId, cml.MembershipLevelId });

        modelBuilder.Entity<RacePackage>()
            .HasKey(rp => new { rp.RaceId, rp.RacePackageId });

        modelBuilder.Entity<ClubMembershipLevelPaymentOption>()
            .HasKey(po => new { po.ClubId, po.MembershipLevelId, po.PaymentOptionId });

        modelBuilder.Entity<ClubMembershipLevelPaymentOption>()
            .HasOne(po => po.MembershipLevel)
            .WithMany(cml => cml.PaymentOptions)
            .HasForeignKey(po => new { po.ClubId, po.MembershipLevelId });
    }
}
