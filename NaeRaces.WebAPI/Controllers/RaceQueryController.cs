using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;
using NaeRaces.WebAPI.Shared.Race;
using OpenIddict.Abstractions;

namespace NaeRaces.WebAPI.Controllers;

public class RaceQueryController : Controller
{
    private readonly IRaceInformationQueryHandler _raceInformationQueryHandler;
    private readonly IRacePackageQueryHandler _racePackageQueryHandler;
    private readonly IRaceDateQueryHandler _raceDateQueryHandler;
    private readonly IClubMemberQueryHandler _clubMemberQueryHandler;
    private readonly IPilotRelevantClubQueryHandler _pilotRelevantClubQueryHandler;

    public RaceQueryController(
        IRaceInformationQueryHandler raceInformationQueryHandler,
        IRacePackageQueryHandler racePackageQueryHandler,
        IRaceDateQueryHandler raceDateQueryHandler,
        IClubMemberQueryHandler clubMemberQueryHandler,
        IPilotRelevantClubQueryHandler pilotRelevantClubQueryHandler)
    {
        _raceInformationQueryHandler = raceInformationQueryHandler ?? throw new ArgumentNullException(nameof(raceInformationQueryHandler));
        _racePackageQueryHandler = racePackageQueryHandler ?? throw new ArgumentNullException(nameof(racePackageQueryHandler));
        _raceDateQueryHandler = raceDateQueryHandler ?? throw new ArgumentNullException(nameof(raceDateQueryHandler));
        _clubMemberQueryHandler = clubMemberQueryHandler ?? throw new ArgumentNullException(nameof(clubMemberQueryHandler));
        _pilotRelevantClubQueryHandler = pilotRelevantClubQueryHandler ?? throw new ArgumentNullException(nameof(pilotRelevantClubQueryHandler));
    }

    [HttpGet("api/race/query/relevant")]
    public async Task<IActionResult> GetRelevantRacesAsync(int? max)
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;

        if(Guid.TryParse(pilotIdClaim, out var pilotId))
        {
            var pilotClubIds = await _pilotRelevantClubQueryHandler.GetPilotRelevantClubs(pilotId).Select(x => x.ClubId).ToListAsync();

            if (pilotClubIds.Any())
            {
                IEnumerable<RaceInformation> races = await _raceInformationQueryHandler.GetRaceInformationForClubsAfterDate(pilotClubIds,DateTime.Now).ToListAsync();

                if (max.HasValue)
                {
                    races = races.OrderBy(x => x.FirstRaceDateStart).Take(max.Value);
                }

                if (races.Any())
                {
                    return Ok(races.Select(x => MapToDto(x)));
                }
            }
        }

        var results = new List<RaceInformationResponse>();

        await foreach(var raceInfo in _raceInformationQueryHandler.GetRaceInformationAfterDate(DateTime.Now)) 
        {
            if(results.Count > (max ?? 10))
            {
                break;
            }
            results.Add(MapToDto(raceInfo));
        }

        return Ok(results);
    }

    private RaceInformationResponse MapToDto(RaceInformation info) =>
        new()
        {
            RaceId = info.RaceId,
            Name = info.Name,
            ClubId = info.ClubId,
            ClubName = info.ClubName,
            LocationName = info.LocationName,
            FirstRaceDateStart = info.FirstRaceDateStart,
            LastRaceDateEnd = info.LastRaceDateEnd,
            RegisteredPilotCount = info.RegisteredPilotCount,
            MinimumPilots = info.MinimumPilots,
            MaximumPilots = info.MaximumPilots,
            IsPublished = info.IsPublished,
            Description = info.Description,
            PaymentDeadline = info.PaymentDeadline,
            GoNoGoDate = info.GoNoGoDate
        };

    [HttpGet("api/race/{raceId:guid}/query/information")]
    public async Task<IActionResult> GetRaceInformationAsync([FromRoute] Guid raceId)
    {
        var info = await _raceInformationQueryHandler.GetRaceInformation(raceId);
        if (info == null)
            return NotFound();

        return Ok(MapToDto(info));
    }

    [HttpGet("api/race/query/club/{clubId:guid}")]
    public async Task<IActionResult> GetRacesForClubAsync([FromRoute] Guid clubId)
    {
        var results = new List<RaceInformationResponse>();
        await foreach (var info in _raceInformationQueryHandler.GetRaceInformationForClub(clubId))
        {
            results.Add(MapToDto(info));
        }
        return Ok(results);
    }

    [Authorize]
    [HttpGet("api/race/{raceId:guid}/query/is-organiser")]
    public async Task<IActionResult> IsCurrentUserRaceOrganiserAsync([FromRoute] Guid raceId)
    {
        var pilotIdClaim = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (!Guid.TryParse(pilotIdClaim, out Guid pilotId))
        {
            return Unauthorized();
        }

        var info = await _raceInformationQueryHandler.GetRaceInformation(raceId);
        if (info == null)
            return NotFound();

        var isOrganiser = await _clubMemberQueryHandler.HasClubMemberRole(
            info.ClubId, pilotId,
            nameof(Command.ValueTypes.ClubMemberRole.Administrator),
            nameof(Command.ValueTypes.ClubMemberRole.RaceOrganiser));

        return Ok(isOrganiser);
    }

    [HttpGet("api/race/{raceId:guid}/query/packages")]
    public async Task<IActionResult> GetRacePackagesAsync([FromRoute] Guid raceId)
    {
        var results = new List<RacePackageResponse>();
        await foreach (var pkg in _racePackageQueryHandler.GetRacePackages(raceId))
        {
            results.Add(new RacePackageResponse
            {
                RacePackageId = pkg.RacePackageId,
                Name = pkg.Name,
                Currency = pkg.Currency,
                Cost = pkg.Cost,
                ApplyDiscounts = pkg.ApplyDiscounts,
                RegistrationOpenDate = pkg.RegistrationOpenDate,
                RegistrationCloseDate = pkg.RegistrationCloseDate,
                IsRegistrationManuallyOpened = pkg.IsRegistrationManuallyOpened,
                PilotPolicyId = pkg.PilotPolicyId
            });
        }
        return Ok(results);
    }

    [HttpGet("api/race/{raceId:guid}/query/dates")]
    public async Task<IActionResult> GetRaceDatesAsync([FromRoute] Guid raceId)
    {
        var results = new List<RaceDateResponse>();
        await foreach (var date in _raceDateQueryHandler.GetRaceDates(raceId))
        {
            results.Add(new RaceDateResponse
            {
                RaceDateId = date.RaceDateId,
                Start = date.Start,
                End = date.End
            });
        }
        return Ok(results);
    }
}
