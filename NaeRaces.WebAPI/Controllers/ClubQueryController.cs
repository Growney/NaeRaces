using Microsoft.AspNetCore.Mvc;
using NaeRaces.Query.Abstractions;
using NaeRaces.WebAPI.Shared.Club;

namespace NaeRaces.WebAPI.Controllers;

public class ClubQueryController : Controller
{
    private readonly IClubOverviewQueryHandler _clubOverviewQueryHandler;
    private readonly IClubLocationQueryHandler _clubLocationQueryHandler;
    private readonly IClubMembershipLevelQueryHandler _clubMembershipLevelQueryHandler;

    public ClubQueryController(
        IClubOverviewQueryHandler clubOverviewQueryHandler,
        IClubLocationQueryHandler clubLocationQueryHandler,
        IClubMembershipLevelQueryHandler clubMembershipLevelQueryHandler)
    {
        _clubOverviewQueryHandler = clubOverviewQueryHandler ?? throw new ArgumentNullException(nameof(clubOverviewQueryHandler));
        _clubLocationQueryHandler = clubLocationQueryHandler ?? throw new ArgumentNullException(nameof(clubLocationQueryHandler));
        _clubMembershipLevelQueryHandler = clubMembershipLevelQueryHandler ?? throw new ArgumentNullException(nameof(clubMembershipLevelQueryHandler));
    }

    [HttpGet("api/club/query/top/{count:int}")]
    public async Task<IActionResult> GetTopClubsByMemberCountAsync([FromRoute] int count)
    {
        var results = new List<TopClubByMemberCountResponse>();
        await foreach (var club in _clubOverviewQueryHandler.GetTopClubsByMemberCount(count))
        {
            results.Add(new TopClubByMemberCountResponse
            {
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
        await foreach (var level in _clubMembershipLevelQueryHandler.GetClubMembershipLevels(clubId))
        {
            results.Add(new ClubMembershipLevelResponse
            {
                MembershipLevelId = level.MembershipLevelId,
                Name = level.Name
            });
        }
        return Ok(results);
    }
}
