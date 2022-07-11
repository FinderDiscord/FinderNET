using Discord;
using Discord.WebSocket;
using FinderNET.Database.Repositories;
using FinderNET.Resources;
using System.Timers;
namespace FinderNET.Modules.Helpers {
    public static class UnBanMuteTimer {
        private static System.Timers.Timer messageTimer;
        private static DiscordShardedClient client;
        private static UserLogsRepository userLogsRepository;
        private static SettingsRepository settingsRepository;
        public static void StartTimer(DiscordShardedClient _client, UserLogsRepository _userLogsRepository, SettingsRepository _settingsRepository) {
            client = _client;
            userLogsRepository = _userLogsRepository;
            settingsRepository = _settingsRepository;
            messageTimer = new System.Timers.Timer(5000);
            messageTimer.Elapsed += OnTimerElapsed;
            messageTimer.AutoReset = true;
            messageTimer.Enabled = true;
        }

        public static async void OnTimerElapsed(object source, ElapsedEventArgs e) {
            foreach (var c in (await userLogsRepository.GetAllAsync())) {
                var guild = client.GetGuild((ulong)c.guildId);
                if (c.tempBan != null && c.tempBan < DateTime.UtcNow) {
                    await guild.RemoveBanAsync((ulong)c.userId);
                    SocketGuildUser user;
                    try {
                        user = guild.GetUser((ulong)c.userId);
                        await user.SendMessageAsync(embed: new EmbedBuilder() {
                            Title = ModerationLocale.ModerationEmbedUnbannedDM_title,
                            Color = Color.Red,
                            Fields = new List<EmbedFieldBuilder>() {
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldServerName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldServerValue, guild.Name),
                                    IsInline = false
                                }
                            },
                            Footer = new EmbedFooterBuilder() {
                                Text = Main.EmbedFooter
                            },
                            ThumbnailUrl = guild.IconUrl
                        }.Build());
                    } catch (Exception) {
                        // ignored
                    }
                    await userLogsRepository.RemoveTempbanTime((ulong)c.guildId, (ulong)c.userId);
                    await userLogsRepository.SaveAsync();
                    continue;
                } else if (c.tempMute != null && c.tempMute < DateTime.UtcNow) {
                    SocketGuildUser user;
                    try {
                        user = guild.GetUser((ulong)c.userId);
                        await user.SendMessageAsync(embed: new EmbedBuilder() {
                            Title = ModerationLocale.ModerationEmbedUnmutedDM_title,
                            Color = Color.Red,
                            Fields = new List<EmbedFieldBuilder>() {
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldServerName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldServerValue, guild.Name),
                                    IsInline = false
                                }
                            },
                            Footer = new EmbedFooterBuilder() {
                                Text = Main.EmbedFooter
                            },
                            ThumbnailUrl = guild.IconUrl
                        }.Build());
                        var muteRole = guild.GetRole(Convert.ToUInt64(await settingsRepository.GetSettingAsync(guild.Id, "muteRoleId")));
                        await user.RemoveRoleAsync(muteRole);
                    } catch (Exception) {
                        // ignored
                    }
                    await userLogsRepository.RemoveTempmuteTime((ulong)c.guildId, (ulong)c.userId);
                    await userLogsRepository.SaveAsync();
                }
            }
        }
    }
}