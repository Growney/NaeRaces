using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NaeRaces.BlazorWebApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseAddress = builder.Configuration["ApiSettings:BaseAddress"];

if (string.IsNullOrWhiteSpace(apiBaseAddress))
{
    apiBaseAddress = builder.HostEnvironment.BaseAddress;
}
builder.Services.AddScoped<ApiAuthorizationMessageHandler>(serviceProvider =>
{
    IAccessTokenProvider tokenProvider = serviceProvider.GetRequiredService<IAccessTokenProvider>();

    return new ApiAuthorizationMessageHandler(tokenProvider, [apiBaseAddress]);
});
builder.Services.AddHttpClient("NaeRaces.ServerAPI")
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(apiBaseAddress))
    .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

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

await builder.Build().RunAsync();
