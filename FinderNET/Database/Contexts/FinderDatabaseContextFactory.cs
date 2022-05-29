using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Finder.Database;

public class FinderDesignTimeFactory : IDesignTimeDbContextFactory<FinderDatabaseContext>
{
    public FinderDatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FinderDatabaseContext>();
        optionsBuilder.UseNpgsql("Server=localhost;Database=finder;Username=postgres;Password=password;");

        return new FinderDatabaseContext(optionsBuilder.Options, null, null);
    }
}