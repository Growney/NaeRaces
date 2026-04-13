using KurrentDB.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaeRaces.Query.EntityFrameworkCore;
using NaeRaces.WebAPI.Shared;

namespace NaeRaces.WebAPI.Controllers;

[Route("api/[controller]")]
public class HealthController(NaeRacesQueryDbContext dbContext, KurrentDBClient kurrentDbClient) : Controller
{
    [HttpGet]
    public async Task<HealthCheckResponse> GetAsync()
    {
        var response = new HealthCheckResponse();

        try
        {
            response.SqlServer = await dbContext.Database.CanConnectAsync();
        }
        catch
        {
            response.SqlServer = false;
        }

        try
        {
            await kurrentDbClient.ReadAllAsync(Direction.Forwards, Position.Start, maxCount: 1)
                .ToListAsync();
            response.KurrentDb = true;
        }
        catch
        {
            response.KurrentDb = false;
        }

        return response;
    }
}
