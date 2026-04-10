using NaeRaces.WebAPI.Data;
using OpenIddict.Abstractions;

namespace NaeRaces.WebAPI;

public static class OpenIddictSeeder
{
    public static async Task SeedOpenIddictClientsAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<OpenIddictDbContext>();
        await context.Database.EnsureCreatedAsync();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var clients = app.Configuration
            .GetSection(OpenIddictClientSettings.SectionName)
            .Get<List<OpenIddictClientSettings>>() ?? [];

        foreach (var client in clients)
        {
            var descriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = client.ClientId,
                ConsentType = client.ConsentType,
                DisplayName = client.DisplayName,
                ClientType = client.ClientType,
            };

            foreach (var uri in client.PostLogoutRedirectUris)
                descriptor.PostLogoutRedirectUris.Add(new Uri(uri));

            foreach (var uri in client.RedirectUris)
                descriptor.RedirectUris.Add(new Uri(uri));

            foreach (var permission in client.Permissions)
                descriptor.Permissions.Add(permission);

            foreach (var requirement in client.Requirements)
                descriptor.Requirements.Add(requirement);

            var existing = await manager.FindByClientIdAsync(descriptor.ClientId);
            if (existing is not null)
                await manager.DeleteAsync(existing);

            await manager.CreateAsync(descriptor);
        }
    }
}
