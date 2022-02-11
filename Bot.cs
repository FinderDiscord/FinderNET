using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace FinderNet {
    class Bot {
        private DiscordSocketClient? client;
        public static Task Main(string[] args) => new Bot().MainAsync();
        public async Task MainAsync() {
            client = new DiscordSocketClient();
            Console.WriteLine("Starting bot...");
            client.Log += Log;
            var token = System.IO.File.ReadAllText("token.txt");
            await client.LoginAsync(TokenType.Bot, token);
            MongoClient mangoClient = new MongoClient("mongodb+srv://bot:eHeNUM6vuSOvSGpT@finder.favdr.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
            IMongoDatabase database = mangoClient.GetDatabase("FinderNET");
            await client.StartAsync();
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}