using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Rest;
using Discord.Net;
using FinderNET.Database;

namespace FinderNET.Modules {
    public class ModerationModule : ModuleBase {
        public ModerationModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
        public static List<ModerationMessage> moderationMessages = new List<ModerationMessage>();

        [SlashCommand("ban", "Bans a user from the server.")]
        public async Task BanCommand(SocketGuildUser user, string reason = "No reason given.") {
            var confirmMessage = await ReplyAsync("", false, new EmbedBuilder() {
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
            await confirmMessage.AddReactionAsync(new Emoji("✅"));
            moderationMessages.Add(new ModerationMessage() {
                messageId = confirmMessage.Id,
                channelId = confirmMessage.Channel.Id,
                guildId = Context.Guild.Id,
                senderId = Context.User.Id,
                userId = user.Id,
                reason = reason,
                Type = ModerationMessageType.Ban
            });
        }

        [SlashCommand("kick", "Kicks a user from the server.")]
        public async Task KickCommand(SocketGuildUser user, string reason = "No reason given.") {
            var confirmMessage = await ReplyAsync("", false, new EmbedBuilder() {
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
            await confirmMessage.AddReactionAsync(new Emoji("✅"));
            moderationMessages.Add(new ModerationMessage() {
                messageId = confirmMessage.Id,
                channelId = confirmMessage.Channel.Id,
                guildId = Context.Guild.Id,
                senderId = Context.User.Id,
                userId = user.Id,
                reason = reason,
                Type = ModerationMessageType.Kick
            });
        }

        [SlashCommand("warn", "Warns a user.")]
        public async Task WarnCommand(SocketGuildUser user, string reason = "No reason given.") {
            var confirmMessage = await ReplyAsync("", false, new EmbedBuilder() {
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
            await confirmMessage.AddReactionAsync(new Emoji("✅"));
            moderationMessages.Add(new ModerationMessage() {
                messageId = confirmMessage.Id,
                channelId = confirmMessage.Channel.Id,
                guildId = Context.Guild.Id,
                senderId = Context.User.Id,
                userId = user.Id,
                reason = reason,
                Type = ModerationMessageType.Warn
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
                            var userBans = await dataAccessLayer.GetUserBans((Int64)guild.Id, (Int64)user.Id);
                            await dataAccessLayer.SetUserBans((Int64)guild.Id, (Int64)user.Id, userBans + 1);
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
                            var userKicks = await dataAccessLayer.GetUserKicks((Int64)guild.Id, (Int64)user.Id);
                            await dataAccessLayer.SetUserKicks((Int64)guild.Id, (Int64)user.Id, userKicks + 1);
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
                            var userWarns = await dataAccessLayer.GetUserWarns((Int64)guild.Id, (Int64)user.Id);
                            await dataAccessLayer.SetUserWarns((Int64)guild.Id, (Int64)user.Id, userWarns + 1);
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
                            if (await dataAccessLayer.GetSettingsValue((Int64)guild.Id, "muteRoleId") == null) {
                                var muteRole1 = await guild.CreateRoleAsync("Muted", new GuildPermissions(connect: true, readMessageHistory: true), Color.DarkGrey, false, true);
                                await dataAccessLayer.SetSettingsValue((Int64)guild.Id, "muteRoleId", muteRole1.Id.ToString());
                            }
                            var muteRole = guild.GetRole(Convert.ToUInt64(await dataAccessLayer.GetSettingsValue((Int64)guild.Id, "muteRoleId")));
                            await user.AddRoleAsync(muteRole);
                            await message.RemoveAllReactionsAsync();
                            moderationMessages.Remove(moderationMessage);
                            var userMutes = await dataAccessLayer.GetUserMutes((Int64)guild.Id, (Int64)user.Id);
                            await dataAccessLayer.SetUserMutes((Int64)guild.Id, (Int64)user.Id, userMutes + 1);
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