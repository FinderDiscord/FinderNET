using Discord;
using Discord.WebSocket;
using FinderNET.Database.Repositories;
using FinderNET.Resources;
using System.Timers;
namespace FinderNET.Modules.Helpers {
    public static class CountdownTimer {
        private static System.Timers.Timer messageTimer;
        private static DiscordShardedClient client;
        private static CountdownRepository countdownRepository;
        public static void StartTimer(DiscordShardedClient _client, CountdownRepository _countdownRepository) {
            client = _client;
            countdownRepository = _countdownRepository;
            messageTimer = new System.Timers.Timer(3000);
            messageTimer.Elapsed += OnTimerElapsed;
            messageTimer.AutoReset = true;
            messageTimer.Enabled = true;
        }

        public static async void OnTimerElapsed(object source, ElapsedEventArgs e) {
            foreach (var c in (await countdownRepository.GetAllAsync())) {
                var guild = client.GetGuild((ulong)c.guildId);
                var channel = (ITextChannel)guild.GetChannel((ulong)c.channelId);
                var messages = (IUserMessage) await channel.GetMessageAsync((ulong)c.messageId);
                if (c.dateTime < DateTime.UtcNow || messages == null) {
                    if (messages != null) {
                        await messages.ModifyAsync(x => x.Embed = new EmbedBuilder() {
                            Title = CountdownLocale.CountdownEmbed_title,
                            Color = Color.Orange,
                            Fields = new List<EmbedFieldBuilder> {
                                new EmbedFieldBuilder() {
                                    Name = CountdownLocale.CountdownEmbed_fieldName,
                                    Value = CountdownLocale.CountdownEmbed_fieldValueEnd
                                }
                            },
                            Footer = new EmbedFooterBuilder() {
                                Text = Main.EmbedFooter
                            }
                        }.Build());
                        var userId = c.pingUserId;
                        var roleId = c.pingRoleId;
                        if (userId != null || roleId != null) {
                            SocketGuildUser? user = null;
                            SocketRole? role = null;
                            try {
                                user = guild.GetUser((ulong)userId);
                            } catch (Exception) {
                                // ignored
                            }
                            try {
                                role = guild.GetRole((ulong)roleId);
                            } catch (Exception) {
                                // ignored
                            }
                            if (user != null || role != null) {
                                await channel.SendMessageAsync(string.Format(CountdownLocale.CountdownPing, user?.Mention ?? role?.Mention));
                            }
                        }
                    }
                    await countdownRepository.RemoveCountdownAsync((ulong)c.messageId, (ulong)c.channelId, (ulong)c.guildId);
                    await countdownRepository.SaveAsync();
                    continue;
                }
                var timeLeft = c.dateTime - DateTime.Now.ToUniversalTime();
                await messages.ModifyAsync((x => x.Embed = new EmbedBuilder() {
                    Title = CountdownLocale.CountdownEmbed_title,
                    Color = Color.Orange,
                    Fields = new List<EmbedFieldBuilder> {
                        new EmbedFieldBuilder() {
                            Name = CountdownLocale.CountdownEmbed_fieldName,
                            Value = string.Format(CountdownLocale.CountdownEmbed_fieldValue, CountdownModule.HumanizeTime(timeLeft))
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