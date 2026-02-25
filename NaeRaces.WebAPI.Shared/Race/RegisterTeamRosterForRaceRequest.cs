using System.ComponentModel.DataAnnotations;

namespace NaeRaces.WebAPI.Shared.Race;

public class RegisterTeamRosterForRaceRequest
{
    [Required]
    public Guid TeamId { get; set; }

    [Required]
    public IEnumerable<TeamPilot> Pilots { get; set; } = Enumerable.Empty<TeamPilot>();

    public class TeamPilot
    {
        [Required]
        public Guid PilotId { get; set; }
        [Required]
        public int RacePackageId { get; set; }
    }
}

