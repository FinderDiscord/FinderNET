using Discord.Commands;
using Discord.WebSocket;

public class HelpModule : ModuleBase<SocketCommandContext> {
    [Command("bleep")]
    public async Task BleepAsync() {
        await ReplyAsync("Bleep bloop");
    }
}