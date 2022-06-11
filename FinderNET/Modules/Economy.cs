using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FinderNET.Database;

namespace FinderNET.Modules {
    public class EconomyModule : ModuleBase {
        public EconomyModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
    }
}
