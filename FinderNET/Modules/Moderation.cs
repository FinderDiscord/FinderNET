using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Net;
using FinderNET.Database.Repositories;

namespace FinderNET.Modules {
    public class ModerationModule : InteractionModuleBase<SocketInteractionContext> {
        private readonly SettingsRepository settingsRepository;
        private readonly UserLogsRepository userLogsRepository;
        public ModerationModule(SettingsRepository _settingsRepository, UserLogsRepository _userLogsRepository) {
            settingsRepository = _settingsRepository;
            userLogsRepository = _userLogsRepository;
        }

        public static List<ModerationMessage> moderationMessages = new List<ModerationMessage>();

        [SlashCommand("ban", "Bans a user from the server.")]
        public async Task BanCommand(SocketGuildUser user, string reason = "No reason given.") {
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Are you sure you want to ban this user?",
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = "User",
                        Value = $"{user.Mention} ({user.Username})",
                        IsInline = false
                    },
                    new EmbedFieldBuilder() {
                        Name = "for reason",
                        Value = $"{reason}",
                        IsInline = false
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = $"FinderBot"
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

        [SlashCommand("kick", "Kicks a user from the server.")]
        public async Task KickCommand(SocketGuildUser user, string reason = "No reason given.") {
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Are you sure you want to kick this user?",
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = "User",
                        Value = $"{user.Mention} ({user.Username})",
                        IsInline = false
                    },
                    new EmbedFieldBuilder() {
                        Name = "for reason",
                        Value = $"{reason}",
                        IsInline = false
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = $"FinderBot"
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

        [SlashCommand("warn", "Warns a user.")]
        public async Task WarnCommand(SocketGuildUser user, string reason = "No reason given.") {
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Are you sure you want to warn this user?",
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = "User",
                        Value = $"{user.Mention} ({user.Username})",
                        IsInline = false
                    },
                    new EmbedFieldBuilder() {
                        Name = "for reason",
                        Value = $"{reason}",
                        IsInline = false
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = $"FinderBot"
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

        [SlashCommand("mute", "Mutes a user.")]
        public async Task MuteCommand(SocketGuildUser user, string reason = "No reason given.") {
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Are you sure you want to mute this user?",
                Color = Color.Red,
                Fields = new List<EmbedFieldBuilder>() {
                    new EmbedFieldBuilder() {
                        Name = "User",
                        Value = $"{user.Mention} ({user.Username})",
                        IsInline = false
                    },
                    new EmbedFieldBuilder() {
                        Name = "for reason",
                        Value = $"{reason}",
                        IsInline = false
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = $"FinderBot"
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
                SocketTextChannel channel = (SocketTextChannel)guild.GetChannel(moderationMessage.channelId);
                var message = await channel.GetMessageAsync(moderationMessage.messageId);
                var user = guild.GetUser(moderationMessage.userId);
                if (guild.Id == moderationMessage.guildId && message.Id == reaction.MessageId && reaction.UserId == moderationMessage.senderId) {
                    if (reaction.User.Value.Id == moderationMessage.senderId && reaction.Emote.Name == "✅") {
                        var userLogs = await userLogsRepository.GetUserLogsAsync(guild.Id, user.Id);
                        if (moderationMessage.Type == ModerationMessageType.Ban) {
                            await guild.AddBanAsync(user, reason: moderationMessage.reason);
                            await message.RemoveAllReactionsAsync();
                            await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                                Title = "User Banned",
                                Color = Color.Red,
                                Fields = new List<EmbedFieldBuilder>() {
                                    new EmbedFieldBuilder() {
                                        Name = "User",
                                        Value = $"{user.Mention} ({user})",
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = "for reason",
                                        Value = $"{moderationMessage.reason}",
                                        IsInline = false
                                    }
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = $"FinderBot"
                                }
                            }.Build());
                            try {
                                await user.SendMessageAsync("", false, new EmbedBuilder() {
                                    Title = "You have been banned",
                                    Color = Color.Red,
                                    Fields = new List<EmbedFieldBuilder>() {
                                        new EmbedFieldBuilder() {
                                            Name = "Server",
                                            Value = $"{guild.Name}",
                                            IsInline = false
                                        },
                                        new EmbedFieldBuilder() {
                                            Name = "Reason",
                                            Value = $"{moderationMessage.reason}",
                                            IsInline = false
                                        },
                                    },
                                    Footer = new EmbedFooterBuilder() {
                                        Text = $"FinderBot"
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
                        } else if (moderationMessage.Type == ModerationMessageType.Kick) {
                            await user.KickAsync(moderationMessage.reason);
                            await message.RemoveAllReactionsAsync();
                            await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                                Title = "User Kicked",
                                Color = Color.Red,
                                Fields = new List<EmbedFieldBuilder>() {
                                    new EmbedFieldBuilder() {
                                        Name = "User",
                                        Value = $"{user.Mention} ({user})",
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = "for reason",
                                        Value = $"{moderationMessage.reason}",
                                        IsInline = false
                                    }
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = $"FinderBot"
                                }
                            }.Build());
                            try {
                                await user.SendMessageAsync("", false, new EmbedBuilder() {
                                    Title = "You have been kicked",
                                    Color = Color.Red,
                                    Fields = new List<EmbedFieldBuilder>() {
                                        new EmbedFieldBuilder() {
                                            Name = "Server",
                                            Value = $"{guild.Name}",
                                            IsInline = false
                                        },
                                        new EmbedFieldBuilder() {
                                            Name = "Reason",
                                            Value = $"{moderationMessage.reason}",
                                            IsInline = false
                                        },
                                    },
                                    Footer = new EmbedFooterBuilder() {
                                        Text = $"FinderBot"
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
                        } else if (moderationMessage.Type == ModerationMessageType.Warn) {
                            await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                                Title = "User Warned",
                                Color = Color.Red,
                                Fields = new List<EmbedFieldBuilder>() {
                                    new EmbedFieldBuilder() {
                                        Name = "User",
                                        Value = $"{user.Mention} ({user})",
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = "for reason",
                                        Value = $"{moderationMessage.reason}",
                                        IsInline = false
                                    }
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = $"FinderBot"
                                }
                            }.Build());
                            try {
                                await user.SendMessageAsync("", false, new EmbedBuilder() {
                                    Title = "You have been warned",
                                    Color = Color.Red,
                                    Fields = new List<EmbedFieldBuilder>() {
                                        new EmbedFieldBuilder() {
                                            Name = "Server",
                                            Value = $"{guild.Name}",
                                            IsInline = false
                                        },
                                        new EmbedFieldBuilder() {
                                            Name = "Reason",
                                            Value = $"{moderationMessage.reason}",
                                            IsInline = false
                                        },
                                    },
                                    Footer = new EmbedFooterBuilder() {
                                        Text = $"FinderBot"
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
                        } else if (moderationMessage.Type == ModerationMessageType.Mute) {
                            await channel.ModifyMessageAsync(message.Id, m => m.Embed = new EmbedBuilder() {
                                Title = "User Muted",
                                Color = Color.Red,
                                Fields = new List<EmbedFieldBuilder>() {
                                    new EmbedFieldBuilder() {
                                        Name = "User",
                                        Value = $"{user.Mention} ({user})",
                                        IsInline = false
                                    },
                                    new EmbedFieldBuilder() {
                                        Name = "for reason",
                                        Value = $"{moderationMessage.reason}",
                                        IsInline = false
                                    }
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = $"FinderBot"
                                }
                            }.Build());
                            try {
                                await user.SendMessageAsync("", false, new EmbedBuilder() {
                                    Title = "You have been muted",
                                    Color = Color.Red,
                                    Fields = new List<EmbedFieldBuilder>() {
                                        new EmbedFieldBuilder() {
                                            Name = "Server",
                                            Value = $"{guild.Name}",
                                            IsInline = false
                                        },
                                        new EmbedFieldBuilder() {
                                            Name = "Reason",
                                            Value = $"{moderationMessage.reason}",
                                            IsInline = false
                                        },
                                    },
                                    Footer = new EmbedFooterBuilder() {
                                        Text = $"FinderBot"
                                    },
                                    ThumbnailUrl = guild.IconUrl
                                }.Build());
                            } catch (HttpException) {
                                // User has DMs disabled
                            }
                            if (!settingsRepository.SettingsExists(guild.Id, "muteRoleId")) {
                                var muteRole1 = await guild.CreateRoleAsync("Muted", new GuildPermissions(connect: true, readMessageHistory: true), Color.DarkGrey, false, true);
                                await settingsRepository.AddSettingsAsync(guild.Id, "muteRoleId", muteRole1.Id.ToString());
                                await settingsRepository.SaveAsync();
                                foreach (var otherchannel in guild.Channels) {
                                    await channel.AddPermissionOverwriteAsync(muteRole1, OverwritePermissions.DenyAll(channel).Modify(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow));
                                }
                            }
                            var muteRole = guild.GetRole(Convert.ToUInt64(await settingsRepository.GetSettingsAsync(guild.Id, "muteRoleId")));
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
    }



    public class ModerationMessage {
        public ulong messageId { get; set; }
        public ulong guildId { get; set; }
        public ulong channelId { get; set; }
        public ulong senderId { get; set; }
        public ulong userId { get; set; }
        public string reason { get; set; } = "No reason given.";
        public ModerationMessageType Type { get; set; } = ModerationMessageType.Null;
        // add date?
    }

    public enum ModerationMessageType {
        Null,
        Ban,
        Kick,
        Warn,
        Mute
    }
}