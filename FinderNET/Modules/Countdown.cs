using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FinderNET.Database.Repositories;
using System.Timers;
using Pathoschild.NaturalTimeParser.Parser;
using System.Text;

namespace FinderNET.Modules {
    public class CountdownModule : InteractionModuleBase<SocketInteractionContext> {
        private readonly CountdownRepository countdownRepository;
        public CountdownModule(CountdownRepository _countdownRepository) {
            countdownRepository = _countdownRepository;
        }
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
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Countdown",
                Color = Color.Orange,
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Time left",
                        Value = HumanizeTime(timeLeft) + " left"
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
            var messages = await GetOriginalResponseAsync();
            if (ping != null) {
                await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Countdown", $"{ping.Mention}"));
                if (ping as SocketRole != null) {
                    await countdownRepository.AddCountdownAsync(messages.Id, Context.Channel.Id, Context.Guild.Id, date.ToUniversalTime(), null, ((SocketRole)ping).Id);
                } else if (ping as SocketGuildUser != null) {
                    await countdownRepository.AddCountdownAsync(messages.Id, Context.Channel.Id, Context.Guild.Id, date.ToUniversalTime(), ((SocketGuildUser)ping).Id, null);
                } else {
                    await RespondAsync("Invalid ping");
                }
            } else {
                await countdownRepository.AddCountdownAsync(messages.Id, Context.Channel.Id, Context.Guild.Id, date.ToUniversalTime(), null, null);
            }
            await countdownRepository.SaveAsync();
        }

        public static string HumanizeTime(TimeSpan time) {
            StringBuilder sb = new StringBuilder();
            if (time.Days > 0) {
                sb.Append($"{time.Days} day{(time.Days == 1 ? "" : "s")}");
            }
            if (time.Hours > 0) {
                if (sb.Length != 0) sb.Append(", ");
                sb.Append($"{time.Hours} hour{(time.Hours == 1 ? "" : "s")}");
            }
            if (time.Minutes > 0) {
                if (sb.Length != 0) sb.Append(", ");
                sb.Append($"{time.Minutes} minute{(time.Minutes == 1 ? "" : "s")}");
            }
            if (time.Seconds > 0) {
                if (sb.Length != 0) sb.Append(", ");
                sb.Append($"{time.Seconds} second{(time.Seconds == 1 ? "" : "s")}");
            }
            return sb.ToString();
        }
    }
    public static class CountdownTimer {
        private static System.Timers.Timer messageTimer;
        private static DiscordSocketClient client;
        private static CountdownRepository countdownRepository;
        public static void StartTimer(DiscordSocketClient _client, CountdownRepository _countdownRepository) {
            client = _client;
            countdownRepository = _countdownRepository;
            messageTimer = new System.Timers.Timer(3000);
            messageTimer.Elapsed += OnTimerElapsed;
            messageTimer.AutoReset = true;
            messageTimer.Enabled = true;
        }

        public static async void OnTimerElapsed(object source, ElapsedEventArgs e) {
            foreach (var c in countdownRepository.GetAll()) {
                SocketGuild guild = client.GetGuild((ulong)c.guildId);
                ITextChannel channel = (ITextChannel)guild.GetChannel((ulong)c.channelId);
                IUserMessage messages = (IUserMessage) await channel.GetMessageAsync((ulong)c.messageId);

                if (c.dateTime < DateTime.UtcNow || messages == null) {
                    if (messages != null) {
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
                        Int64? userId = c.pingUserId;
                        Int64? roleId = c.pingRoleId;
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
                    }
                    await countdownRepository.RemoveCountdownAsync((ulong)c.messageId, (ulong)c.channelId, (ulong)c.guildId);
                    await countdownRepository.SaveAsync();
                    continue;
                }
                TimeSpan timeLeft = c.dateTime - DateTime.Now;
                await messages.ModifyAsync((x => x.Embed = new EmbedBuilder() {
                    Title = "Countdown",
                    Color = Color.Orange,
                    Fields = new List<EmbedFieldBuilder> {
                        new EmbedFieldBuilder() {
                            Name = "Time left",
                            Value = CountdownModule.HumanizeTime(timeLeft) + " left"
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