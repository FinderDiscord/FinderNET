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
            if (date == null || date.Start == null) {
                await ReplyAsync("Invalid date or time");
                return;
            }
            if (date.Start.Value < DateTime.Now) {
                await ReplyAsync("The date or time is in the past");
                return;
            }
            await ReplyAsync("", false, new EmbedBuilder() {
                Title = "Countdown",
                Color = Color.Orange,
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Date",
                        Value = date.Start.Value.ToString("dddd, MMMM d, yyyy"),
                        IsInline = true
                    },
                    new EmbedFieldBuilder() {
                        Name = "Time",
                        Value = date.Start.Value.ToString("h:mm tt"),
                        IsInline = true
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
        }
    }
}