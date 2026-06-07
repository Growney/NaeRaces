using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using NaeRaces.BlazorWebApp;
using NaeRaces.BlazorWebApp.Extensions;
using NaeRaces.BlazorWebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.AddApiClients();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
