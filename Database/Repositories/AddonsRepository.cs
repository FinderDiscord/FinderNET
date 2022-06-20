using FinderNET.Database.Contexts;
using FinderNET.Database.Models;
using FinderNET.Modules.Helpers.Enums;

namespace FinderNET.Database.Repositories {
    public class AddonsRepository : Repository<AddonsModel> {
        public AddonsRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<AddonsModel> GetAddonsAsync(ulong guildId) {
            return await context.Set<AddonsModel>().FindAsync((long)guildId) ?? new AddonsModel();
        }

        public async Task AddAddonAsync(ulong guildId, Addons addon) {
            var addons = await context.Set<AddonsModel>().FindAsync(guildId);
            if (addons == null) {
                await context.Set<AddonsModel>().AddAsync(new AddonsModel {
                    guildId = (long)guildId,
                    addons = new List<Addons> { addon }
                });
                return;
            }
            if (!addons.addons.Contains(addon)) {
                addons.addons.Add(addon);
            }
            context.Set<AddonsModel>().Update(addons);
        }

        public async Task RemoveAddonAsync(ulong guildId, Addons addon) {
            var addons = await context.Set<AddonsModel>().FindAsync(guildId);
            if (addons == null) return;      
            if (addons.addons.Contains(addon)) {
                addons.addons.Remove(addon);
            }
            context.Set<AddonsModel>().Update(addons);

        }

        public async Task<bool> AddonExistsAsync(ulong guildId, Addons addon) {
            var addons = await GetAddonsAsync(guildId);
            return addons.addons.Contains(addon);
        }

        public async Task<bool> AddonsExistsAsync(ulong guildId) {
            var addons = await GetAddonsAsync(guildId);
            return addons.addons.Count > 0;
        }
    }
}