using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Rest;

namespace FinderNET {
    public class BlackjackModule : InteractionModuleBase<SocketInteractionContext> {
        private static List<Blackjack> games = new List<Blackjack>();
        private static List<String> validEmotes = new List<string>() { "✅", "❌"};
        [SlashCommand("blackjack", "Play Blackjack")]
        public async Task BlackjackCommand() {
            await RespondAsync("Blackjack is not implemented yet.");
            SocketInteractionContext ctx = new SocketInteractionContext(Context.Client, Context.Interaction);
            RestUserMessage message = await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} has started a game of Blackjack! React ✅ to join!");
            await message.AddReactionAsync(new Emoji("✅"));
            games.Add(new Blackjack(ctx, ctx.User, ctx.Channel, message));
        }

        public static async Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction reaction) {
            foreach (Blackjack game in games){
                if (reaction.User.Value.IsBot) return;
                bool valid = false;
                foreach (string symbol in validEmotes) {
                    if (reaction.Emote.Name == symbol) {
                        valid = true;
                        break;
                    }
                }
                if (!valid) return;
                if (game.playChannel == null || game.lobbyMessage == null) continue;
                if (game.playChannel.Id != reaction.Channel.Id) continue;
                if (game.lobby && reaction.MessageId == game.lobbyMessage.Id) {
                    if (reaction.Emote.Name == "✅") {
                        await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Blackjack", $"{reaction.User.Value.Username} has joined the game!"));
                        game.AddPlayer(reaction.User.Value);
                        return;
                    }
                }
            }
        }
    }

    public class Blackjack {
        private SocketInteractionContext ctx;
        public IUserMessage? lobbyMessage;
        public IUserMessage? playMessage;
        public IUserMessage joinMessage;
        public RestTextChannel? playChannel;
        public ISocketMessageChannel joinChannel;
        
        List<IUser> players;
        public bool lobby = true;
        public Blackjack(SocketInteractionContext _ctx, IUser creator, ISocketMessageChannel _joinChannel, IUserMessage _joinMessage) {
            this.players = new List<IUser>();
            this.players.Add(creator);
            this.ctx = _ctx;
            this.joinChannel = _joinChannel;
            this.joinMessage = _joinMessage;
            newChannel(creator);
        }

        private async void newChannel(IUser creator){
            playChannel = await ctx.Guild.CreateTextChannelAsync("blackjack", (x) => {
                x.Topic = "Blackjack";
                x.PermissionOverwrites = new List<Overwrite> {
                    new Overwrite(ctx.Guild.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny, sendMessages: PermValue.Deny)),
                    new Overwrite(creator.Id, PermissionTarget.User, new OverwritePermissions(viewChannel: PermValue.Allow))
                };
            });
            await playChannel.SendMessageAsync($"You are waiting in a lobby for players to join!");
            lobbyMessage = await playChannel.SendMessageAsync($"Both players need to react with ✅ to start the game or ❌ to cancel.");
            await lobbyMessage.AddReactionsAsync(new Emoji[] {new Emoji("✅"), new Emoji("❌")});
        }

        public async void AddPlayer(IUser user) {
            if (playChannel == null) return;
            foreach (IUser userIn in players) {
                if (userIn.Id == user.Id) {
                    return;
                }
            }
            this.players.Add(user);
            await playChannel.AddPermissionOverwriteAsync(user, new OverwritePermissions(viewChannel: PermValue.Allow));
            await playChannel.SendMessageAsync($"{user.Mention} has joined the game!");
        }
    }
}