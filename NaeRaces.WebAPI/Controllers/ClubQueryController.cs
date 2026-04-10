using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using NaeRaces.WebAPI.Shared.Club;
using OpenIddict.Abstractions;

namespace NaeRaces.WebAPI.Controllers;

public class ClubQueryController : Controller
{
    private readonly IClubOverviewQueryHandler _clubOverviewQueryHandler;
    private readonly IClubLocationQueryHandler _clubLocationQueryHandler;
    private readonly IClubMembershipLevelQueryHandler _clubMembershipLevelQueryHandler;
    private readonly IClubMemberQueryHandler _clubMemberQueryHandler;
    private readonly IClubDetailsQueryHandler _clubDetailsQueryHandler;
    private readonly IPilotSelectionPolicyQueryHandler _pilotSelectionPolicyQueryHandler;
    private readonly IPilotDetailsQueryHandler _pilotDetailsQueryHandler;
    private readonly IPilotFollowedClubQueryHandler _pilotFollowedClubQueryHandler;

    public ClubQueryController(
        IClubOverviewQueryHandler clubOverviewQueryHandler,
        IClubLocationQueryHandler clubLocationQueryHandler,
        IClubMembershipLevelQueryHandler clubMembershipLevelQueryHandler,
        IClubMemberQueryHandler clubMemberQueryHandler,
        IClubDetailsQueryHandler clubDetailsQueryHandler,
        IPilotSelectionPolicyQueryHandler pilotSelectionPolicyQueryHandler,
        IPilotDetailsQueryHandler pilotDetailsQueryHandler,
        IPilotFollowedClubQueryHandler pilotFollowedClubQueryHandler)
    {
        _clubOverviewQueryHandler = clubOverviewQueryHandler ?? throw new ArgumentNullException(nameof(clubOverviewQueryHandler));
        _clubLocationQueryHandler = clubLocationQueryHandler ?? throw new ArgumentNullException(nameof(clubLocationQueryHandler));
        _clubMembershipLevelQueryHandler = clubMembershipLevelQueryHandler ?? throw new ArgumentNullException(nameof(clubMembershipLevelQueryHandler));
        _clubMemberQueryHandler = clubMemberQueryHandler ?? throw new ArgumentNullException(nameof(clubMemberQueryHandler));
        _clubDetailsQueryHandler = clubDetailsQueryHandler ?? throw new ArgumentNullException(nameof(clubDetailsQueryHandler));
        _pilotSelectionPolicyQueryHandler = pilotSelectionPolicyQueryHandler ?? throw new ArgumentNullException(nameof(pilotSelectionPolicyQueryHandler));
        _pilotDetailsQueryHandler = pilotDetailsQueryHandler ?? throw new ArgumentNullException(nameof(pilotDetailsQueryHandler));
        _pilotFollowedClubQueryHandler = pilotFollowedClubQueryHandler ?? throw new ArgumentNullException(nameof(pilotFollowedClubQueryHandler));
    }

    [Authorize]
    [HttpGet("api/club/query/my-clubs")]
    public async Task<IActionResult> GetMyClubsAsync()
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        var results = new List<MyClubMembershipResponse>();
        var memberClubIds = new HashSet<Guid>();
        var memberships = await _clubMemberQueryHandler.GetPilotMembershipDetails(pilotId).ToListAsync();
        foreach (var membership in memberships)
        {
            if (!membership.IsRegistrationConfirmed)
                continue;

            var club = await _clubOverviewQueryHandler.GetClubOverview(membership.ClubId);
            if (club == null)
                continue;

            memberClubIds.Add(club.ClubId);

            string? levelName = null;
            if (membership.MembershipLevelId.HasValue)
            {
                var level = await _clubMembershipLevelQueryHandler.GetClubMembershipLevel(membership.ClubId, membership.MembershipLevelId.Value);
                levelName = level?.Name;
            }

            var roles = await _clubMemberQueryHandler.GetClubMemberRoles(club.ClubId, pilotId)
                .Select(r => r.Role)
                .ToListAsync();

            results.Add(new MyClubMembershipResponse
            {
                ClubId = club.ClubId,
                ClubCode = club.Code,
                ClubName = club.Name,
                MembershipLevelName = levelName,
                MembershipExpiry = membership.RegistrationValidUntil,
                Roles = roles
            });
        }

        var followedClubIds = await _pilotFollowedClubQueryHandler.GetFollowedClubs(pilotId).ToListAsync();
        foreach (var followed in followedClubIds)
        {
            if (memberClubIds.Contains(followed.ClubId))
                continue;

            var club = await _clubOverviewQueryHandler.GetClubOverview(followed.ClubId);
            if (club == null)
                continue;

            memberClubIds.Add(club.ClubId);

            var roles = await _clubMemberQueryHandler.GetClubMemberRoles(club.ClubId, pilotId)
                .Select(r => r.Role)
                .ToListAsync();

            results.Add(new MyClubMembershipResponse
            {
                ClubId = club.ClubId,
                ClubCode = club.Code,
                ClubName = club.Name,
                IsFollowing = true,
                Roles = roles
            });
        }

        string[] allRoles = ["Administrator", "RaceOrganiser", "Trustee"];
        var clubsWithRoles = await _clubMemberQueryHandler.GetClubIdsWithRoles(pilotId, allRoles).ToListAsync();
        foreach (var clubId in clubsWithRoles)
        {
            if (memberClubIds.Contains(clubId))
                continue;

            var club = await _clubOverviewQueryHandler.GetClubOverview(clubId);
            if (club == null)
                continue;

            memberClubIds.Add(clubId);

            var roles = await _clubMemberQueryHandler.GetClubMemberRoles(clubId, pilotId)
                .Select(r => r.Role)
                .ToListAsync();

            results.Add(new MyClubMembershipResponse
            {
                ClubId = club.ClubId,
                ClubCode = club.Code,
                ClubName = club.Name,
                Roles = roles
            });
        }

        return Ok(results);
    }

    [Authorize]
    [HttpGet("api/club/{clubId:guid}/query/is-following")]
    public async Task<IActionResult> IsCurrentUserFollowingAsync([FromRoute] Guid clubId)
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        var isFollowing = await _pilotFollowedClubQueryHandler.IsFollowingClub(pilotId, clubId);
        return Ok(isFollowing);
    }

    [HttpGet("api/club/query/top/{count:int}")]
    public async Task<IActionResult> GetTopClubsByMemberCountAsync([FromRoute] int count)
    {
        var results = new List<TopClubByMemberCountResponse>();
        await foreach (var club in _clubOverviewQueryHandler.GetTopClubsByMemberCount(count))
        {
            results.Add(new TopClubByMemberCountResponse
            {
                ClubId = club.ClubId,
                Code = club.Code,
                Name = club.Name,
                MemberCount = club.TotalMemberCount
            });
        }
        return Ok(results);
    }

    [HttpGet("api/club/query/with-upcoming-races")]
    public async Task<IActionResult> GetClubsWithUpcomingRacesAsync()
    {
        var results = new List<ClubWithUpcomingRacesResponse>();
        await foreach (var club in _clubOverviewQueryHandler.GetClubsWithRacesAfter(DateTime.UtcNow))
        {
            results.Add(new ClubWithUpcomingRacesResponse
            {
                ClubId = club.ClubId,
                Code = club.Code,
                Name = club.Name,
                MemberCount = club.TotalMemberCount
            });
        }
        return Ok(results);
    }

    [HttpGet("api/club/query/all")]
    public async Task<IActionResult> GetAllClubsAsync()
    {
        var results = new List<ClubSearchResponse>();
        await foreach (var club in _clubOverviewQueryHandler.GetAllClubs())
        {
            results.Add(new ClubSearchResponse
            {
                ClubId = club.ClubId,
                Code = club.Code,
                Name = club.Name,
                MemberCount = club.TotalMemberCount
            });
        }
        return Ok(results);
    }

    [HttpGet("api/club/query/search")]
    public async Task<IActionResult> SearchClubsAsync([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Search term is required.");
        }

        var results = new List<ClubSearchResponse>();
        await foreach (var club in _clubOverviewQueryHandler.SearchClubs(term))
        {
            results.Add(new ClubSearchResponse
            {
                ClubId = club.ClubId,
                Code = club.Code,
                Name = club.Name,
                MemberCount = club.TotalMemberCount
            });
        }
        return Ok(results);
    }

    [HttpGet("api/club/{clubId:guid}/query/overview")]
    public async Task<IActionResult> GetClubOverviewAsync([FromRoute] Guid clubId)
    {
        var club = await _clubOverviewQueryHandler.GetClubOverview(clubId);
        if (club == null)
            return NotFound();

        return Ok(new ClubOverviewResponse
        {
            ClubId = club.ClubId,
            Code = club.Code,
            Name = club.Name,
            MemberCount = club.TotalMemberCount
        });
    }

    [HttpGet("api/club/{clubId:guid}/query/locations")]
    public async Task<IActionResult> GetClubLocationsAsync([FromRoute] Guid clubId)
    {
        var results = new List<ClubLocationResponse>();
        await foreach (var loc in _clubLocationQueryHandler.GetClubLocations(clubId))
        {
            results.Add(new ClubLocationResponse
            {
                LocationId = loc.LocationId,
                Name = loc.Name,
                Information = loc.Information,
                AddressLine1 = loc.AddressLine1,
                AddressLine2 = loc.AddressLine2,
                City = loc.City,
                Postcode = loc.Postcode,
                County = loc.County,
                IsHomeLocation = loc.IsHomeLocation
            });
        }
        return Ok(results);
    }

    [HttpGet("api/club/{clubId:guid}/query/membershiplevels")]
    public async Task<IActionResult> GetClubMembershipLevelsAsync([FromRoute] Guid clubId)
    {
        var results = new List<ClubMembershipLevelResponse>();
        var policyNames = new Dictionary<Guid, string>();

        var levels = await _clubMembershipLevelQueryHandler.GetClubMembershipLevels(clubId).ToListAsync();
        foreach (var level in levels)
        {
            string? policyName = null;
            if (level.PilotPolicyId.HasValue)
            {
                if (!policyNames.TryGetValue(level.PilotPolicyId.Value, out policyName))
                {
                    var policy = await _pilotSelectionPolicyQueryHandler.GetPolicyDetails(level.PilotPolicyId.Value, clubId);
                    policyName = policy?.Name;
                    if (policyName != null)
                        policyNames[level.PilotPolicyId.Value] = policyName;
                }
            }

            results.Add(new ClubMembershipLevelResponse
            {
                MembershipLevelId = level.MembershipLevelId,
                Name = level.Name,
                PilotPolicyId = level.PilotPolicyId,
                PilotPolicyName = policyName,
                PaymentOptions = level.PaymentOptions.Select(po => new ClubMembershipLevelPaymentOptionResponse
                {
                    PaymentOptionId = po.PaymentOptionId,
                    Name = po.Name,
                    PaymentType = po.PaymentType.ToString(),
                    Currency = po.Currency,
                    Price = po.Price,
                    DayOfMonthDue = po.DayOfMonthDue,
                    PaymentInterval = po.PaymentInterval
                }).ToList()
            });
        }
        return Ok(results);
    }

    [HttpGet("api/club/{clubId:guid}/query/contactdetails")]
    public async Task<IActionResult> GetClubContactDetailsAsync([FromRoute] Guid clubId)
    {
        var contactDetails = await _clubDetailsQueryHandler.GetClubContactDetails(clubId);
        if (contactDetails == null)
            return NotFound();

        return Ok(new ClubContactDetailsResponse
        {
            PhoneNumber = contactDetails.PhoneNumber,
            EmailAddress = contactDetails.EmailAddress
        });
    }

    [Authorize]
    [HttpGet("api/club/{clubId:guid}/query/is-admin")]
    public async Task<IActionResult> IsCurrentUserAdminAsync([FromRoute] Guid clubId)
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        var isAdmin = await _clubMemberQueryHandler.HasClubMemberRole(clubId, pilotId, nameof(Command.ValueTypes.ClubMemberRole.Administrator));
        return Ok(isAdmin);
    }

    [Authorize]
    [HttpGet("api/club/{clubId:guid}/query/is-member")]
    public async Task<IActionResult> IsCurrentUserMemberAsync([FromRoute] Guid clubId)
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        var isMember = await _clubMemberQueryHandler.HasEverBeenClubMember(clubId, pilotId);
        return Ok(isMember);
    }

    [HttpGet("api/club/{clubId:guid}/query/members")]
    public async Task<IActionResult> GetClubMembersAsync([FromRoute] Guid clubId)
    {
        Guid? currentPilotId = null;
        bool isAdmin = false;

        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (Guid.TryParse(pilotIdClaim, out Guid parsedPilotId))
        {
            currentPilotId = parsedPilotId;
            isAdmin = await _clubMemberQueryHandler.HasClubMemberRole(clubId, parsedPilotId, nameof(Command.ValueTypes.ClubMemberRole.Administrator));
        }

        bool isMember = currentPilotId.HasValue &&
            await _clubMemberQueryHandler.IsCurrentlyActiveClubMember(clubId, currentPilotId.Value);

        var membershipLevelNames = new Dictionary<int, string>();
        await foreach (var level in _clubMembershipLevelQueryHandler.GetClubMembershipLevels(clubId))
        {
            membershipLevelNames[level.MembershipLevelId] = level.Name;
        }

        var results = new List<ClubMemberListResponse>();
        //We must realise the list in order to query the display name as we iterate through it.
        var clubMembers = await _clubMemberQueryHandler.GetClubMembers(clubId).ToListAsync();
        foreach (var member in clubMembers)
        {
            if (!member.IsRegistrationConfirmed && !isAdmin)
                continue;

            string? levelName = null;
            DateTime? expiry = null;

            if (isAdmin || (isMember && member.PilotId == currentPilotId))
            {
                if (member.MembershipLevelId.HasValue)
                    membershipLevelNames.TryGetValue(member.MembershipLevelId.Value, out levelName);
                expiry = member.RegistrationValidUntil;
            }

            var displayName = await _pilotDetailsQueryHandler.GetPilotDisplayName(member.PilotId)
                ?? member.PilotId.ToString();

            results.Add(new ClubMemberListResponse
            {
                PilotId = member.PilotId,
                DisplayName = displayName,
                MembershipLevelName = levelName,
                MembershipExpiry = expiry,
                IsRegistrationConfirmed = member.IsRegistrationConfirmed,
                RegistrationId = isAdmin || member.PilotId == currentPilotId ? member.Id : null,
                MembershipLevelId = isAdmin ? member.MembershipLevelId : null,
                PaymentOptionId = isAdmin ? member.PaymentOptionId : null
            });
        }

        return Ok(results);
    }

    [Authorize]
    [HttpGet("api/club/query/my-organiser-clubs")]
    public async Task<IActionResult> GetMyOrganiserClubsAsync()
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        var roles = new[] { nameof(Command.ValueTypes.ClubMemberRole.Administrator), nameof(Command.ValueTypes.ClubMemberRole.RaceOrganiser) };
        var results = new List<Shared.Race.RaceOrganiserClubResponse>();
        var clubsWithRoles = await _clubMemberQueryHandler.GetClubIdsWithRoles(pilotId, roles).ToListAsync();
        foreach (var clubId in clubsWithRoles)
        {
            var club = await _clubOverviewQueryHandler.GetClubOverview(clubId);
            if (club == null) continue;

            results.Add(new Shared.Race.RaceOrganiserClubResponse
            {
                ClubId = club.ClubId,
                Code = club.Code,
                Name = club.Name
            });
        }

        return Ok(results);
    }
}
