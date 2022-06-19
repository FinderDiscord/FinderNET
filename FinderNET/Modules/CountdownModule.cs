using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FinderNET.Database.Repositories;
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
            if (time.Seconds <= 0) return sb.ToString();
            if (sb.Length != 0) sb.Append(", ");
            sb.Append($"{time.Seconds} second{(time.Seconds == 1 ? "" : "s")}");
            return sb.ToString();
        }
    }
}