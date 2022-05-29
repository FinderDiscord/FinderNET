using Discord.Interactions;
using FinderNET.Database;

namespace FinderNET.Modules
{
    [Group("addons", "does addons things")]
    public class Addons : ModuleBase
    {
        public Addons(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }

        [SlashCommand("installed", "lists the installed addons")]
        public async Task GetAddons()
        {
            var value = dataAccessLayer.GetAddons(long.Parse(Context.Guild.Id.ToString()));
            string str = "";
            foreach (var item in value)
            {
                str += item.ToString() + ", ";
            }
            str = str.Substring(0, str.Length - 2);
            await ReplyAsync($"the installed addons are {str}");
        }


    }
}
