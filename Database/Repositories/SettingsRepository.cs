using FinderNET.Database.Models;
using FinderNET.Database.Contexts;

namespace FinderNET.Database.Repositories {
    public class SettingsRepository : Repository<Settings> {
        public SettingsRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<Settings> GetSettingsAsync(ulong guildId, string key) {
            return await context.Set<Settings>().FindAsync((long)guildId, key) ?? new Settings();
        }

        public async Task AddSettingsAsync(ulong guildId, string key, string value) {
            var settings = await context.Set<Settings>().FindAsync((long)guildId, key);
            if (settings == null) {
                await context.Set<Settings>().AddAsync(new Settings {
                    guildId = (long)guildId,
                    key = key,
                    value = value
                });
                return;
            }
            settings.guildId = (long)guildId;
            settings.key = key;
            settings.value = value;
            context.Set<Settings>().Update(settings);
        }

        public async Task RemoveSettingsAsync(ulong guildId, string key) {
            var settings = await context.Set<Settings>().FindAsync((long)guildId, key);
            if (settings == null) return;
            context.Set<Settings>().Remove(settings);
        }

        public bool SettingsExists(ulong guildId, string key) {
            return context.Set<Settings>().Find((long)guildId, key) != null;
        }
    }
}