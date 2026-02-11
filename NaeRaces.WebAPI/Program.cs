using NaeRaces.Query.EntityFrameworkCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddKurrentDbEventDbLite(x =>
{

});
builder.Services.AddSqlServerNaeRacesQueryDbContext(builder.Configuration);
builder.Services.AddNaeRacesEntityFrameworkCoreQueryReactions();
builder.Services.AddNaeRacesEntityFrameworkCoreQueryHandlers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
