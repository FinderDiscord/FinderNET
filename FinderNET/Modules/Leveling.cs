using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FinderNET.Database;

namespace FinderNET.Modules {
    [Group("leveling", "Command For Managing Leveling")]
    public class LevelingModule : ModuleBase {
        public LevelingModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
        public async Task OnMessageReceivedEvent(SocketMessage message) {
        }
    }
}