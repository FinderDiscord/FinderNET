using Discord;
using Discord.Interactions;
using FinderNET.Database;
using FinderNET.Modules;

namespace FinderNET {
    public class HelpModule : ModuleBase {
        private readonly InteractionService interactionService;
        public HelpModule(DataAccessLayer dataAccessLayer, InteractionService _interaction) : base(dataAccessLayer) { 
            interactionService = _interaction;
        }

        [SlashCommand("help", "Shows the help message.")]
        public async Task Help() {
            List<SlashCommandInfo> commands = interactionService.SlashCommands.ToList();
            commands.Sort((a, b) => a.Name.CompareTo(b.Name));
            EmbedBuilder embed = new EmbedBuilder() {
                Title = "FinderNET Help",
                Description = "Here are all the commands you can use with FinderNET.",
                Color = Color.Blue,
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderNET"
                }
            };
            foreach (SlashCommandInfo command in commands) {
                embed.AddField(command.Name, command.Description);
            }
            await RespondAsync(embed: embed.Build());
            // Todo: add pagination
        }
    }
}