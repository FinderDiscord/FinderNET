using FinderNET.Database.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FinderNET.Database {
    public class DataAccessLayer {
        private readonly IDbContextFactory<FinderDatabaseContext> contextFactory;

        public DataAccessLayer(IDbContextFactory<FinderDatabaseContext> _contextFactory) {
            contextFactory = _contextFactory;
        }

        // Addons
        public List<string> GetAddons(Int64 id) {
            using var context = contextFactory.CreateDbContext();
            var addons = context.addons.Find(id);
            if (addons == null) {
                return new List<string>();
            }
            return addons.addons;
        }

        public async Task SetAddons(Int64 id, List<string> addons) {
            using var context = contextFactory.CreateDbContext();
            var addonsList = await context.addons.FindAsync(id);
            if (addonsList != null) {
                context.Entry(new Addons() { Id = id, addons = addons }).Property(x => x.addons).IsModified = true;
            } else {
                context.Add(new Addons { Id = id, addons = addons });
            }
            await context.SaveChangesAsync();
        }

        public async Task AddAddons(Int64 id, string addon) {
            using var context = contextFactory.CreateDbContext();
            var addons = await context.addons.FindAsync(id);
            if (addons != null) {
                addons.addons.Add(addon);
            } else {
                context.Add(new Addons { Id = id, addons = new List<string> { addon } });
            }
            await context.SaveChangesAsync();
        }

        public async Task RemoveAllAddons(Int64 id) {
            using var context = contextFactory.CreateDbContext();
            var addons = await context.addons.FindAsync(id);
            if (addons == null) return;
            context.Remove(addons);
            await context.SaveChangesAsync();
        }

        public async Task RemoveAddon(Int64 id, string addon) {
            using var context = contextFactory.CreateDbContext();
            var addons = await context.addons.FindAsync(id);
            if (addons == null) return;
            if (!addons.addons.Contains(addon)) return;
            addons.addons.Remove(addon);
            context.Entry(new Addons() { Id = id, addons = addons.addons }).Property(x => x.addons).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task RemoveAddons(Int64 id, List<string> addons) {
            using var context = contextFactory.CreateDbContext();
            var addonsList = await context.addons.FindAsync(id);
            if (addonsList == null) return;
            foreach (var addon in addons) {
                if (!addonsList.addons.Contains(addon)) continue;
                addonsList.addons.Remove(addon);
            }
            context.Entry(new Addons() { Id = id, addons = addonsList.addons }).Property(x => x.addons).IsModified = true;
            await context.SaveChangesAsync();
        }




        // ModerationLogs

        public UserLogs GetUserLogs(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = context.moderationLogs.Find(guildId);
            if (userLogs == null) {
                return new UserLogs() {
                    userId = userId, bans = 0, kicks = 0, warns = 0, mutes = 0
                };
            }
            return userLogs.userLogs;
        }

        public int GetUserBans(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = context.moderationLogs.Find(guildId);
            if (userLogs == null) {
                return 0;
            }
            return userLogs.userLogs.bans;
        }

        public int GetUserKicks(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = context.moderationLogs.Find(guildId);
            if (userLogs == null) {
                return 0;
            }
            return userLogs.userLogs.kicks;
        }

        public int GetUserWarns(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = context.moderationLogs.Find(guildId);
            if (userLogs == null) {
                return 0;
            }
            return userLogs.userLogs.warns;
        }

        public int GetUserMutes(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = context.moderationLogs.Find(guildId);
            if (userLogs == null) {
                return 0;
            }
            return userLogs.userLogs.mutes;
        }

        public async Task SetUserBans(Int64 guildId, Int64 userId, int bans) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.moderationLogs.FindAsync(guildId);
            if (userLogs == null) {
                userLogs = context.Add(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = new UserLogs() {
                        userId = userId,
                        bans = bans,
                        kicks = 0,
                        warns = 0,
                        mutes = 0
                    }
                }).Entity;
            } else {
                userLogs.userLogs.bans = bans;
                context.Entry(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = userLogs.userLogs
                }).Property(x => x.userLogs.bans).IsModified = true;
            }
            await context.SaveChangesAsync();
        }

        public async Task SetUserKicks(Int64 guildId, Int64 userId, int kicks) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.moderationLogs.FindAsync(guildId);
            if (userLogs == null) {
                userLogs = context.Add(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = new UserLogs() {
                        userId = userId,
                        bans = 0,
                        kicks = kicks,
                        warns = 0,
                        mutes = 0
                    }
                }).Entity;
            } else {
                userLogs.userLogs.kicks = kicks;
                context.Entry(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = userLogs.userLogs
                }).Property(x => x.userLogs.kicks).IsModified = true;
            }
            await context.SaveChangesAsync();
        }

        public async Task SetUserWarns(Int64 guildId, Int64 userId, int warns) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.moderationLogs.FindAsync(guildId);
            if (userLogs == null) {
                userLogs = context.Add(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = new UserLogs() {
                        userId = userId,
                        bans = 0,
                        kicks = 0,
                        warns = warns,
                        mutes = 0
                    }
                }).Entity;
            } else {
                userLogs.userLogs.warns = warns;
                context.Entry(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = userLogs.userLogs
                }).Property(x => x.userLogs.warns).IsModified = true;
            }
            await context.SaveChangesAsync();
        }

        public async Task SetUserMutes(Int64 guildId, Int64 userId, int mutes) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.moderationLogs.FindAsync(guildId);
            if (userLogs == null) {
                userLogs = context.Add(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = new UserLogs() {
                        userId = userId,
                        bans = 0,
                        kicks = 0,
                        warns = 0,
                        mutes = mutes
                    }
                }).Entity;
            } else {
                userLogs.userLogs.mutes = mutes;
                context.Entry(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = userLogs.userLogs
                }).Property(x => x.userLogs.mutes).IsModified = true;
            }
            await context.SaveChangesAsync();
        }

        public async Task SetUserLogs(Int64 guildId, Int64 userId, int bans, int kicks, int warns, int mutes) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.moderationLogs.FindAsync(guildId);
            if (userLogs == null) {
                userLogs = context.Add(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = new UserLogs() {
                        userId = userId,
                        bans = bans,
                        kicks = kicks,
                        warns = warns,
                        mutes = mutes
                    }
                }).Entity;
            } else {
                userLogs.userLogs.bans = bans;
                userLogs.userLogs.kicks = kicks;
                userLogs.userLogs.warns = warns;
                userLogs.userLogs.mutes = mutes;
                context.Entry(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = userLogs.userLogs
                }).Property(x => x.userLogs.bans).IsModified = true;
                context.Entry(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = userLogs.userLogs
                }).Property(x => x.userLogs.kicks).IsModified = true;
                context.Entry(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = userLogs.userLogs
                }).Property(x => x.userLogs.warns).IsModified = true;
                context.Entry(new ModerationLogs() {
                    guildId = guildId,
                    userLogs = userLogs.userLogs
                }).Property(x => x.userLogs.mutes).IsModified = true;
            }
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserLogs(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.moderationLogs.FindAsync(guildId);
            if (userLogs == null) return;
            context.Remove(userLogs);
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserBans(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.moderationLogs.FindAsync(guildId);
            if (userLogs == null) return;
            userLogs.userLogs.bans = 0;
            context.Entry(new ModerationLogs() {
                guildId = guildId,
                userLogs = userLogs.userLogs
            }).Property(x => x.userLogs.bans).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserKicks(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.moderationLogs.FindAsync(guildId);
            if (userLogs == null) return;
            userLogs.userLogs.kicks = 0;
            context.Entry(new ModerationLogs() {
                guildId = guildId,
                userLogs = userLogs.userLogs
            }).Property(x => x.userLogs.kicks).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserWarns(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.moderationLogs.FindAsync(guildId);
            if (userLogs == null) return;
            userLogs.userLogs.warns = 0;
            context.Entry(new ModerationLogs() {
                guildId = guildId,
                userLogs = userLogs.userLogs
            }).Property(x => x.userLogs.warns).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserMutes(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.moderationLogs.FindAsync(guildId);
            if (userLogs == null) return;
            userLogs.userLogs.mutes = 0;
            context.Entry(new ModerationLogs() {
                guildId = guildId,
                userLogs = userLogs.userLogs
            }).Property(x => x.userLogs.mutes).IsModified = true;
            await context.SaveChangesAsync();
        }
    }
}