using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace FinderNET {
    public class SlashCommandHandler {
        private readonly DiscordSocketClient client;
        private readonly InteractionService commands;
        private readonly IServiceProvider services;

        public SlashCommandHandler(DiscordSocketClient _client, InteractionService _commands, IServiceProvider _services) {
            client = _client;
            commands = _commands;
            services = _services;
        }

        public async Task InitializeAsync() {
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            client.InteractionCreated += HandleInteraction;
        }

        private async Task HandleInteraction(SocketInteraction arg) {
            try {
                var ctx = new SocketInteractionContext(client, arg);
                await commands.ExecuteCommandAsync(ctx, services);
            } catch (Exception ex) {
                Console.WriteLine(ex);
                if(arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}