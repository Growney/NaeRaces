using Microsoft.Extensions.DependencyInjection;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.EntityFrameworkCore.Projections;
using NaeRaces.Query.EntityFrameworkCore.QueryHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.EntityFrameworkCore.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddNaeRacesEntityFrameworkCoreQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IClubUniquenessQueryHandler, ClubUniquenessQueryHandler>();
        services.AddScoped<IPilotDetailsQueryHandler, PilotDetailsQueryHandler>();
        services.AddScoped<IClubDetailsQueryHandler, ClubDetailsQueryHandler>();
        services.AddScoped<IClubMemberQueryHandler, ClubMemberQueryHandler>();
        services.AddScoped<IClubMembershipLevelQueryHandler, ClubMembershipLevelQueryHandler>();
        services.AddScoped<ITeamMemberQueryHandler, TeamMemberQueryHandler>();
        services.AddScoped<IPilotFollowedClubQueryHandler, PilotFollowedClubQueryHandler>();
        services.AddScoped<IPilotClubDetailsQueryHandler, PilotClubDetailsQueryHandler>();
        services.AddScoped<IClubLocationQueryHandler, ClubLocationQueryHandler>();
        services.AddScoped<IPilotValidationQueryHandler, PilotValidationQueryHandler>();
        services.AddScoped<IRaceDetailsQueryHandler, RaceDetailsQueryHandler>();
        services.AddScoped<IRaceInformationQueryHandler, RaceInformationQueryHandler>();
        services.AddScoped<IRaceDiscountQueryHandler, RaceDiscountQueryHandler>();
        services.AddScoped<IRacePackageQueryHandler, RacePackageQueryHandler>();
        services.AddScoped<IPilotSelectionPolicyQueryHandler, PilotSelectionPolicyQueryHandler>();
        services.AddScoped<IPilotPolicyValidationQueryHandler, PilotPolicyValidationQueryHandler>();
        services.AddScoped<IPilotRegistrationQueryHandler, PilotRegistrationQueryHandler>();
        services.AddScoped<IClubOverviewQueryHandler, ClubOverviewQueryHandler>();
        services.AddScoped<INaeRacesQueryContext, NaeRacesQueryContext>();

        services.AddTransient<IClock, Clock>();

        return services;

    }

    public static IServiceCollection AddNaeRacesEntityFrameworkCoreQueryReactions(this IServiceCollection services)
    {
        services.AddEventDbLite();
        services.AddConstantReactionPositionStorage<NaeRacesDbContextConstantReactionPositionStorage>();
        services.AddConstantReactionClass<PilotDetailsProjection>();
        services.AddConstantReactionClass<PilotFollowedClubProjection>();
        services.AddConstantReactionClass<ClubUniquenessProjection>();
        services.AddConstantReactionClass<ClubDetailsProjection>();
        services.AddConstantReactionClass<PilotClubDetailsProjection>();
        services.AddConstantReactionClass<ClubMembershipProjection>();
        services.AddConstantReactionClass<ClubMembershipLevelProjection>();
        services.AddConstantReactionClass<ClubLocationProjection>();
        services.AddConstantReactionClass<RacePackageProjection>();
        services.AddConstantReactionClass<RaceInformationProjection>();
        services.AddConstantReactionClass<RaceDetailsProjection>();
        services.AddConstantReactionClass<RaceDiscountProjection>();
        services.AddConstantReactionClass<PilotRaceRegistrationProjection>();
        services.AddConstantReactionClass<PilotValidationProjection>();
        services.AddConstantReactionClass<PilotSelectionPolicyDetailsProjection>();
        services.AddConstantReactionClass<TeamMemberProjection>();
        services.AddConstantReactionClass<ClubOverviewProjection>();

        return services;
    }
}
