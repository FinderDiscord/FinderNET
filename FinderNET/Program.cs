using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinderNET.Modules;
using FinderNET.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using FinderNET.Database;

namespace FinderNET
{
    class Program
    {
        static void Main(string[] args) => RunAsync().GetAwaiter().GetResult();
        static async Task RunAsync()
        {
            using ServiceProvider services = ConfigureServices();
            DiscordSocketClient client = services.GetRequiredService<DiscordSocketClient>();
            InteractionService commands = services.GetRequiredService<InteractionService>();
            IConfiguration config = services.GetRequiredService<IConfiguration>();
            CommandHandler handler = services.GetRequiredService<CommandHandler>();
            await handler.Initialize();
            client.Log += LoggingService.LogAsync;
            commands.Log += LoggingService.LogAsync;
            client.ReactionAdded += TicTacToeModule.OnReactionAddedEvent;
            await client.LoginAsync(TokenType.Bot, config["token"]);
            await client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true).Build())
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandler>()
            .AddDbContextFactory<FinderDatabaseContext>(options =>
            options.UseNpgsql("Server=localhost;Database=finder;Username=postgres;Password=password;"))
            .AddSingleton<DataAccessLayer>()
            .BuildServiceProvider();
        }
    }
}