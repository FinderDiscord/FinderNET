using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FinderNET.Database;

namespace FinderNET.Modules {
    [Group("leveling", "Command For Managing Leveling")]
    public class LevelingModule : ModuleBase {
        public LevelingModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }

        [SlashCommand("level", "Get your current level")]
        public async Task LevelCommand() {
            var user = Context.User as SocketGuildUser;
            var level = await dataAccessLayer.GetLevel((long)user.Guild.Id, (long)user.Id);
            var exp = await dataAccessLayer.GetExp((long)user.Guild.Id, (long)user.Id);
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Level",
                Color = Color.Orange,
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Level",
                        Value = level.ToString()
                    },
                    new EmbedFieldBuilder() {
                        Name = "Exp",
                        Value = exp.ToString()
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
        }


        public async Task OnMessageReceivedEvent(SocketMessage message) {
            const int base_xp = 200;
            const int factor = 2;
            var guild = ((SocketGuildChannel)message.Channel).Guild;
            var exp = await dataAccessLayer.GetExp((long)guild.Id, (long)message.Author.Id);
            var levelToGet = await dataAccessLayer.GetLevel((long)guild.Id, (long)message.Author.Id) + 1;
            var expToGet = base_xp * (int)Math.Pow(factor, levelToGet);
            if (++exp > expToGet) {
                await dataAccessLayer.SetLevel((long)guild.Id, (long)message.Author.Id, levelToGet);
                await dataAccessLayer.SetExp((long)guild.Id, (long)message.Author.Id, 0);
                await RespondAsync("", embed: new EmbedBuilder() {
                    Title = "Level Up",
                    Color = Color.Orange,
                    Fields = new List<EmbedFieldBuilder> {
                        new EmbedFieldBuilder() {
                            Name = "You have leveled up to level ",
                            Value = levelToGet.ToString()
                        }
                    },
                    Footer = new EmbedFooterBuilder() {
                        Text = "FinderBot"
                    }
                }.Build());
            } else {
                await dataAccessLayer.SetExp((long)guild.Id, (long)message.Author.Id, exp);
            }

        }
    }
}