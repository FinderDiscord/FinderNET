using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
namespace FinderNET.Database.Contexts {
    public class FinderDatabaseContextFactory : IDesignTimeDbContextFactory<FinderDatabaseContext> {
        public FinderDatabaseContext CreateDbContext(string[] args) {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true).Build();
            var optionsBuilder = new DbContextOptionsBuilder<FinderDatabaseContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("Default"));
            return new FinderDatabaseContext(optionsBuilder.Options);
        }
    }
}