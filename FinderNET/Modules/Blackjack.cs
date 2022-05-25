using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Rest;
using System.Linq;

namespace FinderNET {
    public class BlackjackModule : InteractionModuleBase<SocketInteractionContext> {
        [SlashCommand("blackjack", "Play Blackjack")]
        public async Task Blackjack() {
            SocketInteractionContext ctx = new SocketInteractionContext(Context.Client, Context.Interaction);
            await RespondAsync($"{ctx.User.Mention} has started a game of Blackjack! React :white_check_mark: to join!");
        }

        public static async Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction reaction) {
            string content = arg2.Value.GetMessageAsync(arg1.Id).Result.Content;
            // TODO: fix this
            if (reaction.Emote.Name == "✅" && content.Contains("Blackjack")) {;
                await arg2.Value.SendMessageAsync($"<@{reaction.UserId}> has joined the game!");
            }

        }
    }
}