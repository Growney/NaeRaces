using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using NaeRaces.Query.EntityFrameworkCore;
using NaeRaces.Query.EntityFrameworkCore.Extensions;
using NaeRaces.Query.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerNaeRacesQueryDbContext(this IServiceCollection services,IConfiguration configuration, Action<SqlServerDbContextOptionsBuilder>? builder = null)
    {
        //Add-Migration InitialCreate -Project NaeRaces.Query.EntityFrameworkCore.SqlServer -StartupProject NaeRaces.WebAPI
        services.AddDbContext<NaeRacesQueryDbContext>(x =>
        {
            x.UseSqlServer(configuration.GetConnectionString("NaeRacesQuery"),options =>
            {
                options.MigrationsAssembly(typeof(ContextFactory).Assembly.FullName);
                builder?.Invoke(options);
            });

        });
        return services;
    }
}
