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

<<<<<<< Updated upstream



        // UserLogs

        public async Task<UserLogs> GetUserLogs(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) {
                return new UserLogs() {
                    userId = userId, bans = 0, kicks = 0, warns = 0, mutes = 0
                };
            }
            return userLogs;
        }

        public async Task<int> GetUserBans(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) return 0;
            return userLogs.bans;
        }

        public async Task<int> GetUserKicks(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) return 0;
            return userLogs.kicks;
        }

        public async Task<int> GetUserWarns(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) return 0;
            return userLogs.warns;
        }

        public async Task<int> GetUserMutes(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) return 0;
            return userLogs.mutes;
        }

        public async Task SetUserBans(Int64 guildId, Int64 userId, int bans) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) {
                userLogs = context.Add(new UserLogs() {
                    guildId = guildId,
                    userId = userId,
                    bans = bans,
                    kicks = 0,
                    warns = 0,
                    mutes = 0
                }).Entity;
            } else {
                context.Entry(userLogs).CurrentValues.SetValues(new UserLogs() {
                    guildId = guildId,
                    userId = userId,
                    bans = bans,
                    kicks = userLogs.kicks,
                    warns = userLogs.warns,
                    mutes = userLogs.mutes
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task SetUserKicks(Int64 guildId, Int64 userId, int kicks) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) {
                userLogs = context.Add(new UserLogs() {
                    guildId = guildId,
                    userId = userId,
                    bans = 0,
                    kicks = kicks,
                    warns = 0,
                    mutes = 0
                }).Entity;
            } else {
                context.Entry(userLogs).CurrentValues.SetValues(new UserLogs() {
                    guildId = guildId,
                    userId = userId,
                    bans = userLogs.bans,
                    kicks = kicks,
                    warns = userLogs.warns,
                    mutes = userLogs.mutes
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task SetUserWarns(Int64 guildId, Int64 userId, int warns) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) {
                userLogs = context.Add(new UserLogs() {
                    guildId = guildId,
                    userId = userId,
                    bans = 0,
                    kicks = 0,
                    warns = warns,
                    mutes = 0
                }).Entity;
            } else {
                context.Entry(userLogs).CurrentValues.SetValues(new UserLogs() {
                    guildId = guildId,
                    userId = userId,
                    bans = userLogs.bans,
                    kicks = userLogs.kicks,
                    warns = warns,
                    mutes = userLogs.mutes
                });
=======
        public async Task<DateTime> GetDateTime(Int64 id) {
            using var context = contextFactory.CreateDbContext();
            var countdown = await context.countdowns.FindAsync(id);
            if (countdown == null) return DateTime.MinValue;
            return countdown.dateTime;
        }

        public async Task SetDateTime(Int64 id, DateTime dateTime) {
            using var context = contextFactory.CreateDbContext();
            var countdown = await context.countdowns.FindAsync(id);
            if (countdown == null) {
                context.Add(new Countdown { MessageId = id, dateTime = dateTime });
            } else {
                context.Entry(new Countdown() { MessageId = id, dateTime = dateTime }).Property(x => x.dateTime).IsModified = true;
>>>>>>> Stashed changes
            }
            await context.SaveChangesAsync();
        }

<<<<<<< Updated upstream
        public async Task SetUserMutes(Int64 guildId, Int64 userId, int mutes) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) {
                userLogs = context.Add(new UserLogs() {
                    guildId = guildId,
                    userId = userId,
                    bans = 0,
                    kicks = 0,
                    warns = 0,
                    mutes = mutes
                }).Entity;
            } else {
                context.Entry(userLogs).CurrentValues.SetValues(new UserLogs() {
                    guildId = guildId,
                    userId = userId,
                    bans = userLogs.bans,
                    kicks = userLogs.kicks,
                    warns = userLogs.warns,
                    mutes = mutes
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserLogs(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) return;
            context.Remove(userLogs);
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserBans(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) return;
            context.Entry(new UserLogs() {
                guildId = guildId,
                userId = userId,
                bans = 0,
                kicks = userLogs.kicks,
                warns = userLogs.warns,
                mutes = userLogs.mutes
            }).Property(x => x.mutes).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserKicks(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) return;
            context.Entry(new UserLogs() {
                guildId = guildId,
                userId = userId,
                bans = userLogs.bans,
                kicks = 0,
                warns = userLogs.warns,
                mutes = userLogs.mutes
            }).Property(x => x.mutes).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserWarns(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) return;
            context.Entry(new UserLogs() {
                guildId = guildId,
                userId = userId,
                bans = userLogs.bans,
                kicks = userLogs.kicks,
                warns = 0,
                mutes = userLogs.mutes
            }).Property(x => x.mutes).IsModified = true;
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserMutes(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var userLogs = await context.userLogs.FindAsync(guildId, userId);
            if (userLogs == null) return;
            context.Entry(new UserLogs() {
                guildId = guildId,
                userId = userId,
                bans = userLogs.bans,
                kicks = userLogs.kicks,
                warns = userLogs.warns,
                mutes = 0
            }).Property(x => x.mutes).IsModified = true;
            await context.SaveChangesAsync();
        }


        // Settings

        public async Task<string?> GetSettingsValue(Int64 guildId, string key) {
            using var context = contextFactory.CreateDbContext();
            var settings = await context.settings.FindAsync(guildId);
            if (settings == null) return null;
            return settings.value;
        }

        public async Task SetSettingsValue(Int64 guildId, string key, string value) {
            using var context = contextFactory.CreateDbContext();
            var settings = await context.settings.FindAsync(guildId);
            if (settings == null) {
                settings = context.Add(new Settings() {
                    guildId = guildId,
                    key = key,
                    value = value
                }).Entity;
            } else {
                context.Entry(settings).CurrentValues.SetValues(new Settings() {
                    guildId = guildId,
                    key = key,
                    value = value
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task RemoveSettingsValue(Int64 guildId, string key) {
            using var context = contextFactory.CreateDbContext();
            var settings = await context.settings.FindAsync(guildId);
            if (settings == null) return;
            context.Remove(settings);
            await context.SaveChangesAsync();
        }


        // Polls

        public async Task<Poll?> GetPoll(Int64 messageId) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) return null;
            return poll;
        }

        public async Task<List<string>> GetAnswers(Int64 messageId) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) return new List<string>();
            return poll.answers;
        }

        public async Task<List<Int64>> GetVotersIds(Int64 messageId) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) return new List<Int64>();
            return poll.votersId;
        }

        public async Task<Int64> GetVotersCount(Int64 messageId) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) return 0;
            return poll.votersId.Count;
        }

        public async Task SetPoll(Int64 messageId, List<string> answers, List<Int64> votersId) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) {
                poll = context.Add(new Poll() {
                    messageId = messageId,
                    answers = answers,
                    votersId = votersId
                }).Entity;
            } else {
                context.Entry(poll).CurrentValues.SetValues(new Poll() {
                    messageId = messageId,
                    answers = answers,
                    votersId = votersId
                });
            }
            await context.SaveChangesAsync();
        }
        public async Task SetAnswers(Int64 messageId, List<string> answers) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) {
                poll = context.Add(new Poll() {
                    messageId = messageId,
                    answers = answers
                }).Entity;
            } else {
                context.Entry(poll).CurrentValues.SetValues(new Poll() {
                    messageId = messageId,
                    answers = answers
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task SetVotersIds(Int64 messageId, List<Int64> votersId) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) {
                poll = context.Add(new Poll() {
                    messageId = messageId,
                    votersId = votersId
                }).Entity;
            } else {
                context.Entry(poll).CurrentValues.SetValues(new Poll() {
                    messageId = messageId,
                    votersId = votersId
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task AddAnswer(Int64 messageId, string answer) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) {
                poll = context.Add(new Poll() {
                    messageId = messageId,
                    answers = new List<string>() { answer }
                }).Entity;
            } else {
                var answers = poll.answers;
                answers.Add(answer);
                context.Entry(poll).CurrentValues.SetValues(new Poll() {
                    messageId = messageId,
                    answers = answers
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task AddVoterId(Int64 messageId, Int64 voterId) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) {
                poll = context.Add(new Poll() {
                    messageId = messageId,
                    votersId = new List<Int64>() { voterId }
                }).Entity;
            } else {
                var votersId = poll.votersId;
                votersId.Add(voterId);
                context.Entry(poll).CurrentValues.SetValues(new Poll() {
                    messageId = messageId,
                    votersId = votersId
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task RemoveAnswer(Int64 messageId, string answer) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) return;
            var answers = poll.answers;
            answers.Remove(answer);
            context.Entry(poll).CurrentValues.SetValues(new Poll() {
                messageId = messageId,
                answers = answers
            });
            await context.SaveChangesAsync();
        }

        public async Task RemoveVoter(Int64 messageId, Int64 voterId) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) return;
            var votersId = poll.votersId;
            votersId.Remove(voterId);
            context.Entry(poll).CurrentValues.SetValues(new Poll() {
                messageId = messageId,
                votersId = votersId
            });
            await context.SaveChangesAsync();
        }

        public async Task RemoveAllVoters(Int64 messageId) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) return;
            context.Remove(poll);
            await context.SaveChangesAsync();
        }

        public async Task RemoveAllAnswers(Int64 messageId) {
            using var context = contextFactory.CreateDbContext();
            var poll = await context.polls.FindAsync(messageId);
            if (poll == null) return;
            context.Remove(poll);
=======
        public async Task RemoveDateTime(Int64 id) {
            using var context = contextFactory.CreateDbContext();
            var countdown = await context.countdowns.FindAsync(id);
            if (countdown == null) return;
            context.Remove(countdown);
>>>>>>> Stashed changes
            await context.SaveChangesAsync();
        }
    }
}