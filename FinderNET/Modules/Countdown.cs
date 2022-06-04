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
            if (int.Parse(date.Start.Value.ToString("yyyy")) - int.Parse(DateTime.Now.ToString("yyyy")) > 0) {
                message += $"{int.Parse(date.Start.Value.ToString("yyyy")) - int.Parse(DateTime.Now.ToString("yyyy"))} year";
                if (int.Parse(date.Start.Value.ToString("yyyy")) - int.Parse(DateTime.Now.ToString("yyyy")) > 1) {
                    message += "s";
                }
                message += " ";
            }
            if (int.Parse(date.Start.Value.ToString("MM")) - int.Parse(DateTime.Now.ToString("MM")) > 0) {
                message += $"{int.Parse(date.Start.Value.ToString("MM")) - int.Parse(DateTime.Now.ToString("MM"))} month";
                if (int.Parse(date.Start.Value.ToString("MM")) - int.Parse(DateTime.Now.ToString("MM")) > 1) {
                    message += "s";
                }
                message += " ";
            }
            if (int.Parse(date.Start.Value.ToString("dd")) - int.Parse(DateTime.Now.ToString("dd")) > 0) {
                message += $"{int.Parse(date.Start.Value.ToString("dd")) - int.Parse(DateTime.Now.ToString("dd"))} day";
                if (int.Parse(date.Start.Value.ToString("dd")) - int.Parse(DateTime.Now.ToString("dd")) > 1) {
                    message += "s";
                }
                message += " ";
            }
            if (int.Parse(date.Start.Value.ToString("hh")) - int.Parse(DateTime.Now.ToString("hh")) > 0) {
                message += $"{int.Parse(date.Start.Value.ToString("hh")) - int.Parse(DateTime.Now.ToString("hh"))} hour";
                if (int.Parse(date.Start.Value.ToString("hh")) - int.Parse(DateTime.Now.ToString("hh")) > 1) {
                    message += "s";
                }
                message += " ";
            }
            if (int.Parse(date.Start.Value.ToString("mm")) - int.Parse(DateTime.Now.ToString("mm")) > 0) {
                message += $"{int.Parse(date.Start.Value.ToString("mm")) - int.Parse(DateTime.Now.ToString("mm"))} minute";
                if (int.Parse(date.Start.Value.ToString("mm")) - int.Parse(DateTime.Now.ToString("mm")) > 1) {
                    message += "s";
                }
                message += " ";
            }
            if (int.Parse(date.Start.Value.ToString("ss")) - int.Parse(DateTime.Now.ToString("ss")) > 0) {
                message += $"{int.Parse(date.Start.Value.ToString("ss")) - int.Parse(DateTime.Now.ToString("ss"))} second";
                if (int.Parse(date.Start.Value.ToString("ss")) - int.Parse(DateTime.Now.ToString("ss")) > 1) {
                    message += "s";
                }
                message += " ";
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
                if (c.dateTime > DateTime.Now) {
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
                    if (c.dateTime.ToString("yyyy") != DateTime.Now.ToString("yyyy")) {
                        message += $"{int.Parse(c.dateTime.ToString("yyyy")) - int.Parse(DateTime.Now.ToString("yyyy"))} year";
                        if (int.Parse(c.dateTime.ToString("yyyy")) - int.Parse(DateTime.Now.ToString("yyyy")) > 1) {
                            message += "s";
                        }
                        message += " ";
                    }
                    if (c.dateTime.ToString("MM") != DateTime.Now.ToString("MM")) {
                        message += $"{int.Parse(c.dateTime.ToString("MM")) - int.Parse(DateTime.Now.ToString("MM"))} month";
                        if (int.Parse(c.dateTime.ToString("MM")) - int.Parse(DateTime.Now.ToString("MM")) > 1) {
                            message += "s";
                        }
                        message += " ";
                    }
                    if (c.dateTime.ToString("dd") != DateTime.Now.ToString("dd")) {
                        message += $"{int.Parse(c.dateTime.ToString("dd")) - int.Parse(DateTime.Now.ToString("dd"))} day";
                        if (int.Parse(c.dateTime.ToString("dd")) - int.Parse(DateTime.Now.ToString("dd")) > 1) {
                            message += "s";
                        }
                        message += " ";
                    }
                    if (c.dateTime.ToString("hh") != DateTime.Now.ToString("hh")) {
                        message += $"{int.Parse(c.dateTime.ToString("hh")) - int.Parse(DateTime.Now.ToString("hh"))} hour";
                        if (int.Parse(c.dateTime.ToString("hh")) - int.Parse(DateTime.Now.ToString("hh")) > 1) {
                            message += "s";
                        }
                        message += " ";
                    }
                    if (c.dateTime.ToString("mm") != DateTime.Now.ToString("mm")) {
                        message += $"{int.Parse(c.dateTime.ToString("mm")) - int.Parse(DateTime.Now.ToString("mm"))} minute";
                        if (int.Parse(c.dateTime.ToString("mm")) - int.Parse(DateTime.Now.ToString("mm")) > 1) {
                            message += "s";
                        }
                        message += " ";
                    }
                    if (c.dateTime.ToString("ss") != DateTime.Now.ToString("ss")) {
                        message += $"{int.Parse(c.dateTime.ToString("ss")) - int.Parse(DateTime.Now.ToString("ss"))} second";
                        if (int.Parse(c.dateTime.ToString("ss")) - int.Parse(DateTime.Now.ToString("ss")) > 1) {
                            message += "s";
                        }
                        message += " ";
                    }
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Countdown", $"{c.dateTime.Year - DateTime.Now.Year} y {c.dateTime.Month - DateTime.Now.Month} month {c.dateTime.Day - DateTime.Now.Day} d {c.dateTime.Hour - DateTime.Now.Hour} h {c.dateTime.Minute - DateTime.Now.Minute} m {c.dateTime.Second - DateTime.Now.Second} s"));
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