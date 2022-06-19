using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FinderNET.Database.Repositories;

namespace FinderNET.Modules {
    [Group("leveling", "Command For Managing Leveling")]
    public class LevelingModule : InteractionModuleBase<SocketInteractionContext> {
        private readonly LevelingRepository context;
        public LevelingModule(LevelingRepository _context) {
            context = _context;
        }

        [SlashCommand("level", "Get your current level")]
        public async Task LevelCommand() {
            var user = Context.User as SocketGuildUser;
            var levels = await context.GetLevelingAsync(user.Guild.Id, user.Id);
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Level",
                Color = Color.Orange,
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Level",
                        Value = levels.level.ToString()
                    },
                    new EmbedFieldBuilder() {
                        Name = "Exp",
                        Value = levels.exp.ToString()
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
        }


        public async Task OnMessageReceivedEvent(SocketMessage message) {
            if (message.Author.IsBot) return;
            const int base_xp = 50;
            const double factor = 1.5;
            var guild = ((SocketGuildChannel)message.Channel).Guild;
            var levels = await context.GetLevelingAsync(guild.Id, message.Author.Id);
            var levelToGet = levels.level + 1;
            var expToGet = base_xp * (int)Math.Pow(factor, levelToGet);
            if (++levels.exp > expToGet) {
                await context.AddLevelingAsync(guild.Id, message.Author.Id, levels.level, 0);
                await message.Channel.SendMessageAsync("", embed: new EmbedBuilder() {
                    Title = $"Level Up {message.Author.Username}",
                    Color = Color.Orange,
                    Fields = new List<EmbedFieldBuilder> {
                        new EmbedFieldBuilder() {
                            Name = "You have leveled up to level",
                            Value = levelToGet
                        }
                    },
                    Footer = new EmbedFooterBuilder() {
                        Text = "FinderBot"
                    }
                }.Build());
            } else {
                await context.AddLevelingAsync(guild.Id, message.Author.Id, levels.level, levels.exp);
            }

        }
    }
}