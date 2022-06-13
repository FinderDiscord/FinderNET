using FinderNET.Database.Contexts;
using FinderNET.Database.Models;

namespace FinderNET.Database.Repositories {
    public class AddonsRepository : Repository<Addons> {
        public AddonsRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<Addons> GetAddonsAsync(ulong guildId) {
            return await context.Set<Addons>().FindAsync((long)guildId) ?? new Addons();
        }

        public async Task AddAddonAsync(ulong guildId, string addon) {
            var addons = await context.Set<Addons>().FindAsync(guildId);
            if (addons == null) {
                await context.Set<Addons>().AddAsync(new Addons {
                    guildId = (long)guildId,
                    addons = new List<string> { addon }
                });
                return;
            }
            if (!addons.addons.Contains(addon)) {
                addons.addons.Add(addon);
            }
            context.Set<Addons>().Update(addons);
        }

        public async Task RemoveAddonAsync(ulong guildId, string addon) {
            var addons = await context.Set<Addons>().FindAsync(guildId);
            if (addons == null) return;      
            if (addons.addons.Contains(addon)) {
                addons.addons.Remove(addon);
            }
            context.Set<Addons>().Update(addons);

        }

        public async Task<bool> AddonExistsAsync(ulong guildId, string addon) {
            var addons = await GetAddonsAsync(guildId);
            return addons.addons.Contains(addon);
        }

        public async Task<bool> AddonsExistsAsync(ulong guildId) {
            var addons = await GetAddonsAsync(guildId);
            return addons.addons.Count > 0;
        }
    }
}