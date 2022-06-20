using FinderNET.Database.Models;
using FinderNET.Database.Contexts;

namespace FinderNET.Database.Repositories {
    public class SettingsRepository : Repository<SettingsModel> {
        public SettingsRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<SettingsModel> GetSettingsAsync(ulong guildId, string key) {
            return await context.Set<SettingsModel>().FindAsync((long)guildId, key) ?? new SettingsModel();
        }

        public async Task AddSettingsAsync(ulong guildId, string key, string value) {
            var settings = await context.Set<SettingsModel>().FindAsync((long)guildId, key);
            if (settings == null) {
                await context.Set<SettingsModel>().AddAsync(new SettingsModel {
                    guildId = (long)guildId,
                    key = key,
                    value = value
                });
                return;
            }
            settings.guildId = (long)guildId;
            settings.key = key;
            settings.value = value;
            context.Set<SettingsModel>().Update(settings);
        }

        public async Task RemoveSettingsAsync(ulong guildId, string key) {
            var settings = await context.Set<SettingsModel>().FindAsync((long)guildId, key);
            if (settings == null) return;
            context.Set<SettingsModel>().Remove(settings);
        }

        public bool SettingsExists(ulong guildId, string key) {
            return context.Set<SettingsModel>().Find((long)guildId, key) != null;
        }
    }
}