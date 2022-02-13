using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace FinderNET {
    class CommandHandler {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;
        private async Task HandleCommandAsync(SocketMessage messageParam) {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            int argPos = 0;
            if (!(message.HasCharPrefix('|', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.Author.IsBot)) return;
            await commands.ExecuteAsync(context: new SocketCommandContext(client, message), argPos: argPos, services: null);
        }
        public CommandHandler(DiscordSocketClient _client, CommandService _commands) {
            client = _client;
            commands = _commands;
        }

        public async Task InstallCommandsAsync() {
            client.MessageReceived += HandleCommandAsync;

            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }
    }
}
