namespace NaeRaces.WebAPI.Shared;

public class HealthCheckResponse
{
    public bool SqlServer { get; set; }
    public bool KurrentDb { get; set; }
    public bool IsHealthy => SqlServer && KurrentDb;
}
