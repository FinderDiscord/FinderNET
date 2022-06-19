using Discord;
using Discord.Interactions;
using FinderNET.Database.Repositories;

namespace FinderNET.Modules {
    
    [Group("addons", "Command For Managing Addons")]
    //todo: make this work
    public class AddonsModule : InteractionModuleBase<SocketInteractionContext> {
        private readonly AddonsRepository addonsRepository;
        public AddonsModule(AddonsRepository _addonsRepository) {
            addonsRepository = _addonsRepository;
        }
        public List<string> SupportedAddons = new List<string>() {
            "TicTacToe", "Blackjack"
        };

        [SlashCommand("list", "Lists the installed addons")]
        public async Task GetAddons() {
            var value = await addonsRepository.GetAddonsAsync(Context.Guild.Id);
            var embed = new EmbedBuilder() {
                Title = "Addon list",
                Color = Color.Orange,
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            };
            if (value == null || value.addons.Count() == 0) {
                foreach (var i in SupportedAddons) {
                    embed.AddField(name: i, value: "Not installed", inline: false);
                }
            } else {
                foreach (var i in SupportedAddons) {
                    if (value.addons.Contains(i.ToLower())) {
                        embed.AddField(name: i, value: "Installed", inline: false);
                    } else {
                        embed.AddField(name: i, value: "Not installed", inline: false);
                    }
                }
            }
            await RespondAsync("", embed: embed.Build());
        }
    }
}
