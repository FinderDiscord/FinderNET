using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FinderNET.Database;
using Chronic;
using System.Timers;

namespace FinderNET.Modules {
    public class CountdownModule : ModuleBase {
        public CountdownModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
        [SlashCommand("countdown", "Countdown to a specific date or time")]
        public async Task CountdownCommand(string datetime, IMentionable? ping = null) {
            var pharser = new Parser();
            var date = pharser.Parse(datetime);
            if (date == null || date.Start == null) {
                await RespondAsync("Invalid date or time");
                return;
            }
            if (date.Start.Value < DateTime.Now) {
                await RespondAsync("The date or time is in the past");
                return;
            }
            if (date.Start.Value.Year - DateTime.Now.Year > 2) {
                await RespondAsync("The date or time is too far in the future");
                return;
            }
            string message = "";
            if (date.Start.Value.Second - DateTime.Now.Second > 0)
                message = $"{date.Start.Value.Second - DateTime.Now.Second} seconds{(date.Start.Value.Second - DateTime.Now.Second == 1 ? "" : "s")} {message}";
            if (date.Start.Value.Minute - DateTime.Now.Minute > 0)
                message = $"{date.Start.Value.Minute - DateTime.Now.Minute} minute{(date.Start.Value.Minute - DateTime.Now.Minute == 1 ? "" : "s")} {message}";
            if (date.Start.Value.Hour - DateTime.Now.Hour > 0)
                message = $"{date.Start.Value.Hour - DateTime.Now.Hour} hour{(date.Start.Value.Hour - DateTime.Now.Hour == 1 ? "" : "s")} {message}";
            if (date.Start.Value.Day - DateTime.Now.Day > 0)
                message = $"{date.Start.Value.Day - DateTime.Now.Day} day{(date.Start.Value.Day - DateTime.Now.Day == 1 ? "" : "s")} {message}";
            if (date.Start.Value.Month - DateTime.Now.Month > 0)
                message = $"{date.Start.Value.Month - DateTime.Now.Month} month{(date.Start.Value.Month - DateTime.Now.Month == 1 ? "" : "s")} {message}";
            if (date.Start.Value.Year - DateTime.Now.Year > 0)
                message = $"{date.Start.Value.Year - DateTime.Now.Year} year{(date.Start.Value.Year - DateTime.Now.Year == 1 ? "" : "s")} {message}";
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
            await dataAccessLayer.SetCountdown((Int64)messages.Id, (Int64)messages.Channel.Id, (Int64)Context.Guild.Id, date.ToTime().ToUniversalTime());
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
                if (c.dateTime < DateTime.UtcNow) {
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
                    if (c.dateTime.Second - DateTime.Now.Second > 0)
                        message = $"{c.dateTime.Second - DateTime.Now.Second} seconds{(c.dateTime.Second - DateTime.Now.Second == 1 ? "" : "s")} {messages}";
                    if (c.dateTime.Minute - DateTime.Now.Minute > 0)
                        message = $"{c.dateTime.Minute - DateTime.Now.Minute} minute{(c.dateTime.Minute - DateTime.Now.Minute == 1 ? "" : "s")} {messages}";
                    if (c.dateTime.Hour - DateTime.Now.Hour > 0)
                        message = $"{c.dateTime.Hour - DateTime.Now.Hour} hour{(c.dateTime.Hour - DateTime.Now.Hour == 1 ? "" : "s")} {messages}";
                    if (c.dateTime.Day - DateTime.Now.Day > 0)
                        message = $"{c.dateTime.Day - DateTime.Now.Day} day{(c.dateTime.Day - DateTime.Now.Day == 1 ? "" : "s")} {messages}";
                    if (c.dateTime.Month - DateTime.Now.Month > 0)
                        message = $"{c.dateTime.Month - DateTime.Now.Month} month{(c.dateTime.Month - DateTime.Now.Month == 1 ? "" : "s")} {messages}";
                    if (c.dateTime.Year - DateTime.Now.Year > 0)
                        message = $"{c.dateTime.Year - DateTime.Now.Year} year{(c.dateTime.Year - DateTime.Now.Year == 1 ? "" : "s")} {messages}";
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