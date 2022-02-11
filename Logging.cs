using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace FinderNET {
    public class LoggingService {
        public LoggingService(DiscordSocketClient client, CommandService command) {
            client.Log += LogAsync;
            command.Log += LogAsync;
        }
        public static Task LogAsync(LogMessage message) {
            if (message.Exception is CommandException cmdException) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{message.Severity}] {cmdException.Command.Aliases.First()} failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
                Console.ResetColor();
            } else {
                if (message.Severity == LogSeverity.Critical) {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.BackgroundColor = ConsoleColor.White;
                } else if (message.Severity == LogSeverity.Error) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.White;
                } else if (message.Severity == LogSeverity.Warning) {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                } else {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.Write($"[{message.Severity}] ");
                Console.ResetColor();
                Console.WriteLine($"{message}");
            }
            return Task.CompletedTask;
        }
    }
}