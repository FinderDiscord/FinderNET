using Microsoft.EntityFrameworkCore;
namespace FinderNET.Database.Contexts {
    public class FinderDatabaseContext : DbContext {
        public FinderDatabaseContext(DbContextOptions options) : base(options) { }
        public DbSet<Addons> addons { get; set; }
        public DbSet<ModerationLogs> moderationLogs { get; set; }
    }
}