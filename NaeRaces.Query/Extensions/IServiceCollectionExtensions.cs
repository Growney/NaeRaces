using Microsoft.Extensions.DependencyInjection;
using NaeRaces.Query.Projections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddNaeRacesTrackedProjections(this IServiceCollection services)
    {
        services.AddTrackedAllStreamProjection<PilotRelevantClubs>();
        services.AddTrackedAllStreamProjection<PilotProfile>();
        services.AddTrackedAllStreamProjection<ClubMemberPopularity>();
        services.AddTrackedAllStreamProjection<ClubDescription>();
        services.AddTrackedAllStreamProjection<ClubMember>();

        return services;
    }
}
