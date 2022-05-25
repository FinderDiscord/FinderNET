using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Rest;
using System.Linq;

namespace FinderNET {
    public class BlackjackModule : InteractionModuleBase<SocketInteractionContext> {
        private static List<Blackjack> games = new List<Blackjack>();
        [SlashCommand("blackjack", "Play Blackjack")]
        public async Task Blackjack() {
            await RespondAsync("Blackjack is not implemented yet.");
            SocketInteractionContext ctx = new SocketInteractionContext(Context.Client, Context.Interaction);
            RestUserMessage message = await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} has started a game of Blackjack! React :white_check_mark: to join!");
            await message.AddReactionAsync(new Emoji("✅"));
            games.Add(new Blackjack(message.Id, message.Channel.Id, ctx.User));
        }

        public static async Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction reaction) {
            bool ValidMessage = false;
            foreach (Blackjack game in games){
                if (game.JoinChannelID == reaction.Channel.Id && game.JoinMessageID == reaction.MessageId) {
                    ValidMessage = true;
                    break;
                }
            } if(!ValidMessage) {return;}
            if (reaction.Emote.Name == "✅") {
                await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Blackjack", $"{reaction.User.Value.Username} has joined the game!"));
            }

        } 

    }

    public class Blackjack {
        public ulong JoinChannelID {get; set;}
        public ulong JoinMessageID {get; set;}
        ulong PlayChannelID;
        ulong PlayMessageID;
        List<IUser> players;
        public Blackjack(ulong _JoinMessageID, ulong _JoinChannelID, IUser creator){
            this.JoinMessageID = _JoinMessageID;
            this.JoinChannelID = _JoinChannelID;
            this.players = new List<IUser>();
            this.players.Add(creator);
            
        }
    }
}