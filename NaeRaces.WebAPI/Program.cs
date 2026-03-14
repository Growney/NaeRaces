using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using NaeRaces.Query.EntityFrameworkCore.Extensions;
using NaeRaces.WebAPI;
using NaeRaces.WebAPI.Data;
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

// Seed the OpenIddict application registration and ensure the DB is created.
await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OpenIddictDbContext>();
    await context.Database.EnsureCreatedAsync();

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    var descriptor = new OpenIddictApplicationDescriptor
    {
        ClientId = "naeraces-blazor-client",
        ConsentType = ConsentTypes.Implicit,
        DisplayName = "NaeRaces Blazor Client",
        ClientType = ClientTypes.Public,
        PostLogoutRedirectUris =
        {
            new Uri("https://localhost:7229/authentication/logout-callback")
        },
        RedirectUris =
        {
            new Uri("https://localhost:7229/authentication/login-callback")
        },
        Permissions =
        {
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.EndSession,
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.GrantTypes.RefreshToken,
            Permissions.ResponseTypes.Code,
            Permissions.Scopes.Email,
            Permissions.Scopes.Profile,
            Permissions.Scopes.Roles
        },
        Requirements =
        {
            Requirements.Features.ProofKeyForCodeExchange
        }
    };

    var existing = await manager.FindByClientIdAsync(descriptor.ClientId!);
    if (existing is not null)
        await manager.DeleteAsync(existing);

    await manager.CreateAsync(descriptor);
}

await app.RunAsync();
