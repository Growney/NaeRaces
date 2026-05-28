using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NaeRaces.Query.Projections;

public class PilotRelevantClubsProjection
{
    public record PilotRelevantClub(Guid PilotId, Guid ClubId, string ClubCode, string ClubName, PilotClubRelationship Relationship);
    public record PilotClubRelationship(bool IsMember, bool IsFollowing, IEnumerable<string> Roles);

    private class PilotRelevantClubsProjectionSnapshot
    {
        public record ClubInfo(Guid ClubId, string Code, string Name);
        public record PilotClubRelationship(Guid PilotId, Guid ClubId, bool IsMember, bool IsFollowing, IEnumerable<string> Roles);

        public IEnumerable<ClubInfo> Clubs { get; set; } = Enumerable.Empty<ClubInfo>();
        public IEnumerable<PilotClubRelationship> Relationships { get; set; } = Enumerable.Empty<PilotClubRelationship>();

    }

    private void Restore(PilotRelevantClubsProjectionSnapshot snapshot)
    {
        foreach (var club in snapshot.Clubs)
        {
            _clubInfo.Add(club.ClubId, (club.Code, club.Name));
        }

        foreach (var relationship in snapshot.Relationships)
        {
            if (!_pilotClubRelationships.TryGetValue(relationship.PilotId, out var clubRelationships))
            {
                clubRelationships = new Dictionary<Guid, PilotClubRelationship>();
                _pilotClubRelationships.Add(relationship.PilotId, clubRelationships);
            }
            clubRelationships[relationship.ClubId] = new PilotClubRelationship(relationship.IsMember, relationship.IsFollowing, relationship.Roles);
        }
    }

    private PilotRelevantClubsProjectionSnapshot Snapshot()
    {
        List<PilotRelevantClubsProjectionSnapshot.ClubInfo> clubs = new();

        foreach (var clubKvp in _clubInfo)
        {
            clubs.Add(new PilotRelevantClubsProjectionSnapshot.ClubInfo(clubKvp.Key, clubKvp.Value.clubCode, clubKvp.Value.clubName));
        }

        List<PilotRelevantClubsProjectionSnapshot.PilotClubRelationship> relationships = new();

        foreach (var pilotKvp in _pilotClubRelationships)
        {
            foreach (var clubKvp in pilotKvp.Value)
            {
                relationships.Add(new PilotRelevantClubsProjectionSnapshot.PilotClubRelationship(pilotKvp.Key, clubKvp.Key, clubKvp.Value.IsMember, clubKvp.Value.IsFollowing, clubKvp.Value.Roles));
            }
        }

        return new PilotRelevantClubsProjectionSnapshot()
        {
            Clubs = clubs,
            Relationships = relationships
        };
    }

    private Dictionary<Guid, (string clubCode, string clubName)> _clubInfo = new();
    private Dictionary<Guid, Dictionary<Guid, PilotClubRelationship>> _pilotClubRelationships = new();

    public IEnumerable<PilotRelevantClub> GetRelevantClubs(Guid pilotId)
    {
        if (!_pilotClubRelationships.TryGetValue(pilotId, out var clubRelationships))
        {
            yield break;
        }

        foreach (var relationshipKvp in clubRelationships)
        {
            if (!_clubInfo.TryGetValue(relationshipKvp.Key, out var clubInfo))
            {
                continue;
            }

            yield return new PilotRelevantClub(pilotId, relationshipKvp.Key, clubInfo.clubCode, clubInfo.clubName, relationshipKvp.Value);
        }
    }

    private void When(ClubDetailsChanged changed)
    {
        _clubInfo[changed.ClubId] = (changed.Code, changed.Name);
    }
    private void When(ClubFormed formed)
    {
        _clubInfo[formed.ClubId] = (formed.Code, formed.Name);
    }

    private void When(PilotFollowedClub followed)
    {
        if (!_pilotClubRelationships.TryGetValue(followed.PilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(followed.PilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(followed.ClubId, out var relationship))
        {
            clubRelationships[followed.ClubId] = new PilotClubRelationship(false, true, []);
        }
        else
        {
            clubRelationships[followed.ClubId] = clubRelationships[followed.ClubId] with { IsFollowing = true };
        }
    }
    private void When(PilotUnfollowedClub unfollowed)
    {
        if (!_pilotClubRelationships.TryGetValue(unfollowed.PilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(unfollowed.PilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(unfollowed.ClubId, out var relationship))
        {
            clubRelationships[unfollowed.ClubId] = new PilotClubRelationship(false, false, []);
        }
        else
        {
            clubRelationships[unfollowed.ClubId] = clubRelationships[unfollowed.ClubId] with { IsFollowing = false };
        }
    }

    private void When(ClubMemberRoleAssigned assigned)
    {
        if (!_pilotClubRelationships.TryGetValue(assigned.PilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(assigned.PilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(assigned.ClubId, out var relationship))
        {
            clubRelationships[assigned.ClubId] = new PilotClubRelationship(false, false, [assigned.Role]);
        }
        else
        {
            clubRelationships[assigned.ClubId] = clubRelationships[assigned.ClubId] with { Roles = [.. clubRelationships[assigned.ClubId].Roles, assigned.Role] };
        }
    }

    private void When(ClubMemberRoleRevoked revoked)
    {
        if (!_pilotClubRelationships.TryGetValue(revoked.PilotId, out var clubRelationships))
        {
            clubRelationships = new();
            _pilotClubRelationships.Add(revoked.PilotId, clubRelationships);
        }

        if (!clubRelationships.TryGetValue(revoked.ClubId, out var relationship))
        {
            clubRelationships[revoked.ClubId] = new PilotClubRelationship(false, false, []);
        }
        else
        {
            clubRelationships[revoked.ClubId] = clubRelationships[revoked.ClubId] with { Roles = [.. clubRelationships[revoked.ClubId].Roles.Where(x=> x != revoked.Role)] };
        }
    }

}
