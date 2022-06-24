using FinderNET.Database.Contexts;
using FinderNET.Database.Models;
using FinderNET.Modules.Helpers;
namespace FinderNET.Database.Repositories {
    public class ItemsRepository : Repository<ItemsModel> {
        public ItemsRepository(FinderDatabaseContext context) : base(context) { }
        
        public async Task<ItemsModel> GetItemsAsync(Int64 guildId, Int64 userId) {
            return await context.Set<ItemsModel>().FindAsync(guildId, userId) ?? new ItemsModel();
        }
        
        public async Task AddAddonAsync(ulong guildId, ulong userId, Items item) {
            var items = await context.Set<ItemsModel>().FindAsync((long)guildId);
            if (items == null) {
                await context.Set<ItemsModel>().AddAsync(new ItemsModel() {
                    guildId = (long)guildId,
                    userId = (long)userId,
                    items = new List<Items> { item }
                });
                return;
            }
            if (!items.items.Contains(item)) {
                items.items.Add(item);
            }
            context.Set<ItemsModel>().Update(items);
        }
        
        public async Task RemoveAddonAsync(ulong guildId, ulong userId, Items item) {
            var items = await context.Set<ItemsModel>().FindAsync((long)guildId, (long)userId);
            if (items == null) return;      
            if (items.items.Contains(item)) {
                items.items.Remove(item);
            }
            context.Set<ItemsModel>().Update(items);
        }
        
        public async Task<bool> ItemExistsAsync(ulong guildId, ulong userId, Items item) {
            var items = await context.Set<ItemsModel>().FindAsync((long)guildId, (long)userId);
            return items != null && items.items.Contains(item);
        }

        public async Task<bool> AddonsExistsAsync(ulong guildId, ulong userId) {
            var items = await context.Set<ItemsModel>().FindAsync((long)guildId, (long)userId);
            return items != null && items.items.Count > 0;
        }
    }
}