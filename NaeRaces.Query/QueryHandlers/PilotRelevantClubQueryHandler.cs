using NaeRaces.Query.Abstractions;
using NaeRaces.Query.Models;

namespace NaeRaces.Query.QueryHandlers;

public class PilotRelevantClubQueryHandler : IPilotRelevantClubQueryHandler
{
    private readonly IClubMemberQueryHandler _clubMemberQueryHandler;
    private readonly IClubOverviewQueryHandler _clubOverviewQueryHandler;
    private readonly IPilotFollowedClubQueryHandler _pilotFollowedClubQueryHandler;

    public PilotRelevantClubQueryHandler(
        IClubMemberQueryHandler clubMemberQueryHandler,
        IClubOverviewQueryHandler clubOverviewQueryHandler,
        IPilotFollowedClubQueryHandler pilotFollowedClubQueryHandler)
    {
        _clubMemberQueryHandler = clubMemberQueryHandler ?? throw new ArgumentNullException(nameof(clubMemberQueryHandler));
        _clubOverviewQueryHandler = clubOverviewQueryHandler ?? throw new ArgumentNullException(nameof(clubOverviewQueryHandler));
        _pilotFollowedClubQueryHandler = pilotFollowedClubQueryHandler ?? throw new ArgumentNullException(nameof(pilotFollowedClubQueryHandler));
    }

    public async IAsyncEnumerable<PilotRelevantClub> GetPilotRelevantClubs(Guid pilotId)
    {
        var seenClubIds = new HashSet<Guid>();

        // Clubs where the pilot is a confirmed member
        await foreach (var membership in _clubMemberQueryHandler.GetPilotMembershipDetails(pilotId))
        {
            if (!membership.IsRegistrationConfirmed)
                continue;

            if (!seenClubIds.Add(membership.ClubId))
                continue;

            var club = await _clubOverviewQueryHandler.GetClubOverview(membership.ClubId);
            if (club == null)
                continue;

            yield return new PilotRelevantClub(club.ClubId, club.Code, club.Name);
        }

        // Clubs the pilot follows
        await foreach (var followed in _pilotFollowedClubQueryHandler.GetFollowedClubs(pilotId))
        {
            if (!seenClubIds.Add(followed.ClubId))
                continue;

            var club = await _clubOverviewQueryHandler.GetClubOverview(followed.ClubId);
            if (club == null)
                continue;

            yield return new PilotRelevantClub(club.ClubId, club.Code, club.Name);
        }

        // Clubs where the pilot holds a role
        string[] allRoles = ["Administrator", "RaceOrganiser", "Trustee"];
        await foreach (var clubId in _clubMemberQueryHandler.GetClubIdsWithRoles(pilotId, allRoles))
        {
            if (!seenClubIds.Add(clubId))
                continue;

            var club = await _clubOverviewQueryHandler.GetClubOverview(clubId);
            if (club == null)
                continue;

            yield return new PilotRelevantClub(club.ClubId, club.Code, club.Name);
        }
    }
}
