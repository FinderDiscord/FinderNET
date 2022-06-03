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
            await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Countdown", $"{Context.User.Username} requested a countdown to {date.ToString()}"));
            if (date == null || date.Start == null || date.End == null) {
                await ReplyAsync("Invalid date or time");
                return;
            }
            if (date.Start.Value < DateTime.Now) {
                await ReplyAsync("The date or time is in the past");
                return;
            }
            var message = await ReplyAsync("", false, new EmbedBuilder() {
                Title = "Countdown",
                Color = Color.Orange,
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Time left",
                        Value = $"{date.Start.Value.Subtract(DateTime.Now).ToString("dd\\.hh\\:mm\\:ss")}",
                        IsInline = true
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
            await dataAccessLayer.SetDateTime((Int64)message.Id, date.End.Value);
            
        }
    }
}