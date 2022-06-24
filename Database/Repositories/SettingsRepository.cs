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
                var dict = new Dictionary<string, string>() { { key, value } };
                await context.Set<SettingsModel>().AddAsync(new SettingsModel {
                    guildId = (long)guildId,
                    settings = new List<IDictionary<string, string>>() {
                        dict
                    }
                });
                return;
            }
            if (settings.settings.Any(d => d.ContainsKey(key))) {
                settings.settings.First(d => d.ContainsKey(key)).Add(key, value);
            } else {
                settings.settings.Add(new Dictionary<string, string>() { { key, value } });
            }
            context.Set<SettingsModel>().Update(settings);
        }

        public async Task RemoveAddonAsync(ulong guildId, string value) {
            var settings = await context.Set<SettingsModel>().FindAsync(guildId);
            if (settings == null) return;      
            if (settings.settings.Any(d => d.ContainsKey(value))) {
                settings.settings.First(d => d.ContainsKey(value)).Remove(value);
            }
            context.Set<SettingsModel>().Update(settings);
        }

        public async Task<bool> SettingsExists(ulong guildId, string key) {
            var settings = await context.Set<SettingsModel>().FindAsync((long)guildId);
            return settings != null && settings.settings.Any(d => d.ContainsKey(key));
        }
        
        public async Task<string?> GetSettingAsync(ulong guildId, string key) {
            var settings = await context.Set<SettingsModel>().FindAsync((long)guildId);
            return settings == null ? null : settings.settings.Any(d => d.ContainsKey(key)) ? settings.settings.First(d => d.ContainsKey(key)).First(k => k.Key == key).Value : null;
        }
    }
}