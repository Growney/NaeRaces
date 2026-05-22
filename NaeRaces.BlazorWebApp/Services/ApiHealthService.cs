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

            Console.WriteLine($"Health Check Poll: {client.BaseAddress}");

            var response = await client.GetFromJsonAsync<HealthCheckResponse>("api/health", cancellationToken);
            Console.WriteLine("Checking health");
            return response?.IsHealthy == true;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}
