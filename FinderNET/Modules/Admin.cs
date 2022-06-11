using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using FinderNET.Database;

namespace FinderNET.Modules {
    public class AdminModule : ModuleBase {
        public AdminModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
        
        [SlashCommand("purge", "Purge a number of messages", runMode: RunMode.Async)]
        public async Task PurgeCommand(int count) {
            var messages = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
            try {
                await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(messages);
            } catch (Exception e) {
                await ReplyAsync("An error occurred while purging messages.");
            }
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Purged",
                Color = Color.Orange,
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
        }
    }
}