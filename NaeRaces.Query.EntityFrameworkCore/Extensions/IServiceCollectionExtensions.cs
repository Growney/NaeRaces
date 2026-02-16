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
        services.AddScoped<IClubLocationQueryHandler, ClubLocationQueryHandler>();
        services.AddScoped<IPilotValidationQueryHandler, PilotValidationQueryHandler>();
        services.AddScoped<IRaceDetailsQueryHandler, RaceDetailsQueryHandler>();
        services.AddScoped<IRaceCostQueryHandler, RaceCostQueryHandler>();
        services.AddScoped<IRaceRegistrationDatesQueryHandler, RaceRegistrationDatesQueryHandler>();
        services.AddScoped<IPilotSelectionPolicyQueryHandler, PilotSelectionPolicyQueryHandler>();
        services.AddScoped<IPilotPolicyValidationQueryHandler, PilotPolicyValidationQueryHandler>();
        services.AddScoped<IPilotRegistrationDetailsQueryHandler, PilotRegistrationDetailsQueryHandler>();

        services.AddScoped<INaeRacesQueryContext, NaeRacesQueryContext>();

        return services;

    }

    public static IServiceCollection AddNaeRacesEntityFrameworkCoreQueryReactions(this IServiceCollection services)
    {
        services.AddEventDbLite();
        services.AddConstantReactionPositionStorage<NaeRacesDbContextConstantReactionPositionStorage>();
        services.AddConstantReactionClass<PilotDetailsProjection>();
        services.AddConstantReactionClass<ClubUniquenessProjection>();
        services.AddConstantReactionClass<ClubDetailsProjection>();
        services.AddConstantReactionClass<ClubMembershipProjection>();
        services.AddConstantReactionClass<ClubMembershipLevelProjection>();
        services.AddConstantReactionClass<RaceCostProjection>();

        return services;
    }
}
