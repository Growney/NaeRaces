using Microsoft.EntityFrameworkCore;

namespace NaeRaces.WebAPI.Data;

public class OpenIddictDbContext : DbContext
{
    public OpenIddictDbContext(DbContextOptions<OpenIddictDbContext> options)
        : base(options)
    {
    }
}
