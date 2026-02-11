using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NaeRaces.Query.EntityFrameworkCore.SqlServer;
internal class ContextFactory : IDesignTimeDbContextFactory<NaeRacesQueryDbContext>
{
    public NaeRacesQueryDbContext CreateDbContext(string[] args)
    {
        string? connectionString;
        if (args.Length > 0)
        {
            connectionString = args[0];
        }
        else
        {
            Console.WriteLine("No argument provided using appsettings");
            IConfiguration environmentVariables = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            string? environment = environmentVariables["DOTNET_ENVIRONMENT"];
            Console.WriteLine($"Environment: {environment}");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddJsonFile("appsettings.Developer.json", optional: true, reloadOnChange: false)
                .Build();

            connectionString = configuration.GetConnectionString("NaeRacesQuery");
        }

        DbContextOptionsBuilder<NaeRacesQueryDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsAssembly(GetType().Assembly.GetName().Name));

        return new NaeRacesQueryDbContext(optionsBuilder.Options);
    }
}