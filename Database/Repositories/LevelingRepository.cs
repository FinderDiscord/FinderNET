using FinderNET.Database.Contexts;
using FinderNET.Database.Models;

namespace FinderNET.Database.Repositories {
    public class LevelingRepository : Repository<LevelingModel> {
        public LevelingRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<LevelingModel> GetLevelingAsync(ulong guildId, ulong userId) {
            return await context.Set<LevelingModel>().FindAsync((long)guildId, (long)userId) ?? new LevelingModel();
        }

        public async Task AddLevelingAsync(ulong guildId, ulong userId, int level, int exp) {
            var leveling = await context.Set<LevelingModel>().FindAsync((long)guildId, (long)userId);
            if (leveling == null) {
                await context.Set<LevelingModel>().AddAsync(new LevelingModel {
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
            context.Set<LevelingModel>().Update(leveling);
        }

        public async Task RemoveLevelingAsync(ulong guildId, ulong userId) {
            var leveling = await context.Set<LevelingModel>().FindAsync((long)guildId, (long)userId);
            if (leveling == null) return;
            context.Set<LevelingModel>().Remove(leveling);
        }
    }
}