using Discord;
using Discord.Interactions;

namespace InteractionsDemo {
    public class GuildAdminModule : InteractionModuleBase<SocketInteractionContext> {
        [SlashCommand("ping", "pong")]
        public async Task Ping() {
            await RespondAsync("Pong!");
        }
    }
}