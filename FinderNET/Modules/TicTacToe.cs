using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Rest;
using FinderNET.Database;

namespace FinderNET.Modules
{
    // TODO:
    // delete channel after
    // check win conditions
    // check delays on line 81
    public class TicTacToeModule : ModuleBase
    {
        public TicTacToeModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
        private static List<string> validEmotes = new List<string>() { "1️⃣", "2️⃣", "3️⃣", "4️⃣", "5️⃣", "6️⃣", "7️⃣", "8️⃣", "9️⃣", "✅", "❌" };
        private static List<TicTacToe> games = new List<TicTacToe>();

        [SlashCommand("tictactoe", "Play TicTacToe")]
        public async Task TicTacToeCommand(SocketGuildUser user)
        {
            SocketInteractionContext ctx = new SocketInteractionContext(Context.Client, Context.Interaction);
            if (user.IsBot)
            {
                await RespondAsync("The user is a bot.");
                return;
            }
            if (user.Id == ctx.User.Id)
            {
                await RespondAsync("You can't play TicTacToe with yourself.");
                return;
            }
            await RespondAsync($"{user.Mention} has been invited to play TicTacToe by {ctx.User.Mention}!");
            IUser p1 = new Random().Next(0, 2) == 0 ? ctx.User : user;
            IUser p2 = ctx.User == p1 ? user : ctx.User;
            string p1Symbol = new Random().Next(0, 2) == 0 ? "❌" : "⭕";
            string p2Symbol = p1Symbol == "❌" ? "⭕" : "❌";
            games.Add(new TicTacToe(ctx, p1, p2, p1Symbol, p2Symbol));
        }

        public static async Task OnReactionAddedEvent(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction reaction)
        {
            foreach (TicTacToe game in games)
            {
                if (game.playChannel == null || game.lobbyMessage == null) return;
                if (game.playChannel.Id != reaction.Channel.Id) return;
                if (reaction.User.Value.IsBot) return;
                bool valid = false;
                foreach (string symbol in validEmotes)
                {
                    if (reaction.Emote.Name == symbol)
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid) return;
                if (
                    game.lobby
                    && reaction.MessageId == game.lobbyMessage.Id
                )
                {
                    if (reaction.Emote.Name == "✅")
                    {
                        game.p1Ready = game.player1.Id == reaction.UserId ? true : game.p1Ready;
                        game.p2Ready = game.player2.Id == reaction.UserId ? true : game.p2Ready;
                        await game.playChannel.SendMessageAsync($"{reaction.User.Value.Mention} is ready!");
                        if (game.p1Ready && game.p2Ready)
                        {
                            game.lobby = false;
                            await game.playChannel.SendMessageAsync($"{game.player1.Mention} and {game.player2.Mention} are ready!\nStarting game...");
                            game.playMessage = await game.playChannel.SendMessageAsync("", false, new EmbedBuilder()
                            {
                                Title = "Tic Tac Toe",
                                Color = Color.Blue,
                                Fields = new List<EmbedFieldBuilder> { new EmbedFieldBuilder { Name = "The Playing Board", Value = "Please Wait..." } },
                                Footer = new EmbedFooterBuilder { Text = $"FinderBot" }
                            }.Build());
                            List<Emoji> emojis = new List<Emoji>();
                            foreach (string item in game.board)
                            {
                                emojis.Add(new Emoji(item));
                            };
                            await game.playMessage.AddReactionsAsync(emojis);
                            await game.playMessage.ModifyAsync((x) =>
                            {
                                x.Embed = new EmbedBuilder()
                                {
                                    Title = "Tic Tac Toe",
                                    Color = Color.Orange,
                                    Fields = new List<EmbedFieldBuilder> { new EmbedFieldBuilder { Name = "The Playing Board", Value = game.GenerateGrid() } },
                                    Footer = new EmbedFooterBuilder { Text = $"FinderBot" }
                                }.Build();
                                x.Content = $"{game.player1.Mention} has been assigned the {game.p1Symbol} symbol.\n{game.player2.Mention} has been assigned the {game.p2Symbol} symbol.\n\n{game.player1.Mention}'s Turn!";
                            });
                        }
                        return;
                    }
                    else if (reaction.Emote.Name == "❌")
                    {
                        await game.playChannel.SendMessageAsync($"{reaction.User.Value.Mention} has cancelled game.");
                        // dunno if this makes the bot unresponsive
                        Thread.Sleep(2000);
                        await game.playChannel.DeleteAsync();
                        games.Remove(game);
                        return;
                    }
                }
                if (game.playMessage == null) return;
                if (
                    !game.win
                    && game.playMessage.Id == reaction.MessageId
                    && (game.p1go && game.player1.Id == reaction.UserId || !game.p1go && game.player2.Id == reaction.UserId)
                )
                {
                    game.p1go = !game.p1go;
                    foreach (string slot in game.board)
                    {
                        if (slot == reaction.Emote.Name)
                        {
                            game.board[game.board.IndexOf(slot)] = game.p1go ? game.p1Symbol : game.p2Symbol;
                            break;
                        }
                    }
                    await game.playMessage.ModifyAsync((x) =>
                    {
                        x.Embed = new EmbedBuilder()
                        {
                            Title = "Tic Tac Toe",
                            Color = Color.Orange,
                            Fields = new List<EmbedFieldBuilder> { new EmbedFieldBuilder { Name = "The Playing Board", Value = game.GenerateGrid() } },
                            Footer = new EmbedFooterBuilder { Text = $"FinderBot" }
                        }.Build();
                        x.Content = $"{game.player1.Mention} has been assigned the {game.p1Symbol} symbol.\n{game.player2.Mention} has been assigned the {game.p2Symbol} symbol.\n\n{(game.p1go ? game.player1.Mention : game.player2.Mention)}'s Turn!";
                    });
                    await game.playMessage.RemoveAllReactionsForEmoteAsync(reaction.Emote);
                    IUser? winner = game.CheckWin();
                    if (winner != null && winner.Id == game.player1.Id)
                    {
                        await game.playMessage.ModifyAsync((x) =>
                        {
                            x.Embed = new EmbedBuilder()
                            {
                                Title = "Tic Tac Toe",
                                Color = Color.Green,
                                Fields = new List<EmbedFieldBuilder> { new EmbedFieldBuilder { Name = "The Playing Board", Value = game.GenerateGrid() } },
                                Footer = new EmbedFooterBuilder { Text = $"FinderBot" }
                            }.Build();
                        });
                        await game.playChannel.SendMessageAsync($"{game.player1.Mention} has won the game!");
                        game.win = true;
                    }
                    else if (winner != null && winner.Id == game.player2.Id)
                    {
                        await game.playMessage.ModifyAsync((x) =>
                        {
                            x.Embed = new EmbedBuilder()
                            {
                                Title = "Tic Tac Toe",
                                Color = Color.Green,
                                Fields = new List<EmbedFieldBuilder> { new EmbedFieldBuilder { Name = "The Playing Board", Value = game.GenerateGrid() } },
                                Footer = new EmbedFooterBuilder { Text = $"FinderBot" }
                            }.Build();
                        });
                        await game.playChannel.SendMessageAsync($"{game.player1.Mention} has won the game!");
                        game.win = true;
                    }
                    else if (game.board.All(x => x == "⭕" || x == "❌"))
                    {
                        await game.playChannel.SendMessageAsync("The game has ended in a draw!");
                        game.win = true;
                    }
                }
                return;
            }
        }

        public class TicTacToe
        {
            private SocketInteractionContext ctx;
            public RestTextChannel? playChannel;
            public RestUserMessage? playMessage;
            public RestUserMessage? lobbyMessage;
            public IUser player1;
            public IUser player2;
            public string p1Symbol;
            public string p2Symbol;
            public bool p1Ready;
            public bool p2Ready;
            public bool win;
            public bool p1go;
            public List<string> board;
            public bool lobby;

            public TicTacToe(SocketInteractionContext _ctx, IUser _player1, IUser _player2, string _p1Symbol, string _p2Symbol)
            {
                ctx = _ctx;
                player1 = _player1;
                player2 = _player2;
                p1Symbol = _p1Symbol;
                p2Symbol = _p2Symbol;
                win = false;
                p1go = true;
                p1Ready = false;
                p2Ready = false;
                board = new List<string>() {
                    "1️⃣", "2️⃣", "3️⃣",
                    "4️⃣", "5️⃣", "6️⃣",
                    "7️⃣", "8️⃣", "9️⃣"
                };
                playMessage = null;
                playChannel = null;
                lobbyMessage = null;
                lobby = true;

                newChannel(player1, player2);
            }

            private async void newChannel(IUser player1, IUser player2)
            {
                playChannel = await ctx.Guild.CreateTextChannelAsync("tictactoe", (x) =>
                {
                    x.Topic = "TicTacToe";
                    x.PermissionOverwrites = new List<Overwrite> {
                        new Overwrite(ctx.Guild.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny)),
                        new Overwrite(player1.Id, PermissionTarget.User, new OverwritePermissions(viewChannel: PermValue.Allow)),
                        new Overwrite(player2.Id, PermissionTarget.User, new OverwritePermissions(viewChannel: PermValue.Allow))
                    };
                });
                lobbyMessage = await playChannel.SendMessageAsync($"Both players need to react with ✅ to start the game or ❌ to cancel.");
                await lobbyMessage.AddReactionsAsync(new Emoji[] { new Emoji("✅"), new Emoji("❌") });
            }
            public string GenerateGrid()
            {
                string grid = "⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛\n⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛\n";
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++) grid += $"⬛⬛⬛{board[i * 3 + j]}";
                    grid += "⬛⬛⬛\n⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛\n⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛⬛\n";
                }
                return grid;
            }
            public IUser? CheckWin()
            {
                if (
                   board[1] == board[2] && board[2] == board[3] && board[3] == p1Symbol // 1, 2, 3
                || board[4] == board[5] && board[5] == board[6] && board[6] == p1Symbol // 4, 5, 6
                || board[7] == board[8] && board[8] == board[9] && board[9] == p1Symbol // 7, 8, 9
                || board[1] == board[4] && board[4] == board[7] && board[7] == p1Symbol // 1, 4, 7
                || board[2] == board[5] && board[5] == board[8] && board[8] == p1Symbol // 2, 5, 8
                || board[3] == board[6] && board[6] == board[9] && board[9] == p1Symbol // 3, 6, 9
                || board[1] == board[5] && board[5] == board[9] && board[9] == p1Symbol // 1, 5, 9
                || board[3] == board[5] && board[5] == board[7] && board[7] == p1Symbol // 3, 5, 7 
                )
                {
                    return player1;
                }
                else if (
                 board[1] == board[2] && board[2] == board[3] && board[3] == p2Symbol // 1, 2, 3
              || board[4] == board[5] && board[5] == board[6] && board[6] == p2Symbol // 4, 5, 6
              || board[7] == board[8] && board[8] == board[9] && board[9] == p2Symbol // 7, 8, 9
              || board[1] == board[4] && board[4] == board[7] && board[7] == p2Symbol // 1, 4, 7
              || board[2] == board[5] && board[5] == board[8] && board[8] == p2Symbol // 2, 5, 8
              || board[3] == board[6] && board[6] == board[9] && board[9] == p2Symbol // 3, 6, 9
              || board[1] == board[5] && board[5] == board[9] && board[9] == p2Symbol // 1, 5, 9
              || board[3] == board[5] && board[5] == board[7] && board[7] == p2Symbol // 3, 5, 7
              )
                {
                    return player2;
                }
                return null;
            }
        }
    }
}