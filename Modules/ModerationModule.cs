using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using FinderNET.Database.Repositories;
using FinderNET.Modules.Helpers;
using FinderNET.Modules.Helpers.Enums;
using FinderNET.Resources;

namespace FinderNET.Modules {
    public class ModerationModule : InteractionModuleBase<ShardedInteractionContext> {
        private readonly SettingsRepository settingsRepository;
        private readonly UserLogsRepository userLogsRepository;
        public ModerationModule(SettingsRepository _settingsRepository, UserLogsRepository _userLogsRepository) {
            settingsRepository = _settingsRepository;
            userLogsRepository = _userLogsRepository;
        }
        
        // todo: permissions

        List<ModerationMessage> moderationMessages = new List<ModerationMessage>();

        [SlashCommand("ban", "Bans a user from the server.", runMode: RunMode.Async)]
        public async Task BanCommand(SocketGuildUser user, string reason = "No reason given.") {
            await RespondAsync(embed: new EmbedBuilder() {
                Title = ModerationLocale.ModerationEmbedBan_title,
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldUserName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldUserValue, user.Mention, user.Username),
                        IsInline = false
                    },
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, reason),
                        IsInline = false
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = Main.EmbedFooter
                }
            }.Build());
            var message = await GetOriginalResponseAsync();
            await message.AddReactionAsync(new Emoji("✅"));
            moderationMessages.Add(new ModerationMessage() {
                messageId = message.Id,
                channelId = message.Channel.Id,
                guildId = Context.Guild.Id,
                senderId = Context.User.Id,
                userId = user.Id,
                reason = reason,
                Type = ModerationMessageType.Ban
            });
        }

        [SlashCommand("kick", "Kicks a user from the server.", runMode: RunMode.Async)]
        public async Task KickCommand(SocketGuildUser user, string reason = "No reason given.") {
            await RespondAsync(embed: new EmbedBuilder() {
                Title = ModerationLocale.ModerationEmbedKick_title,
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldUserName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldUserValue, user.Mention, user.Username),
                        IsInline = false
                    },
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, reason),
                        IsInline = false
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = Main.EmbedFooter
                }
            }.Build());
            var message = await GetOriginalResponseAsync();
            await message.AddReactionAsync(new Emoji("✅"));
            moderationMessages.Add(new ModerationMessage() {
                messageId = message.Id,
                channelId = message.Channel.Id,
                guildId = Context.Guild.Id,
                senderId = Context.User.Id,
                userId = user.Id,
                reason = reason,
                Type = ModerationMessageType.Kick
            });
        }

        [SlashCommand("warn", "Warns a user.", runMode: RunMode.Async)]
        public async Task WarnCommand(SocketGuildUser user, string reason = "No reason given.") {
            await RespondAsync(embed: new EmbedBuilder() {
                Title = ModerationLocale.ModerationEmbedWarn_title,
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldUserName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldUserValue, user.Mention, user.Username),
                        IsInline = false
                    },
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, reason),
                        IsInline = false
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = Main.EmbedFooter
                }
            }.Build());
            var message = await GetOriginalResponseAsync();
            await message.AddReactionAsync(new Emoji("✅"));
            moderationMessages.Add(new ModerationMessage() {
                messageId = message.Id,
                channelId = message.Channel.Id,
                guildId = Context.Guild.Id,
                senderId = Context.User.Id,
                userId = user.Id,
                reason = reason,
                Type = ModerationMessageType.Warn
            });
        }

        [SlashCommand("mute", "Mutes a user.", runMode: RunMode.Async)]
        public async Task MuteCommand(SocketGuildUser user, string reason = "No reason given.") {
            await RespondAsync(embed: new EmbedBuilder() {
                Title = ModerationLocale.ModerationEmbedMute_title,
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldUserName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldUserValue, user.Mention, user.Username),
                        IsInline = false
                    },
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, reason),
                        IsInline = false
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = Main.EmbedFooter
                }
            }.Build());
            var message = await GetOriginalResponseAsync();
            await message.AddReactionAsync(new Emoji("✅"));
            moderationMessages.Add(new ModerationMessage() {
                messageId = message.Id,
                channelId = message.Channel.Id,
                guildId = Context.Guild.Id,
                senderId = Context.User.Id,
                userId = user.Id,
                reason = reason,
                Type = ModerationMessageType.Mute
            });
        }

        public async Task OnReactionAddedEvent(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction reaction) {
            if (reaction.User.Value.IsBot) return;
            foreach (var moderationMessage in moderationMessages) {
                var guild = ((SocketGuildChannel)reaction.Channel).Guild;
                var channel = (SocketTextChannel)guild.GetChannel(moderationMessage.channelId);
                var message = await channel.GetMessageAsync(moderationMessage.messageId);
                var user = guild.GetUser(moderationMessage.userId);
                if (guild.Id != moderationMessage.guildId || message.Id != reaction.MessageId || reaction.UserId != moderationMessage.senderId) continue;
                if (reaction.User.Value.Id != moderationMessage.senderId || reaction.Emote.Name != "✅") continue;
                var userLogs = await userLogsRepository.GetUserLogsAsync(guild.Id, user.Id);
                switch(moderationMessage.Type) {
                    case ModerationMessageType.Ban:
                        await guild.AddBanAsync(user, reason: moderationMessage.reason);
                        await message.RemoveAllReactionsAsync();
                        await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                            Title = ModerationLocale.ModerationEmbedBanned_title,
                            Color = Color.Red,
                            Fields = new List<EmbedFieldBuilder>() {
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldUserName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldUserValue, user.Mention, user.Username),
                                    IsInline = false
                                },
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, moderationMessage.reason),
                                    IsInline = false
                                }
                            },
                            Footer = new EmbedFooterBuilder() {
                                Text = Main.EmbedFooter
                            }
                        }.Build());
                        try {
                            await user.SendMessageAsync(embed: new EmbedBuilder() {
                                Title = ModerationLocale.ModerationEmbedBannedDM_title,
                                Color = Color.Red,
                                Fields = new List<EmbedFieldBuilder>() {
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldServerName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldServerValue, guild.Name),
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, moderationMessage.reason),
                                        IsInline = false
                                    },
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = Main.EmbedFooter
                                },
                                ThumbnailUrl = guild.IconUrl
                            }.Build());
                        } catch (HttpException) {
                            // User has DMs disabled
                        }
                        moderationMessages.Remove(moderationMessage);
                        await userLogsRepository.AddUserLogsAsync(guild.Id, user.Id, userLogs.bans + 1, userLogs.kicks, userLogs.warns, userLogs.mutes);
                        await userLogsRepository.SaveAsync();
                        return;
                    case ModerationMessageType.Kick:
                        await user.KickAsync(moderationMessage.reason);
                        await message.RemoveAllReactionsAsync();
                        await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                            Title = ModerationLocale.ModerationEmbedKicked_title,
                            Color = Color.Red,
                            Fields = new List<EmbedFieldBuilder>() {
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldUserName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldUserValue, user.Mention, user.Username),
                                    IsInline = false
                                },
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, moderationMessage.reason),
                                    IsInline = false
                                }
                            },
                            Footer = new EmbedFooterBuilder() {
                                Text = Main.EmbedFooter
                            }
                        }.Build());
                        try {
                            await user.SendMessageAsync(embed: new EmbedBuilder() {
                                Title = ModerationLocale.ModerationEmbedKickedDM_title,
                                Color = Color.Red,
                                Fields = new List<EmbedFieldBuilder>() {
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldServerName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldServerValue, guild.Name),
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, moderationMessage.reason),
                                        IsInline = false
                                    },
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = Main.EmbedFooter
                                },
                                ThumbnailUrl = guild.IconUrl
                            }.Build());
                        } catch (HttpException) {
                            // User has DMs disabled
                        }
                        moderationMessages.Remove(moderationMessage);
                        await userLogsRepository.AddUserLogsAsync(guild.Id, user.Id, userLogs.bans, userLogs.kicks + 1, userLogs.warns, userLogs.mutes);
                        await userLogsRepository.SaveAsync();
                        return;
                    case ModerationMessageType.Warn:
                        await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                            Title = ModerationLocale.ModerationEmbedWarned_title,
                            Color = Color.Red,
                            Fields = new List<EmbedFieldBuilder>() {
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldUserName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldUserValue, user.Mention, user.Username),
                                    IsInline = false
                                },
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, moderationMessage.reason),
                                    IsInline = false
                                }
                            },
                            Footer = new EmbedFooterBuilder() {
                                Text = Main.EmbedFooter
                            }
                        }.Build());
                        try {
                            await user.SendMessageAsync(embed: new EmbedBuilder() {
                                Title = ModerationLocale.ModerationEmbedWarnedDM_title,
                                Color = Color.Red,
                                Fields = new List<EmbedFieldBuilder>() {
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldServerName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldServerValue, guild.Name),
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, moderationMessage.reason),
                                        IsInline = false
                                    },
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = Main.EmbedFooter
                                },
                                ThumbnailUrl = guild.IconUrl
                            }.Build());
                        } catch (HttpException) {
                            // User has DMs disabled
                        }
                        moderationMessages.Remove(moderationMessage);
                        await userLogsRepository.AddUserLogsAsync(guild.Id, user.Id, userLogs.bans, userLogs.kicks, userLogs.warns + 1, userLogs.mutes);
                        await userLogsRepository.SaveAsync();
                        return;
                    case ModerationMessageType.Mute:
                    default:
                        await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                            Title = ModerationLocale.ModerationEmbedMuted_title,
                            Color = Color.Red,
                            Fields = new List<EmbedFieldBuilder>() {
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldUserName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldUserValue, user.Mention, user.Username),
                                    IsInline = false
                                },
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, moderationMessage.reason),
                                    IsInline = false
                                }
                            },
                            Footer = new EmbedFooterBuilder() {
                                Text = Main.EmbedFooter
                            }
                        }.Build());
                        try {
                            await user.SendMessageAsync(embed: new EmbedBuilder() {
                                Title = ModerationLocale.ModerationEmbedMutedDM_title,
                                Color = Color.Red,
                                Fields = new List<EmbedFieldBuilder>() {
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldServerName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldServerValue, guild.Name),
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue, moderationMessage.reason),
                                        IsInline = false
                                    },
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = Main.EmbedFooter
                                },
                                ThumbnailUrl = guild.IconUrl
                            }.Build());
                        } catch (HttpException) {
                            // User has DMs disabled
                        }
                        if (!(await settingsRepository.SettingsExists(guild.Id, "muteRoleId"))) {
                            var muteRole1 = await guild.CreateRoleAsync("Muted", new GuildPermissions(connect: true, readMessageHistory: true), Color.DarkGrey, false, true);
                            await settingsRepository.AddSettingsAsync(guild.Id, "muteRoleId", muteRole1.Id.ToString());
                            await settingsRepository.SaveAsync();
                            foreach (var otherchannel in guild.Channels) {
                                await channel.AddPermissionOverwriteAsync(muteRole1, OverwritePermissions.DenyAll(channel).Modify(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow));
                            }
                        }
                        var muteRole = guild.GetRole(Convert.ToUInt64(await settingsRepository.GetSettingAsync(guild.Id, "muteRoleId")));
                        await user.AddRoleAsync(muteRole);
                        await message.RemoveAllReactionsAsync();
                        moderationMessages.Remove(moderationMessage);
                        await userLogsRepository.AddUserLogsAsync(guild.Id, user.Id, userLogs.bans, userLogs.kicks, userLogs.warns, userLogs.mutes + 1);
                        await userLogsRepository.SaveAsync();
                        return;
                }
            }
        }
    }
}