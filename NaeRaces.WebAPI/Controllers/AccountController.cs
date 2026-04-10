using System.Security.Claims;
using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NaeRaces.Command.Aggregates;
using NaeRaces.Command.ValueTypes;

namespace NaeRaces.WebAPI.Controllers;

public class AccountController : Controller
{
    private readonly IAggregateRepository _aggregateRepository;

    public AccountController(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }

    [HttpGet("~/Identity/Account/Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("~/Identity/Account/Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, bool rememberMe = false, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewData["Error"] = "Email and password are required.";
            return View();
        }

        var pilotId = GeneratePilotIdFromEmail(email);
        var pilot = await _aggregateRepository.Get<Pilot, Guid>(pilotId);

        if (pilot is null || pilot.PasswordHash is null)
        {
            ViewData["Error"] = "Invalid email or password.";
            return View();
        }

        if (!BCrypt.Net.BCrypt.Verify(password, pilot.PasswordHash))
        {
            ViewData["Error"] = "Invalid email or password.";
            return View();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, pilot.Id.ToString()),
            new(ClaimTypes.Name, pilot.UserName ?? email),
            new(ClaimTypes.Email, pilot.Email ?? email)
        };

        foreach (var role in pilot.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = rememberMe
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return Redirect("/");
    }

    [HttpGet("~/Identity/Account/Register")]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("~/Identity/Account/Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string email, string callSign, string password, string confirmPassword, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(callSign))
        {
            ViewData["Error"] = "All fields are required.";
            return View();
        }

        if (password != confirmPassword)
        {
            ViewData["Error"] = "Passwords do not match.";
            return View();
        }

        var pilotId = GeneratePilotIdFromEmail(email);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        CallSign callSignValueType = CallSign.Create(callSign);
        var pilot = _aggregateRepository.CreateNew(() => new Pilot(pilotId, callSignValueType, email, callSign, passwordHash));

        try
        {
            await _aggregateRepository.Save(pilot);
        }
        catch
        {
            ViewData["Error"] = "Error occured";
            return View();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, pilot.Id.ToString()),
            new(ClaimTypes.Name, callSign),
            new(ClaimTypes.Email, email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return Redirect("/");
    }

    [HttpPost("~/Identity/Account/Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    private static Guid GeneratePilotIdFromEmail(string email)
    {
        return GuidFromString($"NaeRaces:Pilot:{email.ToLowerInvariant()}");
    }

    private static Guid GuidFromString(string input)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(input));
        return new Guid(bytes.AsSpan(0, 16));
    }
}
