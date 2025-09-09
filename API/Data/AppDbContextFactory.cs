using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IceAndStone.API.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            string connectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                ?? "Server=localhost;Port=3306;Database=iceandstone;User=root;Password=secret;TreatTinyAsBoolean=true";

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(connectionString, serverVersion);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
