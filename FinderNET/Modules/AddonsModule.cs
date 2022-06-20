using Discord;
using Discord.Interactions;
using FinderNET.Database.Repositories;
using FinderNET.Resources;

namespace FinderNET.Modules {
    
    [Group("addons", "Command For Managing Addons")]
    //todo: make this work
    public class AddonsModule : InteractionModuleBase<SocketInteractionContext> {
        private readonly AddonsRepository addonsRepository;
        public AddonsModule(AddonsRepository _addonsRepository) {
            addonsRepository = _addonsRepository;
        }
        public List<string> SupportedAddons = new List<string>() {
            "TicTacToe", "Economy", "Leveling", "Ticket"
        };

        [SlashCommand("list", "Lists the installed addons")]
        public async Task GetAddons() {
            var value = await addonsRepository.GetAddonsAsync(Context.Guild.Id);
            var embed = new EmbedBuilder() {
                Title = AddonsLocale.AddonsEmbedList_title,
                Color = Color.Orange,
                Footer = new EmbedFooterBuilder() {
                    Text = Main.EmbedFooter
                }
            };
            if (value.addons.Any()) {
                foreach (var i in SupportedAddons) {
                    embed.AddField(name: i, value: value.addons.Contains(i.ToLower()) ? AddonsLocale.AddonsInstalled : AddonsLocale.AddonsNotInstalled, inline: false);
                }
            } else {
                foreach (var i in SupportedAddons) {
                    embed.AddField(name: i, value: AddonsLocale.AddonsNotInstalled, inline: false);
                }
            }
            await RespondAsync("", embed: embed.Build());
        }
    }
}
