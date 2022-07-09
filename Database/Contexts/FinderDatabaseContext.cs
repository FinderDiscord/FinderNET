using Microsoft.EntityFrameworkCore;
using FinderNET.Database.Models;
using FinderNET.Database.Repositories;
namespace FinderNET.Database.Contexts {
    public class FinderDatabaseContext : DbContext {
        public FinderDatabaseContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<UserLogsModel>().HasKey(table => new {
                table.guildId, table.userId
            });
            builder.Entity<CountdownModel>().HasKey(table => new {
                table.messageId, table.channelId, table.guildId
            });
            builder.Entity<LevelingModel>().HasKey(table => new {
                table.guildId, table.userId
            });
            builder.Entity<EconomyModel>().HasKey(table => new {
                table.guildId, table.userId
            });
            builder.Entity<SettingsModel>().HasKey(table => new {
                table.guildId, table.key
            });
            builder.Entity<TicketsModel>().HasKey(table => new {
                table.guildId, table.supportChannelId
            });
            builder.Entity<ItemInvModel>().HasKey(table => new {
                table.guildId, table.userId,
            });
        }
        
        public DbSet<AddonsModel> addons { get; set; }
        public DbSet<UserLogsModel> userLogs { get; set; }
        public DbSet<SettingsModel> settings { get; set; }
        public DbSet<PollsModel> polls { get; set; }
        public DbSet<CountdownModel> countdowns { get; set; }
        public DbSet<TicketsModel> tickets { get; set; }
        public DbSet<LevelingModel> leveling { get; set; }
        public DbSet<EconomyModel> economy { get; set; }
        public DbSet<ItemInvModel > itemInventory { get; set; }
    }
}