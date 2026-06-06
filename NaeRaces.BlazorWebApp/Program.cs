using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NaeRaces.BlazorWebApp;
using NaeRaces.BlazorWebApp.Extensions;
using NaeRaces.BlazorWebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorBootstrap();
builder.Services.AddScoped<ClubContext>();

await builder.AddApiClients();

await builder.Build().RunAsync();
