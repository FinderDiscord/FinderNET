using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB;

namespace FinderNET {
    class Bot {
        private readonly DiscordSocketClient client;
        public IConfigurationRoot config { get; }
        public static void Main(string[] args) => new Bot().MainAsync().GetAwaiter().GetResult();
        public Bot() {
            // Maybe change this location
            config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("config.json").Build();
            client = new DiscordSocketClient();
            client.Log += FinderNET.LoggingService.LogAsync;
            client.Ready += ReadyAsync;
        }
        public async Task MainAsync() {
            CommandService commands = new CommandService();
            CommandHandler commandHandler = new CommandHandler(client, commands);
            await commandHandler.InstallCommandsAsync();
            await client.LoginAsync(TokenType.Bot, config.GetSection("token").Value);
            MongoClient mangoClient = new MongoClient(config.GetSection("mongoUri").Value);
            IMongoDatabase database = mangoClient.GetDatabase("FinderNET");
            await client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }
        public async Task ReadyAsync() {
            await client.SetActivityAsync(new Game("the screams of humans", ActivityType.Listening));
        }
    }
}