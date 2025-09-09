using IceAndStone.API.Data;
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

application.MapControllers();
application.MapGet("/health", () => Results.Ok(new { status = "ok" }));

application.Run();
