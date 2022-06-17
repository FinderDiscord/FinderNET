using FinderNET.Database.Contexts;
using FinderNET.Database.Models;

namespace FinderNET.Database.Repositories {
    public class LevelingRepository : Repository<Leveling> {
        public LevelingRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<Leveling> GetLevelingAsync(ulong guildId, ulong userId) {
            return await context.Set<Leveling>().FindAsync((long)guildId, (long)userId) ?? new Leveling();
        }

        public async Task AddLevelingAsync(ulong guildId, ulong userId, int level, int exp) {
            var leveling = await context.Set<Leveling>().FindAsync((long)guildId, (long)userId);
            if (leveling == null) {
                await context.Set<Leveling>().AddAsync(new Leveling {
                    guildId = (long)guildId,
                    userId = (long)userId,
                    level = level,
                    exp = exp,
                });
                return;
            }
            leveling.guildId = (long)guildId;
            leveling.userId = (long)userId;
            leveling.level = level;
            leveling.exp = exp;
            context.Set<Leveling>().Update(leveling);
        }

        public async Task RemoveLevelingAsync(ulong guildId, ulong userId) {
            var leveling = await context.Set<Leveling>().FindAsync((long)guildId, (long)userId);
            if (leveling == null) return;
            context.Set<Leveling>().Remove(leveling);
        }
    }
}