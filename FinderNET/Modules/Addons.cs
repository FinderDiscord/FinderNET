using Discord;
using Discord.Interactions;
using FinderNET.Database;

namespace FinderNET.Modules {
    
    [Group("addons", "Command For Managing Addons")]
    public class Addons : ModuleBase {
        public List<string> SupportedAddons = new List<string>() {
            "TicTacToe", "Blackjack"
        };
        public Addons(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }

        [SlashCommand("list", "Lists the installed addons")]
        public async Task GetAddons() {
            var value = dataAccessLayer.GetAddons(long.Parse(Context.Guild.Id.ToString()));
            var embed = new EmbedBuilder() {
                Title = "Addon list",
                Color = Color.Orange,
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            };
            if (value == null || value.Count == 0) {
                foreach (var i in SupportedAddons) {
                    embed.AddField(name: i, value: "Not installed", inline: false);
                }
            } else {
                foreach (var i in SupportedAddons) {
                    if (value.Contains(i.ToLower())) {
                        embed.AddField(name: i, value: "Installed", inline: false);
                    } else {
                        embed.AddField(name: i, value: "Not installed", inline: false);
                    }
                }
            }
            await ReplyAsync("", false, embed.Build());
        }

        
        
    }
}
