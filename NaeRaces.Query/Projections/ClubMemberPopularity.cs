using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;

namespace NaeRaces.Query.Projections;

public class ClubMemberPopularity
{
    public record ClubPopularity(Guid ClubId, string ClubCode, string ClubName, int Members, int Followers);

    private class ClubMemberPopularitySnapshot
    {
        public record Popularity(Guid ClubId, string ClubCode, string ClubName, IEnumerable<Guid> Members, IEnumerable<Guid> Followers);
        public IEnumerable<Popularity> Clubs { get; set; } = Enumerable.Empty<Popularity>();
    }

    //We don't just hold the number of members or followers as the events need to be idempotent so we need to ensure that we are adding and removing the correct pilot
    private record ClubDetails(string Code, string Name, List<Guid> Members, List<Guid> Followers);

    private Dictionary<Guid, ClubDetails> _details = new();

    private void Restore(ClubMemberPopularitySnapshot snapshot)
    {
        foreach (var club in snapshot.Clubs)
        {
            _details[club.ClubId] = new ClubDetails(club.ClubCode, club.ClubName, club.Members.ToList(), club.Followers.ToList());
        }
    }

    private ClubMemberPopularitySnapshot Snapshot()
    {
        List<ClubMemberPopularitySnapshot.Popularity> clubPopularity = new();

        foreach (var kvp in _details)
        {
            clubPopularity.Add(new ClubMemberPopularitySnapshot.Popularity(kvp.Key, kvp.Value.Code, kvp.Value.Name, kvp.Value.Members, kvp.Value.Followers));
        }

        return new ClubMemberPopularitySnapshot()
        {
            Clubs = clubPopularity
        };
    }

    public IEnumerable<ClubPopularity> GetTop(int quantity)
        => _details.OrderByDescending(x => x.Value.Members.Count + x.Value.Followers.Count)
        .Take(quantity)
        .Select(x => new ClubPopularity(x.Key, x.Value.Code, x.Value.Name, x.Value.Members.Count, x.Value.Followers.Count));

    public IEnumerable<ClubPopularity> GetAll()
        => _details.Select(x => new ClubPopularity(x.Key, x.Value.Code, x.Value.Name, x.Value.Members.Count, x.Value.Followers.Count));

    private void When(ClubFormed formed)
    {
        _details[formed.ClubId] = new ClubDetails(formed.Code, formed.Name, [], []);
    }

    private void When(ClubDetailsChanged changed)
    {
        _details[changed.ClubId] = _details[changed.ClubId] with { Code = changed.Code, Name = changed.Name };
    }

    private void When(PilotFollowedClub follow)
    {
        var club = _details[follow.ClubId];

        if (!club.Followers.Contains(follow.PilotId))
        {
            club.Followers.Add(follow.PilotId);
        }
    }
    private void When(PilotUnfollowedClub unfollow)
    {
        var club = _details[unfollow.ClubId];

        club.Followers.Remove(unfollow.PilotId);
    }

    private void When(PilotClubMembershipConfirmed eventObj) => AddMember(eventObj.ClubId, eventObj.PilotId);
    private void When(PilotClubMembershipManuallyConfirmed eventObj) => AddMember(eventObj.ClubId, eventObj.PilotId);
    private void When(PilotClubMembershipCancelled eventObj) => RemoveMember(eventObj.ClubId, eventObj.PilotId);
    private void When(PilotClubMembershipExpired eventObj) => RemoveMember(eventObj.ClubId, eventObj.PilotId);
    private void When(PilotClubMembershipRevoked eventObj) => RemoveMember(eventObj.ClubId, eventObj.PilotId);

    private void AddMember(Guid clubId, Guid memberId)
    {
        var club = _details[clubId];

        if (!club.Members.Contains(memberId))
        {
            club.Members.Add(memberId);
        }
    }
    private void RemoveMember(Guid clubId, Guid memberId)
    {
        var club = _details[clubId];

        club.Members.Remove(memberId);
    }
}
