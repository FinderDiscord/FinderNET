using FinderNET.Database.Contexts;
using FinderNET.Database.Models;

namespace FinderNET.Database.Repositories {
    public class CountdownRepository : Repository<CountdownModel> {
        public CountdownRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<CountdownModel> GetCountdownAsync(ulong messageId, ulong channelId, ulong guildId) {
            return await context.Set<CountdownModel>().FindAsync((long)messageId, (long)channelId, (long)guildId) ?? new CountdownModel();
        }

        public async Task AddCountdownAsync(ulong messageId, ulong channelId, ulong guildId, DateTime dateTime, ulong? pingUserId = null, ulong? pingRoleId = null) {
            var addons = await context.Set<CountdownModel>().FindAsync((long)messageId, (long)channelId, (long)guildId);
            if (addons == null) {
                await context.Set<CountdownModel>().AddAsync(new CountdownModel {
                    messageId = (long)messageId,
                    channelId = (long)channelId,
                    guildId = (long)guildId,
                    dateTime = dateTime,
                    pingUserId = (long?)pingUserId ?? null,
                    pingRoleId = (long?)pingRoleId ?? null
                });
                return;
            }
            addons.messageId = (long)messageId;
            addons.channelId = (long)channelId;
            addons.dateTime = dateTime;
            addons.pingUserId = (long?)pingUserId ?? null;
            addons.pingRoleId = (long?)pingRoleId ?? null;
            context.Set<CountdownModel>().Update(addons);
        }

        public async Task RemoveCountdownAsync(ulong messageId, ulong channelId, ulong guildId) {
            var addons = await context.Set<CountdownModel>().FindAsync((long)messageId, (long)channelId, (long)guildId);
            if (addons == null) return;
            context.Set<CountdownModel>().Remove(addons);
        }
    }
}