using FinderNET.Database.Models;
using FinderNET.Database.Contexts;

namespace FinderNET.Database.Repositories {
    public class SettingsRepository : Repository<SettingsModel> {
        public SettingsRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<SettingsModel> GetSettingsAsync(ulong guildId) {
            return await context.Set<SettingsModel>().FindAsync((long)guildId) ?? new SettingsModel();
        }

        public async Task AddSettingsAsync(ulong guildId, string key, string value) {
            var settings = await context.Set<SettingsModel>().FindAsync((long)guildId);
            if (settings == null) {
                await context.Set<SettingsModel>().AddAsync(new SettingsModel {
                    guildId = (long)guildId,
                    setting = key,
                    value = value
                });
                return;
            }
            settings.setting = key;
            settings.value = value;
            context.Set<SettingsModel>().Update(settings);
        }

        public async Task<bool> SettingsExists(ulong guildId, string key) {
            var settings = await context.Set<SettingsModel>().FindAsync((long)guildId);
            return settings != null && settings.setting.Contains(key);
        }
        
        public async Task<string?> GetSettingAsync(ulong guildId, string key) {
            var settings = await context.Set<SettingsModel>().FindAsync((long)guildId);
            return settings?.setting.Contains(key) ?? false ? settings.value : null;
        }
    }
}