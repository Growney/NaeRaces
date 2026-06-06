using Microsoft.Extensions.DependencyInjection;
using NaeRaces.Query.Projections;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddNaeRacesEntityFrameworkCoreQueryReactions(this IServiceCollection services)
    {
        services.AddTrackedAllStreamProjection<PilotRelevantClubsProjection>();
        services.AddTrackedAllStreamProjection<ClubMemberPopularity>();

        return services;
    }
}
