using FinderNET.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using Discord;

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
            }
            await context.SaveChangesAsync();
        }

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
        }

        // polls

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
        }


        // countdown
        public async Task<Countdown?> GetCountdown(Int64 messageId, Int64 channelId, Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var countdown = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (countdown == null) return null;
            return countdown;
        }

        public async Task<List<Countdown?>> GetCountdownsInChannel(Int64 channelId, Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var countdowns = await context.countdowns.Where(x => x.channelId == channelId && x.guildId == guildId).ToListAsync();
            return countdowns;
        }

        public async Task<List<Countdown?>> GetCountdownsInGuild(Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var countdowns = await context.countdowns.Where(x => x.guildId == guildId).ToListAsync();
            return countdowns;
        }

        public async Task<List<Countdown?>> GetAllCountdowns() {
            using var context = contextFactory.CreateDbContext();
            var countdowns = await context.countdowns.ToListAsync();
            return countdowns;
        }

        public async Task SetCountdown(Int64 messageId, Int64 channelId, Int64 guildId, DateTime endTime) {
            using var context = contextFactory.CreateDbContext();
            var countdown = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (countdown == null) {
                countdown = context.Add(new Countdown() {
                    messageId = messageId,
                    channelId = channelId,
                    guildId = guildId,
                    dateTime = endTime
                }).Entity;
            } else {
                context.Entry(countdown).CurrentValues.SetValues(new Countdown() {
                    messageId = messageId,
                    channelId = channelId,
                    guildId = guildId,
                    dateTime = endTime
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task RemoveCountdown(Int64 messageId, Int64 channelId, Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var countdown = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (countdown == null) return;
            context.Remove(countdown);
            await context.SaveChangesAsync();
        }

        public async Task RemoveAllCountdownsInChannel(Int64 channelId, Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var countdowns = await context.countdowns.Where(x => x.channelId == channelId && x.guildId == guildId).ToListAsync();
            foreach (var countdown in countdowns) {
                context.Remove(countdown);
            }
            await context.SaveChangesAsync();
        }

        public async Task RemoveAllCountdownsInGuild(Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var countdowns = await context.countdowns.Where(x => x.guildId == guildId).ToListAsync();
            foreach (var countdown in countdowns) {
                context.Remove(countdown);
            }
            await context.SaveChangesAsync();
        }

        public async Task RemoveAllCountdowns() {
            using var context = contextFactory.CreateDbContext();
            var countdowns = await context.countdowns.ToListAsync();
            foreach (var countdown in countdowns) {
                context.Remove(countdown);
            }
            await context.SaveChangesAsync();
        }

        public async Task<DateTime?> GetCountdownEndTime(Int64 messageId, Int64 channelId, Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var countdown = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (countdown == null) return null;
            return countdown.dateTime;
        }

        public async Task SetCountdownEndTime(Int64 messageId, Int64 channelId, Int64 guildId, DateTime endTime) {
            using var context = contextFactory.CreateDbContext();
            var countdown = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (countdown == null) return;
            context.Entry(countdown).CurrentValues.SetValues(new Countdown() {
                messageId = messageId,
                channelId = channelId,
                guildId = guildId,
                dateTime = endTime
            });
            await context.SaveChangesAsync();
        }

        public async Task<Int64?> GetPingUserId(Int64 messageId, Int64 channelId, Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var ping = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (ping == null) return null;
            return ping.pingUserId;
        }

        public async Task SetPingUserId(Int64 messageId, Int64 channelId, Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var ping = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (ping == null) return;
            context.Entry(ping).CurrentValues.SetValues(new Countdown() {
                messageId = messageId,
                channelId = channelId,
                guildId = guildId,
                pingUserId = userId
            });
            await context.SaveChangesAsync();
        }

        public async Task<Int64?> GetPingRoleId(Int64 messageId, Int64 channelId, Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var ping = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (ping == null) return null;
            return ping.pingRoleId;
        }

        public async Task SetPingRoleId(Int64 messageId, Int64 channelId, Int64 guildId, Int64 roleId) {
            using var context = contextFactory.CreateDbContext();
            var ping = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (ping == null) return;
            context.Entry(ping).CurrentValues.SetValues(new Countdown() {
                messageId = messageId,
                channelId = channelId,
                guildId = guildId,
                pingRoleId = roleId
            });
            await context.SaveChangesAsync();
        }

        public async Task RemovePingUserId(Int64 messageId, Int64 channelId, Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var ping = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (ping == null) return;
            context.Entry(ping).CurrentValues.SetValues(new Countdown() {
                messageId = messageId,
                channelId = channelId,
                guildId = guildId,
                pingUserId = null
            });
            await context.SaveChangesAsync();
        }

        public async Task RemovePingRoleId(Int64 messageId, Int64 channelId, Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var ping = await context.countdowns.FindAsync(messageId, channelId, guildId);
            if (ping == null) return;
            context.Entry(ping).CurrentValues.SetValues(new Countdown() {
                messageId = messageId,
                channelId = channelId,
                guildId = guildId,
                pingRoleId = null
            });
            await context.SaveChangesAsync();
        }

        // tickets

        public async Task<List<Tickets>> GetTickets(Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            return await context.tickets.Where(x => x.guildId == guildId).ToListAsync();
        }

        public async Task<Tickets?> GetTicket(Int64 ticketId) {
            using var context = contextFactory.CreateDbContext();
            return await context.tickets.FindAsync(ticketId);
        }

        public async Task<Int64?> GetIntroMessageId(Int64 ticketId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return null;
            return ticket.introMessageId;
        }

        public async Task SetIntroMessageId(Int64 ticketId, Int64 messageId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            context.Entry(ticket).CurrentValues.SetValues(new Tickets() {
                ticketId = ticketId,
                introMessageId = messageId
            });
            await context.SaveChangesAsync();
        }

        public async Task RemoveIntroMessageId(Int64 ticketId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            context.Entry(ticket).CurrentValues.SetValues(new Tickets() {
                ticketId = ticketId,
                introMessageId = null
            });
            await context.SaveChangesAsync();
        }

        public async Task<string?> GetTicketName(Int64 ticketId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return null;
            return ticket.name;
        }

        public async Task SetTicketName(Int64 ticketId, string name) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            context.Entry(ticket).CurrentValues.SetValues(new Tickets() {
                ticketId = ticketId,
                name = name
            });
            await context.SaveChangesAsync();
        }

        public async Task RemoveTicketName(Int64 ticketId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            context.Entry(ticket).CurrentValues.SetValues(new Tickets() {
                ticketId = ticketId,
                name = null
            });
            await context.SaveChangesAsync();
        }

        public async Task AddTicket(Int64 ticketId, Int64? guildId, Int64? channelId, Int64? userId, string? name, Int64? introMessageId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = new Tickets() {
                ticketId = ticketId,
                guildId = guildId,
                supportChannelId = channelId,
                userIds = new List<long?>() { userId },
                name = name,
                introMessageId = introMessageId,
                claimedUserId = new List<long>()
            };
            context.tickets.Add(ticket);
            await context.SaveChangesAsync();
        }

        public async Task RemoveTicket(Int64 ticketId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            context.tickets.Remove(ticket);
            await context.SaveChangesAsync();
        }

        public async Task<Int64?> GetGuildId(Int64 ticketId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return null;
            return ticket.guildId;
        }

        public async Task<Int64?> GetChannelId(Int64 ticketId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return null;
            return ticket.supportChannelId;
        }

        public async Task SetGuildId(Int64 ticketId, Int64 guildId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            context.Entry(ticket).CurrentValues.SetValues(new Tickets() {
                ticketId = ticketId,
                guildId = guildId
            });
            await context.SaveChangesAsync();
        }

        public async Task SetChannelId(Int64 ticketId, Int64 channelId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            context.Entry(ticket).CurrentValues.SetValues(new Tickets() {
                ticketId = ticketId,
                supportChannelId = channelId
            });
            await context.SaveChangesAsync();
        }

        public async Task<List<Int64?>> GetTicketUsers(Int64 ticketId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return new List<Int64?>();
            return ticket.userIds;
        }

        public async Task AddTicketUser(Int64 ticketId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            if (ticket.userIds.Contains(userId)) return;
            ticket.userIds.Add(userId);
            await context.SaveChangesAsync();
        }

        public async Task RemoveTicketUser(Int64 ticketId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            if (!ticket.userIds.Contains(userId)) return;
            ticket.userIds.Remove(userId);
            await context.SaveChangesAsync();
        }

        public async Task<List<Int64>> GetClaimedUserId(Int64 ticketId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return new List<Int64>();
            return ticket.claimedUserId;
        }

        public async Task SetClaimedUserId(Int64 ticketId, List<Int64> userIds) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            context.Entry(ticket).CurrentValues.SetValues(new Tickets() {
                ticketId = ticketId,
                claimedUserId = userIds
            });
            await context.SaveChangesAsync();
        }

        public async Task AddClaimedUserId(Int64 ticketId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            if (ticket.claimedUserId.Contains(userId)) return;
            ticket.claimedUserId.Add(userId);
            await context.SaveChangesAsync();
        }

        public async Task RemoveClaimedUserId(Int64 ticketId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var ticket = await context.tickets.FindAsync(ticketId);
            if (ticket == null) return;
            if (!ticket.claimedUserId.Contains(userId)) return;
            ticket.claimedUserId.Remove(userId);
            await context.SaveChangesAsync();
        }

        // leveling
        public async Task<int> GetLevel(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var user = await context.leveling.FindAsync(guildId, userId);
            if (user == null) return 0;
            return user.level;
        }

        public async Task SetLevel(Int64 guildId, Int64 userId, int level) {
            using var context = contextFactory.CreateDbContext();
            var user = await context.leveling.FindAsync(guildId, userId);
            if (user == null) {
                user = new Leveling() {
                    guildId = guildId,
                    userId = userId,
                    level = level
                };
                context.leveling.Add(user);
            } else {
                context.Entry(user).CurrentValues.SetValues(new Leveling() {
                    guildId = guildId,
                    userId = userId,
                    level = level
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task<int> GetExp(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var user = await context.leveling.FindAsync(guildId, userId);
            if (user == null) return 0;
            return user.exp;
        }

        public async Task SetExp(Int64 guildId, Int64 userId, int exp) {
            using var context = contextFactory.CreateDbContext();
            var user = await context.leveling.FindAsync(guildId, userId);
            if (user == null) {
                user = new Leveling() {
                    guildId = guildId,
                    userId = userId,
                    exp = exp
                };
                context.leveling.Add(user);
            } else {
                context.Entry(user).CurrentValues.SetValues(new Leveling() {
                    guildId = guildId,
                    userId = userId,
                    exp = exp
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task ResetLevel(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var user = await context.leveling.FindAsync(guildId, userId);
            if (user == null) {
                return;
            } else {
                context.Entry(user).CurrentValues.SetValues(new Leveling() {
                    guildId = guildId,
                    userId = userId,
                    level = 0,
                    exp = user.exp
                });
            }
            await context.SaveChangesAsync();
        }

        public async Task ResetExp(Int64 guildId, Int64 userId) {
            using var context = contextFactory.CreateDbContext();
            var user = await context.leveling.FindAsync(guildId, userId);
            if (user == null) {
                return;
            } else {
                context.Entry(user).CurrentValues.SetValues(new Leveling() {
                    guildId = guildId,
                    userId = userId,
                    level = user.level,
                    exp = 0
                });
            }
            await context.SaveChangesAsync();
        }


    }
}