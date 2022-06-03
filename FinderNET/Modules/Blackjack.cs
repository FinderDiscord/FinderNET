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
                if (game.lobby && game.joinChannel.Id == reaction.Channel.Id && reaction.MessageId == game.joinMessage.Id) {
                    if (reaction.Emote.Name == "✅" && reaction.UserId != game.creator.Id && !game.players.Contains(reaction.User.Value)) {
                        await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Blackjack", $"{reaction.User.Value.Username} has joined the game!"));
                        await game.playChannel.SendMessageAsync($"{reaction.User.Value.Mention} has joined the game!");
                        game.AddPlayer(reaction.User.Value);
                        return;
                    }
                } else if (game.lobby && game.playChannel.Id == reaction.Channel.Id && reaction.MessageId == game.lobbyMessage.Id && reaction.User.Value.Id == game.creator.Id) {
                    if (reaction.Emote.Name == "✅") {
                        game.StartGame();
                        return;
                    } else if (reaction.Emote.Name == "❌") {
                        await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Blackjack", $"{reaction.User.Value.Username} has cancelled the game!"));
                        await game.playChannel.SendMessageAsync($"{reaction.User.Value.Mention} has cancelled the game!");
                        await game.playChannel.DeleteAsync();
                        await game.joinMessage.DeleteAsync();
                        games.Remove(game);
                        return;
                    }
                }
            }
        }

        public static async Task ButtonHandler(SocketMessageComponent component){
            await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Blackjack", $"{component.User.Username} has pressed {component.Data.CustomId}!"));
            Blackjack? game = null;
            foreach (Blackjack _game in games){
                if (_game.playChannel == null || _game.lobbyMessage == null) continue;
                foreach (IUser user in _game.players){
                    if (user.Id == component.User.Id && (component.Data.CustomId == $"{_game.gameId}-H" || component.Data.CustomId == $"{_game.gameId}-S")) {
                        game = _game;
                        break;
                    }
                }
            }
            if (game == null) return;
            await component.RespondAsync($"{component.User.Mention} has pressed {component.Data.CustomId}!");
        }
    }

    public class Blackjack {
        private SocketInteractionContext ctx;
        public IUserMessage? lobbyMessage;
        public IUserMessage? playMessage;
        public IUserMessage joinMessage;
        public RestTextChannel? playChannel;
        public ISocketMessageChannel joinChannel;
        public IUser creator;
        public List<IUser> players = new List<IUser>();
        public bool lobby = true;
        public int gameId;
        public Blackjack(SocketInteractionContext _ctx, IUser _creator, ISocketMessageChannel _joinChannel, IUserMessage _joinMessage) {
            ctx = _ctx;
            players.Add(_creator);
            creator = _creator;
            joinChannel = _joinChannel;
            joinMessage = _joinMessage;
            newChannel(creator);
            // generate random game id
            gameId = new Random().Next(100000000, 999999999);
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
            lobbyMessage = await playChannel.SendMessageAsync($"{creator.Mention} react with ✅ to start the game or ❌ to cancel.");
            await lobbyMessage.AddReactionsAsync(new Emoji[] {new Emoji("✅"), new Emoji("❌")});
        }

        public async void AddPlayer(IUser user) {
            if (playChannel == null) return;
            foreach (IUser userIn in players) {
                if (userIn.Id == user.Id) {
                    return;
                }
            }
            players.Add(user);
            await playChannel.AddPermissionOverwriteAsync(user, new OverwritePermissions(viewChannel: PermValue.Allow));
        }

        public async void StartGame() {
            //TODO: Make this not one line
            if (!lobby || playChannel == null) { await LoggingService.LogAsync(new LogMessage(LogSeverity.Critical, "Blackjack", "Tried to start a game!")); return;}
            lobby = false;
            await lobbyMessage.DeleteAsync();
            await playChannel.GetMessagesAsync().Flatten().ForEachAsync(async (x) => {
                await x.DeleteAsync();
            });
            await playChannel.SendMessageAsync($"{creator.Mention} has started the game!");
            playMessage = await playChannel.SendMessageAsync($"test");
            foreach (IUser user in players) {
                await user.SendMessageAsync("test", components: new ComponentBuilder().WithButton("hit", $"{gameId}-H").WithButton("stick", $"{gameId}-S").Build());
            }
        }
    }
}