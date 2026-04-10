using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using NaeRaces.Query.EntityFrameworkCore.Extensions;
using NaeRaces.WebAPI;
using NaeRaces.WebAPI.Data;
using NaeRaces.WebAPI.Services;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();
builder.Services.AddSingleton<ICorsPolicyProvider, OpenIddictCorsPolicyProvider>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddOpenApi();
builder.Services.AddKurrentDbEventDbLite(x =>
{

});
builder.Services.AddSqlServerNaeRacesQueryDbContext(builder.Configuration);
builder.Services.AddNaeRacesEntityFrameworkCoreQueryReactions();
builder.Services.AddNaeRacesEntityFrameworkCoreQueryHandlers();
builder.Services.AddHostedService<ClubMembershipExpiryService>();

// Configure the OpenIddict EF Core DbContext for token/application storage.
builder.Services.AddDbContext<OpenIddictDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OpenIddict"));
    options.UseOpenIddict();
});

// Configure authentication with a policy scheme that selects bearer or cookie auth based on the request.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "BearerOrCookie";
    options.DefaultChallengeScheme = "BearerOrCookie";
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
})
.AddPolicyScheme("BearerOrCookie", displayName: null, options =>
{
    options.ForwardDefaultSelector = context =>
    {
        string? authorization = context.Request.Headers["Authorization"];
        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
});

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<OpenIddictDbContext>();
    })
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("connect/authorize")
               .SetEndSessionEndpointUris("connect/logout")
               .SetTokenEndpointUris("connect/token")
               .SetUserInfoEndpointUris("connect/userinfo");

        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

        options.AllowAuthorizationCodeFlow()
               .AllowRefreshTokenFlow();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableEndSessionEndpointPassthrough()
               .EnableStatusCodePagesIntegration()
               .EnableTokenEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

await app.SeedOpenIddictClientsAsync();

await app.RunAsync();
