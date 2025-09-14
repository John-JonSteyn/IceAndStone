using IceAndStone.API.Data;
using IceAndStone.API.Repositories;
using IceAndStone.API.Repositories.Abstractions;
using IceAndStone.API.Services;
using IceAndStone.API.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                     ?? builder.Configuration.GetConnectionString("Default")
                     ?? throw new InvalidOperationException("Missing connection string 'Default'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var application = builder.Build();

using (var scope = application.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (application.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    application.UseSwagger();
    application.UseSwaggerUI();
}

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IRoundRepository, RoundRepository>();
builder.Services.AddScoped<IScoreRepository, ScoreRepository>();

builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IRoundService, RoundService>();
builder.Services.AddScoped<IScoreService, ScoreService>();

application.MapControllers();
application.MapGet("/health", () => Results.Ok(new { status = "ok" }));

application.Run();
