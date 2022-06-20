using FinderNET.Database.Contexts;
using FinderNET.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FinderNET.Database.Repositories {
    public class UserLogsRepository : Repository<UserLogsModel> {
        public UserLogsRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<UserLogsModel> GetUserLogsAsync(ulong guildId, ulong userId) {
            return await context.Set<UserLogsModel>().FindAsync((long)guildId, (long)userId) ?? new UserLogsModel();
        }

        public async Task AddUserLogsAsync(ulong guildId, ulong userId, int bans, int kicks, int warns, int mutes) {
            var userLogs = await context.Set<UserLogsModel>().FindAsync((long)guildId, (long)userId);
            if (userLogs == null) {
                await context.Set<UserLogsModel>().AddAsync(new UserLogsModel {
                    guildId = (long)guildId,
                    userId = (long)userId,
                    bans = bans,
                    kicks = kicks,
                    warns = warns,
                    mutes = mutes
                });
                return;
            }
            userLogs.guildId = (long)guildId;
            userLogs.userId = (long)userId;
            userLogs.bans = bans;
            userLogs.kicks = kicks;
            userLogs.warns = warns;
            userLogs.mutes = mutes;
            context.Set<UserLogsModel>().Update(userLogs);
        }

        public async Task RemoveUserLogsAsync(ulong guildId, ulong userId) {
            var userLogs = await context.Set<UserLogsModel>().FindAsync((long)guildId, (long)userId);
            if (userLogs == null) return;
            context.Set<UserLogsModel>().Remove(userLogs);
        }

        public async Task<bool> UserLogsExistsAsync(ulong guildId, ulong userId) {
            return await context.Set<UserLogsModel>().FindAsync((long)guildId, (long)userId) != null;
        }
    }
}