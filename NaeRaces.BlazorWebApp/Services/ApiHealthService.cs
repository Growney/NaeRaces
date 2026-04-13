using System.Net.Http.Json;
using NaeRaces.WebAPI.Shared;

namespace NaeRaces.BlazorWebApp.Services;

public class ApiHealthService(IHttpClientFactory httpClientFactory)
{
    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = httpClientFactory.CreateClient("NaeRaces.ServerAPI.Health");
            var response = await client.GetFromJsonAsync<HealthCheckResponse>("api/health", cancellationToken);
            return response?.IsHealthy == true;
        }
        catch
        {
            return false;
        }
    }
}
