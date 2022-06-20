using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FinderNET.Handlers {
    public class CommandHandler {
        private readonly InteractionService commands;
        private readonly DiscordSocketClient client;
        private readonly IConfiguration config;
        private readonly IServiceProvider services;
        public CommandHandler(InteractionService _commands, DiscordSocketClient _client, IConfiguration _config, IServiceProvider _services) {
            commands = _commands;
            client = _client;
            config = _config;
            services = _services;
        }

        public async Task Initialize() {
            await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);
            client.InteractionCreated += InteractionCreated;
            client.ButtonExecuted += ButtonExecuted;
            client.Ready += Ready;
            commands.SlashCommandExecuted += commandsSlashCommandExecuted;
            commands.AutocompleteHandlerExecuted += commandsAutocompleteHandlerExecuted;
        }

        private Task commandsAutocompleteHandlerExecuted(IAutocompleteHandler arg1, Discord.IInteractionContext arg2, IResult arg3) {
            return Task.CompletedTask;
        }

        private Task commandsSlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3) {
            return Task.CompletedTask;
        }

        private async Task ButtonExecuted(SocketMessageComponent arg) {
            var ctx = new SocketInteractionContext<SocketMessageComponent>(client, arg);
            await commands.ExecuteCommandAsync(ctx, services);
        }

        private async Task Ready() {
            await RegisterCommands();
            client.Ready -= Ready;
        }

        private async Task InteractionCreated(SocketInteraction arg) {
            SocketInteractionContext ctx = new SocketInteractionContext(client, arg);
            IResult result = await commands.ExecuteCommandAsync(ctx, services);
        }

        private async Task RegisterCommands() {
            #if DEBUG
                await commands.RegisterCommandsToGuildAsync(ulong.Parse(config["testGuild"]), true);
            #else
                await commands.RegisterCommandsGloballyAsync(true);
            #endif
        }
    }
}