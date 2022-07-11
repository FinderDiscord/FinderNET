using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using FinderNET.Database.Repositories;
using FinderNET.Modules.Helpers;
using FinderNET.Modules.Helpers.Enums;
using FinderNET.Resources;
using Pathoschild.NaturalTimeParser.Parser;

namespace FinderNET.Modules {
    public class ModerationModule : InteractionModuleBase<ShardedInteractionContext> {
        private readonly SettingsRepository settingsRepository;
        private readonly UserLogsRepository userLogsRepository;
        public ModerationModule(SettingsRepository _settingsRepository, UserLogsRepository _userLogsRepository) {
            settingsRepository = _settingsRepository;
            userLogsRepository = _userLogsRepository;
        }
        
        // todo: permissions

        public static List<ModerationMessage> moderationMessages = new List<ModerationMessage>();

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
        
        [SlashCommand("unmute", "Unmutes a user.", runMode: RunMode.Async)]
        public async Task UnmuteCommand(SocketGuildUser user) {
            await RespondAsync(embed: new EmbedBuilder() {
                Title = ModerationLocale.ModerationEmbedUnmute_title,
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldUserName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldUserValue, user.Mention, user.Username),
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
                Type = ModerationMessageType.Unmute
            });
        }
        
        [SlashCommand("unban", "Unbans a user.", runMode: RunMode.Async)]
        public async Task UnbanCommand(SocketGuildUser user) {
            await RespondAsync(embed: new EmbedBuilder() {
                Title = ModerationLocale.ModerationEmbedUnban_title,
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldUserName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldUserValue, user.Mention, user.Username),
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
                Type = ModerationMessageType.Unban
            });
        }
        
        [SlashCommand("tempban", "Bans a user for a certain amount of time.", runMode: RunMode.Async)]
        public async Task TempBanCommand(SocketGuildUser user, string time, string reason = "No reason given.") {
            DateTime timeSpan = DateTime.UtcNow + TimeSpan.Parse(time);
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
                    },
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldTimeName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldTimeValue, time),
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
                Type = ModerationMessageType.Tempban,
                time = timeSpan
            });
        }
        
        [SlashCommand("tempmute", "Mutes a user for a certain amount of time.", runMode: RunMode.Async)]
        public async Task TempMuteCommand(SocketGuildUser user, string time, string reason = "No reason given.") {
            DateTime timeSpan = DateTime.UtcNow + TimeSpan.Parse(time);
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
                    },
                    new EmbedFieldBuilder() {
                        Name = ModerationLocale.ModerationEmbed_fieldTimeName,
                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldTimeValue, time),
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
                Type = ModerationMessageType.Tempmute,
                time = timeSpan
            });
        }

        [SlashCommand("logs", "Displays the logs for a user.", runMode: RunMode.Async)]
        public async Task LogsCommand(SocketGuildUser user = null) {
            var logs = await userLogsRepository.GetUserLogsAsync(Context.Guild.Id, (user?.Id ?? Context.User.Id));
            var muteRoleId = await settingsRepository.GetSettingAsync(Context.Guild.Id, "muteRoleId");
            var ismuted = ((SocketGuildUser)(user ?? Context.User)).Roles.Any(x => x.Id == ulong.Parse(muteRoleId));
            await RespondAsync(embed: new EmbedBuilder() {
                Title = "Logs for " + (user == null ? Context.User.Username : user.Username),
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = "Warnings",
                        Value = logs.warns,
                        IsInline = true
                    },
                    new EmbedFieldBuilder() {
                        Name = "Mutes",
                        Value = logs.mutes,
                        IsInline = true
                    },
                    new EmbedFieldBuilder() {
                        Name = "Kicks",
                        Value = logs.kicks,
                        IsInline = true
                    },
                    new EmbedFieldBuilder() {
                        Name = "Bans",
                        Value = logs.bans,
                        IsInline = true
                    },
                    new EmbedFieldBuilder() {
                        Name = "Is Muted",
                        Value = ismuted ? "Yes" : "No",
                        IsInline = true
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = Main.EmbedFooter
                }
            }.Build());
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
                    case ModerationMessageType.Unmute:
                        await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                            Title = ModerationLocale.ModerationEmbedUnmuted_title,
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
                                Title = ModerationLocale.ModerationEmbedUnmutedDM_title,
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
                        var muteRole2 = guild.GetRole(Convert.ToUInt64(await settingsRepository.GetSettingAsync(guild.Id, "muteRoleId")));
                        await user.RemoveRoleAsync(muteRole2);
                        await message.RemoveAllReactionsAsync();
                        moderationMessages.Remove(moderationMessage);
                        return;
                    case ModerationMessageType.Unban:
                        await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                            Title = ModerationLocale.ModerationEmbedUnbanned_title,
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
                                Title = ModerationLocale.ModerationEmbedUnbannedDM_title,
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
                        await message.RemoveAllReactionsAsync();
                        moderationMessages.Remove(moderationMessage);
                        return;
                    case ModerationMessageType.Tempban:
                        await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                            Title = ModerationLocale.ModerationEmbedTempbanned_title,
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
                                },
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldDurationName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldDurationValue, moderationMessage.time!.Value),
                                    IsInline = false
                                }
                            },
                            Footer = new EmbedFooterBuilder() {
                                Text = Main.EmbedFooter
                            }
                        }.Build());
                        try {
                            await user.SendMessageAsync(embed: new EmbedBuilder() {
                                Title = ModerationLocale.ModerationEmbedTempbannedDM_title,
                                Color = Color.Red,
                                Fields = new List<EmbedFieldBuilder>() {
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldServerName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldServerValue, guild.Name),
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue,
                                        moderationMessage.reason),
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldDurationName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldDurationValue, moderationMessage.time!.Value),
                                        IsInline = false
                                    }
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = Main.EmbedFooter
                                },
                                ThumbnailUrl = guild.IconUrl
                            }.Build());
                        } catch (HttpException) {
                            // User has DMs disabled
                        }
                        await userLogsRepository.AddTempbanTime(Context.Guild.Id, user.Id, moderationMessage.time!.Value);
                        await userLogsRepository.SaveAsync();
                        await message.RemoveAllReactionsAsync();
                        moderationMessages.Remove(moderationMessage);
                        return;
                    case ModerationMessageType.Tempmute:
                        await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                            Title = ModerationLocale.ModerationEmbedTempmuted_title,
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
                                },
                                new EmbedFieldBuilder() {
                                    Name = ModerationLocale.ModerationEmbed_fieldDurationName,
                                    Value = string.Format(ModerationLocale.ModerationEmbed_fieldDurationValue, moderationMessage.time!.Value),
                                    IsInline = false
                                }
                            },
                            Footer = new EmbedFooterBuilder() {
                                Text = Main.EmbedFooter
                            }
                        }.Build());
                        try {
                            await user.SendMessageAsync(embed: new EmbedBuilder() {
                                Title = ModerationLocale.ModerationEmbedTempmutedDM_title,
                                Color = Color.Red,
                                Fields = new List<EmbedFieldBuilder>() {
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldServerName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldServerValue, guild.Name),
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldReasonName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldReasonValue,
                                        moderationMessage.reason),
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = ModerationLocale.ModerationEmbed_fieldDurationName,
                                        Value = string.Format(ModerationLocale.ModerationEmbed_fieldDurationValue, moderationMessage.time!.Value),
                                        IsInline = false
                                    }
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = Main.EmbedFooter
                                },
                                ThumbnailUrl = guild.IconUrl
                            }.Build());
                        } catch (HttpException) {
                            // User has DMs disabled
                        }
                        await userLogsRepository.AddTempmuteTime(Context.Guild.Id, user.Id, moderationMessage.time!.Value);
                        await userLogsRepository.SaveAsync();
                        await message.RemoveAllReactionsAsync();
                        moderationMessages.Remove(moderationMessage);
                        return;
                }
            }
        }
    }
}