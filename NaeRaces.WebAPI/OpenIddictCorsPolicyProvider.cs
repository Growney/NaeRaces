using Microsoft.AspNetCore.Cors.Infrastructure;
using OpenIddict.Abstractions;

namespace NaeRaces.WebAPI;

public class OpenIddictCorsPolicyProvider(IServiceProvider serviceProvider) : ICorsPolicyProvider
{
    public async Task<CorsPolicy?> GetPolicyAsync(HttpContext context, string? policyName)
    {
        var origin = context.Request.Headers.Origin.ToString();
        if (string.IsNullOrEmpty(origin))
            return null;

        await using var scope = serviceProvider.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        await foreach (var application in manager.ListAsync())
        {
            var redirectUris = await manager.GetRedirectUrisAsync(application);
            var postLogoutRedirectUris = await manager.GetPostLogoutRedirectUrisAsync(application);

            foreach (var uri in redirectUris.Concat(postLogoutRedirectUris))
            {
                if (Uri.TryCreate(uri, UriKind.Absolute, out var redirectUri))
                {
                    var appOrigin = $"{redirectUri.Scheme}://{redirectUri.Authority}";
                    if (string.Equals(appOrigin, origin, StringComparison.OrdinalIgnoreCase))
                    {
                        return new CorsPolicyBuilder()
                            .WithOrigins(origin)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .Build();
                    }
                }
            }
        }

        return null;
    }
}
