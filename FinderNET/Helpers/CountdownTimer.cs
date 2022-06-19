using Discord;
using Discord.WebSocket;
using FinderNET.Database.Repositories;
using FinderNET.Modules;
using FinderNET.Resources;
using System.Timers;
namespace FinderNET.Helpers {
    public static class CountdownTimer {
        private static System.Timers.Timer messageTimer;
        private static DiscordSocketClient client;
        private static CountdownRepository countdownRepository;
        public static void StartTimer(DiscordSocketClient _client, CountdownRepository _countdownRepository) {
            client = _client;
            countdownRepository = _countdownRepository;
            messageTimer = new System.Timers.Timer(3000);
            messageTimer.Elapsed += OnTimerElapsed;
            messageTimer.AutoReset = true;
            messageTimer.Enabled = true;
        }

        public static async void OnTimerElapsed(object source, ElapsedEventArgs e) {
            foreach (var c in countdownRepository.GetAll()) {
                SocketGuild guild = client.GetGuild((ulong)c.guildId);
                ITextChannel channel = (ITextChannel)guild.GetChannel((ulong)c.channelId);
                IUserMessage messages = (IUserMessage) await channel.GetMessageAsync((ulong)c.messageId);

                if (c.dateTime < DateTime.UtcNow || messages == null) {
                    if (messages != null) {
                        await messages.ModifyAsync(x => x.Embed = new EmbedBuilder() {
                            Title = "Countdown",
                            Color = Color.Orange,
                            Fields = new List<EmbedFieldBuilder> {
                                new EmbedFieldBuilder() {
                                    Name = "Time left",
                                    Value = "Times Up"
                                }
                            },
                            Footer = new EmbedFooterBuilder() {
                                Text = Main.EmbedFooter
                            }
                        }.Build());
                        Int64? userId = c.pingUserId;
                        Int64? roleId = c.pingRoleId;
                        if (userId != null) {
                            SocketGuildUser user = guild.GetUser((ulong)userId);
                            if (user != null) {
                                await channel.SendMessageAsync($"{user.Mention} times up!");
                            }
                        } else if (roleId != null) {
                            SocketRole role = guild.GetRole((ulong)roleId);
                            if (role != null) {
                                await channel.SendMessageAsync($"{role.Mention} times up!");
                            }
                        }
                    }
                    await countdownRepository.RemoveCountdownAsync((ulong)c.messageId, (ulong)c.channelId, (ulong)c.guildId);
                    await countdownRepository.SaveAsync();
                    continue;
                }
                TimeSpan timeLeft = c.dateTime - DateTime.Now.ToUniversalTime();
                await messages.ModifyAsync((x => x.Embed = new EmbedBuilder() {
                    Title = "Countdown",
                    Color = Color.Orange,
                    Fields = new List<EmbedFieldBuilder> {
                        new EmbedFieldBuilder() {
                            Name = "Time left",
                            Value = CountdownModule.HumanizeTime(timeLeft) + " left"
                        }
                    },
                    Footer = new EmbedFooterBuilder() {
                        Text = Main.EmbedFooter
                    }
                }.Build()));
            }
        }
    }
}