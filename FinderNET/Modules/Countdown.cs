using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FinderNET.Database;
using System.Timers;
using Pathoschild.NaturalTimeParser.Parser;
namespace FinderNET.Modules {
    public class CountdownModule : ModuleBase {
        public CountdownModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
        [SlashCommand("countdown", "Countdown to a specific date or time")]
        public async Task CountdownCommand(string datetime, IMentionable? ping = null) {
            DateTime date;
            try {
                date = DateTime.Now.Offset(datetime);
            } catch (TimeParseFormatException) {
                await RespondAsync("Invalid date or time");
                return;
            }
            TimeSpan timeLeft = date - DateTime.Now;
            if (timeLeft.TotalSeconds < 0) {
                await RespondAsync("The date or time is in the past");
                return;
            }
            if (timeLeft.TotalDays > 365) {
                await RespondAsync("The date or time is too far in the future");
                return;
            }
            string message = "";
            if (timeLeft.Days > 0) {
                message += $"{timeLeft.Days} day{(timeLeft.Days == 1 ? "" : "s")}";
            }
            if (timeLeft.Hours > 0) {
                if (message != "") message += ", ";
                message += $"{timeLeft.Hours} hour{(timeLeft.Hours == 1 ? "" : "s")}";
            }
            if (timeLeft.Minutes > 0) {
                if (message != "") message += ", ";
                message += $"{timeLeft.Minutes} minute{(timeLeft.Minutes == 1 ? "" : "s")}";
            }
            if (timeLeft.Seconds > 0) {
                if (message != "") message += " and ";
                message += $"{timeLeft.Seconds} second{(timeLeft.Seconds == 1 ? "" : "s")}";
            }
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Countdown",
                Color = Color.Orange,
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Time left",
                        Value = message + " left"
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
            var messages = await GetOriginalResponseAsync();
            await dataAccessLayer.SetCountdown((Int64)messages.Id, (Int64)messages.Channel.Id, (Int64)Context.Guild.Id, date.ToUniversalTime());
            SocketRole? role = null;
            SocketGuildUser? user = null;
            if (ping != null) {
                if (ping as SocketGuildUser == null) {
                    if (ping as SocketRole == null) {
                        await RespondAsync("Invalid ping");
                        return;
                    }
                    role = (SocketRole)ping;
                    await dataAccessLayer.SetPingUserId((Int64)messages.Id, (Int64)messages.Channel.Id, (Int64)Context.Guild.Id, (Int64)role.Id);
                } else {
                    user = (SocketGuildUser)ping;
                    await dataAccessLayer.SetPingUserId((Int64)messages.Id, (Int64)messages.Channel.Id, (Int64)Context.Guild.Id, (Int64)user.Id);
                }
            }
        }
    }
    public static class CountdownTimer {
        private static System.Timers.Timer messageTimer;
        private static DiscordSocketClient client;
        private static DataAccessLayer dataAccessLayer;
        public static void StartTimer(DiscordSocketClient _client, DataAccessLayer _dataAccessLayer) {
            client = _client;
            dataAccessLayer = _dataAccessLayer;
            messageTimer = new System.Timers.Timer(5000);
            messageTimer.Elapsed += OnTimerElapsed;
            messageTimer.AutoReset = true;
            messageTimer.Enabled = true;
        }

        public static async void OnTimerElapsed(object source, ElapsedEventArgs e) {
            foreach (var c in await dataAccessLayer.GetAllCountdowns()) {
                SocketGuild guild = client.GetGuild((ulong)c.guildId);
                ITextChannel channel = (ITextChannel)guild.GetChannel((ulong)c.channelId);
                IUserMessage messages = (IUserMessage) await channel.GetMessageAsync((ulong)c.messageId);
                TimeSpan timeLeft = c.dateTime - DateTime.Now;
                if (timeLeft.TotalSeconds < 0) {
                    await messages.ModifyAsync(x => x.Embed = new EmbedBuilder() {
                        Title = "Countdown",
                        Color = Color.Orange,
                        Fields = new List<EmbedFieldBuilder> {
                            new EmbedFieldBuilder() {
                                Name = "Time left",
                                Value = "Times Up"
                            }
                        },
                        Footer = new EmbedFooterBuilder() {
                            Text = "FinderBot"
                        }
                    }.Build());
                    Int64? userId = await dataAccessLayer.GetPingUserId(c.messageId, c.channelId, c.guildId);
                    Int64? roleId = await dataAccessLayer.GetPingRoleId(c.messageId, c.channelId, c.guildId);
                    if (userId != null) {
                        SocketGuildUser user = guild.GetUser((ulong)userId);
                        if (user != null) {
                            await channel.SendMessageAsync($"{user.Mention} times up!");
                        }
                    } else if (roleId != null) {
                        SocketRole role = guild.GetRole((ulong)roleId);
                        if (role != null) {
                            await channel.SendMessageAsync($"{role.Mention} times up!");
                        }
                    }
                    await dataAccessLayer.RemoveCountdown(c.messageId, c.channelId, c.guildId);
                    continue;
                } else {
                    string message = "";
                    if (timeLeft.Days > 0) {
                        message += $"{timeLeft.Days} day{(timeLeft.Days == 1 ? "" : "s")}";
                    }
                    if (timeLeft.Hours > 0) {
                        if (message != "") message += ", ";
                        message += $"{timeLeft.Hours} hour{(timeLeft.Hours == 1 ? "" : "s")}";
                    }
                    if (timeLeft.Minutes > 0) {
                        if (message != "") message += ", ";
                        message += $"{timeLeft.Minutes} minute{(timeLeft.Minutes == 1 ? "" : "s")}";
                    }
                    if (timeLeft.Seconds > 0) {
                        if (message != "") message += " and ";
                        message += $"{timeLeft.Seconds} second{(timeLeft.Seconds == 1 ? "" : "s")}";
                    }
                    await messages.ModifyAsync((x => x.Embed = new EmbedBuilder() {
                        Title = "Countdown",
                        Color = Color.Orange,
                        Fields = new List<EmbedFieldBuilder> {
                            new EmbedFieldBuilder() {
                                Name = "Time left",
                                Value = message + " left"
                            }
                        },
                        Footer = new EmbedFooterBuilder() {
                            Text = "FinderBot"
                        }
                    }.Build()));
                }
            }
        }
    }
}