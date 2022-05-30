using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Rest;
using FinderNET.Database;

namespace FinderNET.Modules {
    public class ModerationModule : ModuleBase {
        public ModerationModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
        public List<ModerationMessage> moderationMessages = new List<ModerationMessage>();

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

        public async void ReactionAdded_Event(Cacheable<IUserMessage, UInt64> arg, ISocketMessageChannel arg1, SocketReaction reaction) {
            if (reaction.User.Value.IsBot) return;
            foreach (var moderationMessage in moderationMessages) {
                var guild = Context.Client.GetGuild(moderationMessage.guildId);
                if (guild == null) continue;
                var user = guild.GetUser(moderationMessage.userId);
                if (user == null) continue;
                var channel = guild.GetTextChannel(moderationMessage.channelId);
                if (channel == null) continue;
                var message = await channel.GetMessageAsync(moderationMessage.messageId);
                if (message == null) continue;
                var sender = guild.GetUser(moderationMessage.senderId);
                if (sender == null) continue;
                var reason = moderationMessage.reason;
                var type = moderationMessage.Type;
                if (message.Id == reaction.MessageId && reaction.UserId == Context.User.Id) {
                    if (reaction.User.Value.Id == moderationMessage.senderId && reaction.Emote.Name == "✅") {
                        if (type == ModerationMessageType.Ban) {
                            await Context.Guild.AddBanAsync(user, reason: moderationMessage.reason);
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
                                        Value = $"{reason}",
                                        IsInline = false
                                    }
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = $"FinderBot"
                                }
                            }.Build());
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
                                        Value = $"{reason}",
                                        IsInline = false
                                    },
                                },
                                Footer = new EmbedFooterBuilder() {
                                    Text = $"FinderBot"
                                },
                                ThumbnailUrl = guild.IconUrl
                            }.Build());
                            moderationMessages.Remove(moderationMessage);
                            await dataAccessLayer.SetUserBans((Int64)guild.Id, (Int64)user.Id, dataAccessLayer.GetUserBans((Int64)guild.Id, (Int64)user.Id) + 1);
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
        Ban
    }
}