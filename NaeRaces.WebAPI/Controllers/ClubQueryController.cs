using EventDbLite.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaeRaces.Command.Aggregates;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using NaeRaces.Query.Projections;
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
    private readonly IPilotRelevantClubQueryHandler _pilotRelevantClubQueryHandler;
    private readonly IProjectionProvider _projectionProvider;

    public ClubQueryController(
        IClubOverviewQueryHandler clubOverviewQueryHandler,
        IClubLocationQueryHandler clubLocationQueryHandler,
        IClubMembershipLevelQueryHandler clubMembershipLevelQueryHandler,
        IClubMemberQueryHandler clubMemberQueryHandler,
        IClubDetailsQueryHandler clubDetailsQueryHandler,
        IPilotSelectionPolicyQueryHandler pilotSelectionPolicyQueryHandler,
        IPilotDetailsQueryHandler pilotDetailsQueryHandler,
        IPilotFollowedClubQueryHandler pilotFollowedClubQueryHandler,
        IPilotRelevantClubQueryHandler pilotRelevantClubQueryHandler,
        IProjectionProvider projectionProvider)
    {
        _clubOverviewQueryHandler = clubOverviewQueryHandler ?? throw new ArgumentNullException(nameof(clubOverviewQueryHandler));
        _clubLocationQueryHandler = clubLocationQueryHandler ?? throw new ArgumentNullException(nameof(clubLocationQueryHandler));
        _clubMembershipLevelQueryHandler = clubMembershipLevelQueryHandler ?? throw new ArgumentNullException(nameof(clubMembershipLevelQueryHandler));
        _clubMemberQueryHandler = clubMemberQueryHandler ?? throw new ArgumentNullException(nameof(clubMemberQueryHandler));
        _clubDetailsQueryHandler = clubDetailsQueryHandler ?? throw new ArgumentNullException(nameof(clubDetailsQueryHandler));
        _pilotSelectionPolicyQueryHandler = pilotSelectionPolicyQueryHandler ?? throw new ArgumentNullException(nameof(pilotSelectionPolicyQueryHandler));
        _pilotDetailsQueryHandler = pilotDetailsQueryHandler ?? throw new ArgumentNullException(nameof(pilotDetailsQueryHandler));
        _pilotFollowedClubQueryHandler = pilotFollowedClubQueryHandler ?? throw new ArgumentNullException(nameof(pilotFollowedClubQueryHandler));
        _pilotRelevantClubQueryHandler = pilotRelevantClubQueryHandler ?? throw new ArgumentNullException(nameof(pilotRelevantClubQueryHandler));
        _projectionProvider = projectionProvider ?? throw new ArgumentNullException(nameof(projectionProvider));
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
        var projection = await _projectionProvider.CloneAsync<PilotRelevantClubsProjection>();

        var relevantClubs = projection.Object.GetRelevantClubs(pilotId);

        var responses = relevantClubs.Select(x => new MyClubMembershipResponse()
        {
            ClubId = x.ClubId,
            ClubCode = x.ClubCode,
            ClubName = x.ClubName,
            IsFollowing = x.Relationship.IsFollowing,
            Roles = x.Relationship.Roles.ToList(),
            MembershipExpiry = x.Relationship.Membership?.Expiry,
            MembershipLevelName = x.Relationship.Membership?.MembershipName,
        });

        return Ok(responses);
    }

    [HttpGet("api/club/{clubId:guid}/query/description")]
    public async Task<IActionResult> GetClubDescriptionAsync([FromRoute] Guid clubId)
    {
        var projection = await _projectionProvider.CloneAsync<ClubDescription>();

        string? description = projection.Object.GetClubDescription(clubId);

        return Ok(new ClubDescriptionResponse()
        {
            Description = description
        });
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

    [HttpGet("api/club/query/all-with-membership")]
    public async Task<IActionResult> GetAllClubsWithMembershipInfoAsync()
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;

        IEnumerable<PilotRelevantClubsProjection.PilotRelevantClub> clubsRelevantToCurrentUser = Enumerable.Empty<PilotRelevantClubsProjection.PilotRelevantClub>();
        if (Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            var relevantClubProjection = await _projectionProvider.CloneAsync<PilotRelevantClubsProjection>();

            clubsRelevantToCurrentUser = relevantClubProjection.Object.GetRelevantClubs(pilotId);
        }

        var memberPopularityProjection = await _projectionProvider.CloneAsync<ClubMemberPopularity>();

        var clubPopularity = memberPopularityProjection.Object.GetAll();

        var responses = clubPopularity.Select(x => new ClubWithMembershipResponse
        {
            ClubId = x.ClubId,
            ClubCode = x.ClubCode,
            ClubName = x.ClubName,
            MembershipLevelName = clubsRelevantToCurrentUser.FirstOrDefault(clubMembership => x.ClubId == clubMembership.ClubId)?.Relationship?.Membership?.MembershipName,
            MembershipExpiry = clubsRelevantToCurrentUser.FirstOrDefault(clubMembership => x.ClubId == clubMembership.ClubId)?.Relationship?.Membership?.Expiry,
            IsMembershipConfirmed = clubsRelevantToCurrentUser.FirstOrDefault(clubMembership => x.ClubId == clubMembership.ClubId)?.Relationship?.Membership?.IsConfirmed,
            IsFollowing = clubsRelevantToCurrentUser.FirstOrDefault(clubMembership => x.ClubId == clubMembership.ClubId)?.Relationship?.IsFollowing ?? false,
            Members = x.Members,
            Followers = x.Followers
        });

        return Ok(responses);
    }

    [HttpGet("api/club/query/top/{count:int}")]
    public async Task<IActionResult> GetTopClubsByMemberCountAsync([FromRoute] int count)
    {
        var projection = await _projectionProvider.CloneAsync<ClubMemberPopularity>();

        var relevantClubs = projection.Object.GetTop(count);

        var results = new List<TopClubByMemberCountResponse>();

        foreach (var club in relevantClubs)
        {
            results.Add(new TopClubByMemberCountResponse
            {
                ClubId = club.ClubId,
                Code = club.ClubCode,
                Name = club.ClubName,
                MemberCount = club.Members,
                FollowerCount = club.Followers
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

    private async Task<IActionResult> CheckForRolesAsync(Guid clubId, params IEnumerable<string> roles)
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        var canEditAbout = await _clubMemberQueryHandler.HasClubMemberRole(clubId, pilotId, roles);

        if (!canEditAbout)
        {
            return Unauthorized();
        }

        return Ok(canEditAbout);
    }

    [Authorize]
    [HttpGet("api/club/{clubId:guid}/query/can-edit-about")]
    public Task<IActionResult> CanCurrentUserEditAboutAsync([FromRoute] Guid clubId) => CheckForRolesAsync(clubId, Command.ValueTypes.ClubMemberRole.Trustee, Command.ValueTypes.ClubMemberRole.Administrator);
    [Authorize]
    [HttpGet("api/club/{clubId:guid}/query/can-manage-races")]
    public Task<IActionResult> CanCurrentManageRacesAsync([FromRoute] Guid clubId) => CheckForRolesAsync(clubId, Command.ValueTypes.ClubMemberRole.Trustee, Command.ValueTypes.ClubMemberRole.Administrator, Command.ValueTypes.ClubMemberRole.RaceOrganiser);
    [Authorize]
    [HttpGet("api/club/{clubId:guid}/query/is-admin")]
    public Task<IActionResult> IsCurrentUserAdminAsync([FromRoute] Guid clubId) => CheckForRolesAsync(clubId, Command.ValueTypes.ClubMemberRole.Administrator);

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

    [HttpGet("api/club/{clubId:guid}/query/members/details")]
    public async Task<IActionResult> GetClubMemberDetailsAsync([FromRoute] Guid clubId)
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        bool isAdminOrTrustee = false;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            isAdminOrTrustee = await _clubMemberQueryHandler.HasClubMemberRole(clubId, pilotId,
                nameof(Command.ValueTypes.ClubMemberRole.Administrator),
                nameof(Command.ValueTypes.ClubMemberRole.Trustee));
        }

        var projection = await _projectionProvider.CloneAsync<NaeRaces.Query.Projections.ClubMember>();
        var members = projection.Object.GetClubMembers(clubId);

        var results = members.Select(member => new ClubMemberDetailResponse
        {
            PilotId = member.PilotId,
            CallSign = member.CallSign,
            Name = member.Name,
            Nationality = member.Nationality,
            Email = isAdminOrTrustee ? member.Email : null,
            DateOfBirth = isAdminOrTrustee ? member.DateOfBirth : null,
            MembershipLevelId = isAdminOrTrustee ? member.MembershipLevelId : null,
            MembershipLevelName = isAdminOrTrustee ? member.MembershipLevelName : null,
            PaymentOptionId = isAdminOrTrustee ? member.PaymentOptionId : null,
            PaymentOptionName = isAdminOrTrustee ? member.PaymentOptionName : null,
            ValidUntil = isAdminOrTrustee ? member.ValidUntil : null,
        }).ToList();

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
