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
        
        public async Task AddTempbanTime(ulong guildId, ulong userId, DateTime time) {
            var userLogs = await context.Set<UserLogsModel>().FindAsync((long)guildId, (long)userId);
            if (userLogs == null) {
                await context.Set<UserLogsModel>().AddAsync(new UserLogsModel {
                    guildId = (long)guildId,
                    userId = (long)userId,
                    tempBan = time
                });
                return;
            }
            userLogs.guildId = (long)guildId;
            userLogs.userId = (long)userId;
            userLogs.tempBan = time;
            context.Set<UserLogsModel>().Update(userLogs);
        }
        
        public async Task RemoveTempbanTime(ulong guildId, ulong userId) {
            var userLogs = await context.Set<UserLogsModel>().FindAsync((long)guildId, (long)userId);
            if (userLogs == null) {
                return;
            }
            userLogs.guildId = (long)guildId;
            userLogs.userId = (long)userId;
            userLogs.tempBan = null;
            context.Set<UserLogsModel>().Update(userLogs);
        }
        
        public async Task AddTempmuteTime(ulong guildId, ulong userId, DateTime time) {
            var userLogs = await context.Set<UserLogsModel>().FindAsync((long)guildId, (long)userId);
            if (userLogs == null) {
                await context.Set<UserLogsModel>().AddAsync(new UserLogsModel {
                    guildId = (long)guildId,
                    userId = (long)userId,
                    tempMute = time
                });
                return;
            }
            userLogs.guildId = (long)guildId;
            userLogs.userId = (long)userId;
            userLogs.tempMute = time;
            context.Set<UserLogsModel>().Update(userLogs);
        }
        
        public async Task RemoveTempmuteTime(ulong guildId, ulong userId) {
            var userLogs = await context.Set<UserLogsModel>().FindAsync((long)guildId, (long)userId);
            if (userLogs == null) {
                return;
            }
            userLogs.guildId = (long)guildId;
            userLogs.userId = (long)userId;
            userLogs.tempMute = null;
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
        
        public async Task<bool> IsUnbanned(ulong guildId, ulong userId) {
            var userLogs = await context.Set<UserLogsModel>().FindAsync((long)guildId, (long)userId);
            if (userLogs == null) return false;
            return userLogs.tempBan > DateTime.UtcNow;
        }
        
        public async Task<bool> IsUnmuted(ulong guildId, ulong userId) {
            var userLogs = await context.Set<UserLogsModel>().FindAsync((long)guildId, (long)userId);
            if (userLogs == null) return false;
            return userLogs.tempMute > DateTime.UtcNow;
        }
    }
}