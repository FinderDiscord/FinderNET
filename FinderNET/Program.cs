using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FinderNET.Modules;
using FinderNET.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using FinderNET.Database;
using System.Diagnostics;
using Newtonsoft.Json;

namespace FinderNET {
    class Program {
        static void Main(string[] args) => RunAsync().GetAwaiter().GetResult();
        static async Task RunAsync() {
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = "service postgresql start", }; 
            Process proc = new Process() { StartInfo = startInfo, };
            proc.Start();
            proc.WaitForExit();
            if (!File.Exists(Directory.GetCurrentDirectory() + "appsettings.json")) {
                List<Appsettings> appsettings = new List<Appsettings>();
                appsettings.Add(new Appsettings() {
                    testGuild = 0,
                    ConnectionStrings = new List<string>() {
                        "Server=localhost;Database=finder;Username=postgres;Password=enter database password;"
                    }
                });
                using (StreamWriter file = File.CreateText(Directory.GetCurrentDirectory() + "appsettings.json")) {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, appsettings.ToArray());
                }
            }
            using ServiceProvider services = ConfigureServices();
            DiscordSocketClient client = services.GetRequiredService<DiscordSocketClient>();
            InteractionService commands = services.GetRequiredService<InteractionService>();
            IConfiguration config = services.GetRequiredService<IConfiguration>();
            CommandHandler handler = services.GetRequiredService<CommandHandler>();
            DataAccessLayer dal = services.GetRequiredService<DataAccessLayer>();
            await handler.Initialize();
            client.Log += LoggingService.LogAsync;
            commands.Log += LoggingService.LogAsync;
            CountdownTimer.StartTimer(client, dal);
            client.ReactionAdded += TicTacToeModule.OnReactionAddedEvent;
            client.ReactionAdded += new ModerationModule(services.GetRequiredService<DataAccessLayer>()).OnReactionAddedEvent;
            client.ButtonExecuted += new PollModule(services.GetRequiredService<DataAccessLayer>()).OnButtonExecutedEvent;
            client.ButtonExecuted += new TicketingModule.Tickets(services.GetRequiredService<DataAccessLayer>()).OnButtonExecutedEvent;
            await client.LoginAsync(TokenType.Bot, config["token"]);
            await client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        static ServiceProvider ConfigureServices() {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true).Build();
            return new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<InteractionService>()
            .AddSingleton<CommandHandler>()
            .AddDbContextFactory<FinderDatabaseContext>(options => options.UseNpgsql(configuration.GetConnectionString("Default")))
            .AddSingleton<DataAccessLayer>()
            .BuildServiceProvider();
        }
    }
}
