using ChangeTrackerOwnedEntitiesReproducible.Domain;
using ChangeTrackerOwnedEntitiesReproducible.Infrastructure;
using ChangeTrackerOwnedEntitiesReproducible.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    IConfigurationSection psqlconf = builder.Configuration.GetSection("Psql");
    string connectionString = $"Host={psqlconf["Hostname"]};Port=5432;Database={psqlconf["Database"]};Username={psqlconf["User"]};Password={psqlconf["Pass"]};Include Error Detail=true";
    //string? connectionString = configuration.GetConnectionString("Database");
    Ensure.NotNullOrEmpty(connectionString);
                
    options.UseNpgsql(connectionString, npgsqlOptions =>
            npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
        .UseSnakeCaseNamingConvention()
        .LogTo(Console.WriteLine, LogLevel.Trace);
});

// Repositories go here
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();