using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NaeRaces.BlazorWebApp.Services;

namespace NaeRaces.BlazorWebApp.Extensions;

static class WebAssemblyHostBuilderExtensions
{
    public static async Task<WebAssemblyHostBuilder> AddApiClients(this WebAssemblyHostBuilder builder)
    {
        var apiBaseAddress = await ResolveApiBaseAddressAsync(builder);

        Console.WriteLine($"API Address: {apiBaseAddress}");

        builder.Services.AddScoped<ApiAuthorizationMessageHandler>(serviceProvider =>
        {
            var tokenProvider = serviceProvider.GetRequiredService<IAccessTokenProvider>();
            return new ApiAuthorizationMessageHandler(tokenProvider, [apiBaseAddress]);
        });

        builder.Services.AddHttpClient("NaeRaces.ServerAPI")
            .ConfigureHttpClient(client =>
            {
                Console.WriteLine(apiBaseAddress);
                client.BaseAddress = new Uri(apiBaseAddress);
            })
            .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

        builder.Services.AddHttpClient("NaeRaces.ServerAPI.Health")
            .ConfigureHttpClient(client =>
            {
                Console.WriteLine(apiBaseAddress);
                client.BaseAddress = new Uri(apiBaseAddress);
            });

        builder.Services.AddScoped<ApiHealthService>();

        builder.Services.AddScoped(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return factory.CreateClient("NaeRaces.ServerAPI");
        });

        builder.Services.AddOidcAuthentication(options =>
        {
            options.ProviderOptions.ClientId = "naeraces-blazor-client";
            options.ProviderOptions.Authority = apiBaseAddress;
            options.ProviderOptions.ResponseType = "code";
            options.ProviderOptions.ResponseMode = "query";

            options.ProviderOptions.DefaultScopes.Add("roles");
            options.UserOptions.RoleClaim = "role";
        });

        return builder;
    }

    private static async Task<string> ResolveApiBaseAddressAsync(WebAssemblyHostBuilder builder)
    {
        var configured = builder.Configuration["ApiSettings:BaseAddress"];

        return string.IsNullOrWhiteSpace(configured)
            ? builder.HostEnvironment.BaseAddress
            : configured;
    }
}
