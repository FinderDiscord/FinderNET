using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Rest;
using FinderNET.Database;
using Chronic;

namespace FinderNET.Modules {
    public class Countdown : ModuleBase {
        public Countdown(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
        [SlashCommand("countdown", "Countdown to a specific date or time")]
        public async Task CountdownCommand(string datetime) {
            var pharser = new Parser();
            var date = pharser.Parse(datetime);
            if (date == null || date.End == null) {
                await RespondAsync("Invalid date or time");
                return;
            }
            if (date.End.Value < DateTime.Now) {
                await RespondAsync("The date or time is in the past");
                return;
            }
            if (date.End.Value.Year - DateTime.Now.Year > 2) {
                await RespondAsync("The date or time is too far in the future");
                return;
            }
            string message = "";
            await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Countdown", $"{Context.User.Username} requested a countdown to {date.End.Value}"));
            await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Countdown", $"{int.Parse(date.End.Value.ToString("mm")) - int.Parse(DateTime.Now.ToString("mm"))} minute{(int.Parse(date.End.Value.ToString("mm")) == 1 ? "" : "s")}"));
            if (int.Parse(date.End.Value.ToString("mm")) - int.Parse(DateTime.Now.ToString("mm")) > 0) {
                message = $"{int.Parse(date.End.Value.ToString("mm")) - int.Parse(DateTime.Now.ToString("mm"))} minute{(int.Parse(date.End.Value.ToString("mm")) - int.Parse(DateTime.Now.ToString("mm")) == 1 ? "" : "s")} {message}";
            }
            await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Countdown", $"{int.Parse(date.End.Value.ToString("hh")) - int.Parse(DateTime.Now.ToString("hh"))} hour{(int.Parse(date.End.Value.ToString("hh")) == 1 ? "" : "s")}"));
            if (int.Parse(date.End.Value.ToString("hh")) - int.Parse(DateTime.Now.ToString("hh")) > 0) {
                message = $"{int.Parse(date.End.Value.ToString("hh")) - int.Parse(DateTime.Now.ToString("hh"))} hour{(int.Parse(date.End.Value.ToString("hh")) == 1 ? "" : "s")} {message}";
            }
            await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Countdown", $"{int.Parse(date.End.Value.ToString("dd")) - int.Parse(DateTime.Now.ToString("dd"))} day{(int.Parse(date.End.Value.ToString("dd")) == 1 ? "" : "s")}"));
            if (int.Parse(date.End.Value.ToString("dd")) - int.Parse(DateTime.Now.ToString("dd")) > 0) {
                message = $"{int.Parse(date.End.Value.ToString("dd")) - int.Parse(DateTime.Now.ToString("dd"))} day{(int.Parse(date.End.Value.ToString("dd")) == 1 ? "" : "s")} {message}";
            }
            await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Countdown", $"{int.Parse(date.End.Value.ToString("MM")) - int.Parse(DateTime.Now.ToString("MM"))} month{(int.Parse(date.End.Value.ToString("MM")) == 1 ? "" : "s")}"));
            if (int.Parse(date.End.Value.ToString("MM")) - int.Parse(DateTime.Now.ToString("MM")) > 0) {
                message = $"{int.Parse(date.End.Value.ToString("MM")) - int.Parse(DateTime.Now.ToString("MM"))} month{(int.Parse(date.End.Value.ToString("MM")) == 1 ? "" : "s")} {message}";
            }
            await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Countdown", $"{int.Parse(date.End.Value.ToString("yyyy")) - int.Parse(DateTime.Now.ToString("yyyy"))} year{(int.Parse(date.End.Value.ToString("yyyy")) == 1 ? "" : "s")}"));
            if (int.Parse(date.End.Value.ToString("yyyy")) - int.Parse(DateTime.Now.ToString("yyyy")) > 0) {
                message = $"{int.Parse(date.End.Value.ToString("yyyy")) - int.Parse(DateTime.Now.ToString("yyyy"))} year{(int.Parse(date.End.Value.ToString("yyyy")) == 1 ? "" : "s")} {message}";
            }

            await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Countdown", $"{message}"));
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Countdown",
                Color = Color.Orange,
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Time left",
                        Value = message
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
        }
    }
}