using NaeRaces.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaeRaces.Query.Projections;

public class ClubDescription
{
    private Dictionary<Guid, string> _descriptions = new();

    private class ClubDescriptionSnapshot
    {
        public record ClubDescription(Guid ClubId, string Description);

        public IEnumerable<ClubDescription> Descriptions { get; set; } = Enumerable.Empty<ClubDescription>();
    }

    private ClubDescriptionSnapshot Snapshot()
    {
        List<ClubDescriptionSnapshot.ClubDescription> descriptions = new();
        foreach(var description in _descriptions)
        {
            descriptions.Add(new ClubDescriptionSnapshot.ClubDescription(description.Key, description.Value));
        }
        return new ClubDescriptionSnapshot()
        {
            Descriptions = descriptions
        };
    }
    private void Restore(ClubDescriptionSnapshot snapshot)
    {
        foreach(var description in snapshot.Descriptions)
        {
            _descriptions[description.ClubId] = description.Description;
        }
    }

    private void When(ClubDescriptionSet set)
    {
        _descriptions[set.ClubId] = set.Description;
    }

    public string? GetClubDescription(Guid clubId)
    {
        if(!_descriptions.TryGetValue(clubId, out string? description))
        {
            return null;
        }
        return description;
    }
}
