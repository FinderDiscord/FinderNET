using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Rest;
using System.Linq;

namespace FinderNET {
    public class TicTacToeModule : InteractionModuleBase<SocketInteractionContext> {
        static bool won = false;
        static bool p1go = true;
        static string? p1XorO;
        static string? p2XorO;
        static SocketUser? p1;
        static SocketUser? p2;
        static RestTextChannel? channel;
        static RestUserMessage? message;
        static string[] board = new string[9] {"1️⃣", "2️⃣", "3️⃣", "4️⃣", "5️⃣", "6️⃣", "7️⃣", "8️⃣", "9️⃣"};
        public static string GenerateGrid(string[] board) {
            string grid = "\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\n\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\n";
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) grid += $"\u2B1B\u2B1B\u2B1B{board[i * 3 + j]}";
                grid += "\u2B1B\u2B1B\u2B1B\n\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\n\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\u2B1B\n";
            }
            return grid;
        }
        public static IUser? CheckWin(string[] board) {
            if (p1 == null || p2 == null) return null;
            if (board[1] == board[2] && board[2] == board[3] && board[3] == p1XorO || board[4] == board[5] && board[5] == board[6] && board[6] == p1XorO || board[7] == board[8] && board[8] == board[9] && board[9] == p1XorO || board[1] == board[4] && board[4] == board[7] && board[7] == p1XorO || board[2] == board[5] && board[5] == board[8] && board[8] == p1XorO || board[3] == board[6] && board[6] == board[9] && board[9] == p1XorO || board[1] == board[5] && board[5] == board[9] && board[9] == p1XorO || board[3] == board[5] && board[5] == board[7] && board[7] == p1XorO) {
                return p1;
            } else if (board[1] == board[2] && board[2] == board[3] && board[3] == p2XorO || board[4] == board[5] && board[5] == board[6] && board[6] == p2XorO || board[7] == board[8] && board[8] == board[9] && board[9] == p2XorO || board[1] == board[4] && board[4] == board[7] && board[7] == p2XorO || board[2] == board[5] && board[5] == board[8] && board[8] == p2XorO || board[3] == board[6] && board[6] == board[9] && board[9] == p2XorO || board[1] == board[5] && board[5] == board[9] && board[9] == p2XorO || board[3] == board[5] && board[5] == board[7] && board[7] == p2XorO) {
                return p2;
            }
            return null;
        }
        [SlashCommand("tictactoe", "Play TicTacToe")]
        public async Task TicTacToe(SocketGuildUser user) {
            if (user.IsBot) {
                await ReplyAsync("The user is a bot.");
                return;
            }
            if (user.Status == UserStatus.Offline) {
                await ReplyAsync("The user is offline.");
            }
            SocketInteractionContext ctx = new SocketInteractionContext(Context.Client, Context.Interaction);
            await RespondAsync($"{user.Mention} has been invited to play TicTacToe by {ctx.User.Mention}!");
            channel = await ctx.Guild.CreateTextChannelAsync("tictactoe", (x) => {
                x.Topic = "TicTacToe";
                x.PermissionOverwrites = new List<Overwrite> {
                    new Overwrite(ctx.Guild.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny)),
                    new Overwrite(ctx.User.Id, PermissionTarget.User, new OverwritePermissions(viewChannel: PermValue.Allow)),
                    new Overwrite(user.Id, PermissionTarget.User, new OverwritePermissions(viewChannel: PermValue.Allow))
                };
            });
            p1 = new Random().Next(0, 2) == 0 ? ctx.User : user;
            p2 = ctx.User == p1 ? user : ctx.User;
            p1XorO = new Random().Next(0, 2) == 0 ? "❌" : "⭕";
            p2XorO = p1XorO == "❌" ? "⭕" : "❌";
            message = await channel.SendMessageAsync("", false, new EmbedBuilder() {
                Title = "Tic Tac Toe",
                Color = Color.Blue,
                Fields = new List<EmbedFieldBuilder> {new EmbedFieldBuilder {Name = "The Playing Board", Value = "Please Wait..."}},
                Footer = new EmbedFooterBuilder {Text = $"FinderBot"}
            }.Build());
            List<Emoji> emojis = new List<Emoji>();
            foreach (var item in board) {
                emojis.Add(new Emoji(item));
            };
            await message.AddReactionsAsync(emojis);
            await message.ModifyAsync((x) => {
                x.Embed = new EmbedBuilder() {
                    Title = "Tic Tac Toe",
                    Color = Color.Orange,
                    Fields = new List<EmbedFieldBuilder> {new EmbedFieldBuilder {Name = "The Playing Board", Value = GenerateGrid(board)}},
                    Footer = new EmbedFooterBuilder {Text = $"FinderBot"}
                }.Build();
                x.Content = $"{p1.Mention} has been assigned the {p1XorO} symbol.\n{p2.Mention} has been assigned the {p2XorO} symbol.\n\n{(p1go ? p1.Mention : p2.Mention)}'s Turn!";
            });
        }

        public static async Task OnReactionAddedEvent(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction reaction) {
            if (!won) {
                if (message == null) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Critical, "TicTacToe", $"Message is null"));
                    return;
                }
                if (channel == null) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Critical, "TicTacToe", $"Channel is null"));
                    return;
                }
                if (reaction.User.Value.IsBot) return;
                if (p1 == null || p2 == null) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Error, "TicTacToe", $"{reaction.User.Value.Username} reacted but TicTacToe has not started yet!"));
                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return;
                }
                if (p1XorO == null || p2XorO == null) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Error, "TicTacToe", $"{reaction.User.Value.Username} reacted but TicTacToe has not started yet!"));
                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return;
                }
                if (reaction.UserId != p1.Id && reaction.UserId != p2.Id) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Error, "TicTacToe", $"{reaction.User.Value.Username} are not in a TicTacToe game!"));
                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return;
                }
                if (reaction.Channel.Id != channel.Id) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Error, "TicTacToe", $"{reaction.User.Value.Username} reacted in the wrong channel!"));
                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return;
                }
                if (reaction.MessageId != message.Id) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Error, "TicTacToe", $"{reaction.User.Value.Username} reacted in the wrong message!"));
                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return;
                }
                bool isValid = false;
                for (int i = 0; i < board.Length; i++) if (board[i] == reaction.Emote.Name) isValid = true;
                if (!isValid) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Error, "TicTacToe", $"{reaction.User.Value.Username} reacted with an invalid emoji!"));
                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return;
                }
                if (!p1go && reaction.UserId == p1.Id) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Error, "TicTacToe", $"It is not {reaction.User.Value.Username}'s go!"));
                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return;
                }
                if (p1go && reaction.UserId == p2.Id) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Error, "TicTacToe", $"It is not {reaction.User.Value.Username}'s go!"));
                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                    return;
                }
                for (int i = 0; i < board.Length; i++) {
                    if (board[i] == reaction.Emote.Name) {
                        board[i] = p1go ? p1XorO : p2XorO;
                        p1go = !p1go;
                    }
                }
                await message.ModifyAsync((x) => {
                    x.Embed = new EmbedBuilder() {
                        Title = "Tic Tac Toe",
                        Color = Color.Orange,
                        Fields = new List<EmbedFieldBuilder> {new EmbedFieldBuilder {Name = "The Playing Board", Value = GenerateGrid(board)}},
                        Footer = new EmbedFooterBuilder {Text = $"FinderBot"}
                    }.Build();
                    x.Content = $"{p1.Mention} has been assigned the {p1XorO} symbol.\n{p2.Mention} has been assigned the {p2XorO} symbol.\n\n{(p1go ? p1.Mention : p2.Mention)}'s Turn!";
                });
                await message.RemoveAllReactionsForEmoteAsync(reaction.Emote);
                IUser? winner = CheckWin(board);
                if (winner == p1) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "TicTacToe", $"{p1.Username} has won the game!"));
                    await message.ModifyAsync((x) => {
                        x.Embed = new EmbedBuilder() {
                            Title = "Tic Tac Toe",
                            Color = Color.Green,
                            Fields = new List<EmbedFieldBuilder> {new EmbedFieldBuilder {Name = "The Playing Board", Value = GenerateGrid(board)}},
                            Footer = new EmbedFooterBuilder {Text = $"FinderBot"}
                        }.Build();
                    });
                    await message.ModifyAsync((x) => {
                        channel.SendMessageAsync($"{p1.Mention} has won the game!");
                    });
                    won = true;
                } else if (winner == p2) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "TicTacToe", $"{p2.Username} has won the game!"));
                    await message.ModifyAsync((x) => {
                        x.Embed = new EmbedBuilder() {
                            Title = "Tic Tac Toe",
                            Color = Color.Green,
                            Fields = new List<EmbedFieldBuilder> {new EmbedFieldBuilder {Name = "The Playing Board", Value = GenerateGrid(board)}},
                            Footer = new EmbedFooterBuilder {Text = $"FinderBot"}
                        }.Build();
                    });
                    await message.ModifyAsync((x) => {
                        channel.SendMessageAsync($"{p2.Mention} has won the game!");
                    });
                    won = true;
                } else if (board.All(x => x == "⭕" || x == "❌")) {
                    await LoggingService.LogAsync(new LogMessage(LogSeverity.Info, "TicTacToe", $"The game has ended in a draw!"));
                    await message.ModifyAsync((x) => {
                        channel.SendMessageAsync("The game has ended in a draw!");
                    });
                    won = true;
                }
                
            }
        }
    }
}