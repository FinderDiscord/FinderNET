using FinderNET.Database.Contexts;
using FinderNET.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FinderNET.Database.Repositories {
    public class UserLogsRepository : Repository<UserLogs> {
        public UserLogsRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<UserLogs> GetUserLogsAsync(ulong guildId, ulong userId) {
            return await context.Set<UserLogs>().FindAsync((long)guildId, (long)userId) ?? new UserLogs();
        }

        public async Task AddUserLogsAsync(ulong guildId, ulong userId, int bans, int kicks, int warns, int mutes) {
            var userLogs = await context.Set<UserLogs>().FindAsync((long)guildId, (long)userId);
            if (userLogs == null) {
                await context.Set<UserLogs>().AddAsync(new UserLogs {
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
            context.Set<UserLogs>().Update(userLogs);
        }

        public async Task RemoveUserLogsAsync(ulong guildId, ulong userId) {
            var userLogs = await context.Set<UserLogs>().FindAsync((long)guildId, (long)userId);
            if (userLogs == null) return;
            context.Set<UserLogs>().Remove(userLogs);
        }

        public async Task<bool> UserLogsExistsAsync(ulong guildId, ulong userId) {
            return await context.Set<UserLogs>().FindAsync((long)guildId, (long)userId) != null;
        }
    }
}