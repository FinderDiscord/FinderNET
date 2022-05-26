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
            games.Add(new Blackjack(ctx, message.Id, message.Channel.Id, ctx.User));
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
                foreach (Blackjack game in games) {
                    if (game.JoinChannelID == reaction.Channel.Id && game.JoinMessageID == reaction.MessageId) {
                        game.AddPlayer(reaction.User.Value);
                        break;
                    }
                }
            }

        } 

    }

    public class Blackjack {
        public ulong JoinChannelID {get; set;}
        public ulong JoinMessageID {get; set;}
        ulong PlayChannelID;
        ulong PlayMessageID;
        private SocketInteractionContext ctx;
        List<IUser> players;
        RestTextChannel? channel;
        bool started = false;
        public Blackjack(SocketInteractionContext _ctx, ulong _JoinMessageID, ulong _JoinChannelID, IUser creator){
            this.JoinMessageID = _JoinMessageID;
            this.JoinChannelID = _JoinChannelID;
            this.players = new List<IUser>();
            this.players.Add(creator);
            this.ctx = _ctx;
            newChannel(creator);
        }

        private async void newChannel(IUser creator){
            channel = await ctx.Guild.CreateTextChannelAsync("blackjack", (x) => {
                x.Topic = "Blackjack";
                x.PermissionOverwrites = new List<Overwrite> {
                    new Overwrite(ctx.Guild.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny)),
                    new Overwrite(creator.Id, PermissionTarget.User, new OverwritePermissions(viewChannel: PermValue.Allow))
                };
            });
            await channel.SendMessageAsync($"You are waiting in a lobby for players to join!");
            await channel.SendMessageAsync($"{creator.Mention}, react :white_check_mark: to start!");
        }

        public async Task Join(IUser user) {
            this.players.Add(user);
            await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "Blackjack", $"{user.Username} has joined the game!"));
            await ctx.Channel.SendMessageAsync($"{user.Mention} has joined the game!");
        }

        public async void AddPlayer(IUser user) {
            foreach (IUser userIn in players) {
                if (userIn.Id == user.Id) {
                    return;
                }
            }
            this.players.Add(user);
            await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(viewChannel: PermValue.Allow));
            await channel.SendMessageAsync($"{user.Mention} has joined the game!");
        }
    }
}