using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FinderNET {
    class Program {
        static void Main (string[] args) {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("config.json").Build();
            RunAsync(config).GetAwaiter().GetResult();
        }
        static async Task RunAsync (IConfiguration config) {
            using ServiceProvider services = ConfigureServices(config);
            DiscordSocketClient client = services.GetRequiredService<DiscordSocketClient>();
            InteractionService commands = services.GetRequiredService<InteractionService>();
            client.Log += LoggingService.LogAsync;
            commands.Log += LoggingService.LogAsync;
            client.Ready += async () => {
                if (IsDebug()) {
                    await commands.RegisterCommandsToGuildAsync(ulong.Parse(config["testGuild"]), true);
                } else {
                    await commands.RegisterCommandsGloballyAsync(true);
                }
            };
            await services.GetRequiredService<SlashCommandHandler>().InitializeAsync();
            await client.LoginAsync(TokenType.Bot, config["token"]);
            await client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        static ServiceProvider ConfigureServices (IConfiguration config) {
            return new ServiceCollection().AddSingleton(config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<SlashCommandHandler>()
                .BuildServiceProvider();
        }
        static bool IsDebug() {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }
    }
}