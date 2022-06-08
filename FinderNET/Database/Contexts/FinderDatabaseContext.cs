using Microsoft.EntityFrameworkCore;
namespace FinderNET.Database.Contexts {
    public class FinderDatabaseContext : DbContext {
        public FinderDatabaseContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<UserLogs>().HasKey(table => new {
                table.guildId, table.userId
            });
            builder.Entity<Countdown>().HasKey(table => new {
                table.messageId, table.channelId, table.guildId
            });
        }

        

        public DbSet<Addons> addons { get; set; }
        public DbSet<UserLogs> userLogs { get; set; }
        public DbSet<Settings> settings { get; set; }
        public DbSet<Poll> polls { get; set; }
        public DbSet<Countdown> countdowns { get; set; }
        public DbSet<Tickets> tickets { get; set; }
    }
}