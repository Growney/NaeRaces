namespace NaeRaces.Query.EntityFrameworkCore.Models;

public class PilotRaceRegistration
{
    public Guid PilotId { get; set; }
    public Guid RaceId { get; set; }
    public Guid RegistrationId { get; set; }
}
