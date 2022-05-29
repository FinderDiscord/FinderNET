using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
namespace FinderNET.Database.Contexts {
    public class FinderDatabaseContextFactory : IDesignTimeDbContextFactory<FinderDatabaseContext> {
        public FinderDatabaseContext CreateDbContext(string[] args) {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true);
            var optionsBuilder = new DbContextOptionsBuilder<FinderDatabaseContext>();
            optionsBuilder.UseNpgsql("Server=localhost;Database=finder;Username=postgres;Password=password;");
            return new FinderDatabaseContext(optionsBuilder.Options);
        }
    }
}