using FinderNET.Database.Contexts;
using FinderNET.Database.Models;
using FinderNET.Modules.Helpers;
using Microsoft.EntityFrameworkCore;
namespace FinderNET.Database.Repositories {
    public class ItemInvRepository : Repository<ItemInvModel> {
        public ItemInvRepository(FinderDatabaseContext context) : base(context) { }
        
        public async Task<ItemInvModel?> GetItemsAsync(Int64 guildId, Int64 userId) {
            return await context.Set<ItemInvModel>().FindAsync(guildId, userId);
        }
        
        public async Task AddItemAsync(ulong guildId, ulong userId, Guid itemId, int amount) {
            var items = await context.Set<ItemInvModel>().FindAsync((long)guildId, (long)userId);
            if (items == null) {
                await context.Set<ItemInvModel>().AddAsync(new ItemInvModel() {
                    guildId = (long)guildId,
                    userId = (long)userId,
                    itemIds = new List<Guid>(),
                });
            }
            for (int i = 0; i < amount; i++) {
                items.itemIds.Add(itemId);
            }
            context.Set<ItemInvModel>().Update(items);
        }
        
        public async Task RemoveItemAsync(ulong guildId, ulong userId, Guid itemId, int amount) {
            var items = await context.Set<ItemInvModel>().FindAsync((long)guildId, (long)userId);
            if (items == null) return;
            if (items.itemIds.Contains(itemId)) {
                throw new Exception("Item not found in inventory");
            }
            if (items.itemIds.Count(x => x == itemId) < amount) {
                throw new Exception("Not enough items in inventory");
            }
            for (int i = 0; i < amount; i++) {
                items.itemIds.Remove(itemId);
            }
            context.Set<ItemInvModel>().Update(items);
        }
        
        public async Task<bool> ItemExistsAsync(ulong guildId, ulong userId, Guid itemId) {
            var items = await context.Set<ItemInvModel>().FindAsync((long)guildId, (long)userId);
            return items != null && items.itemIds.Contains(itemId);
        }

        public async Task<bool> ItemsExistsAsync(ulong guildId, ulong userId) {
            var items = await context.Set<ItemInvModel>().FindAsync((long)guildId, (long)userId);
            return items != null && items.itemIds.Count > 0;
        }
    }
}