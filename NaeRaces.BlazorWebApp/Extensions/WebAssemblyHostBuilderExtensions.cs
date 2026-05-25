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

        // Store the resolved address so components (e.g. LoginDisplay) can read it from configuration
        builder.Configuration["ApiSettings:BaseAddress"] = apiBaseAddress;

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
        HttpRequestMessage apiRequest = new HttpRequestMessage(HttpMethod.Get, "/app/wwwroot/api-address");

        using (HttpClient requestClient = new HttpClient())
        {
            requestClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);

            HttpResponseMessage response = await requestClient.SendAsync(apiRequest);

            if (response.IsSuccessStatusCode)
            {
                var contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;

                // When served by Nginx, the response will be plain text.
                // When served by the Blazor dev server, the SPA fallback returns text/html (index.html).
                if (!contentType.Contains("text/html", StringComparison.OrdinalIgnoreCase))
                {
                    string apiAddress = (await response.Content.ReadAsStringAsync()).Trim();

                    if (Uri.TryCreate(apiAddress, UriKind.Absolute, out _))
                    {
                        Console.WriteLine($"Requested Address: {apiAddress}");
                        return apiAddress;
                    }
                }
            }
        }
        var configured = builder.Configuration["ApiSettings:BaseAddress"];

        return string.IsNullOrWhiteSpace(configured)
            ? builder.HostEnvironment.BaseAddress
            : configured;
    }
}
