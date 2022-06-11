using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using FinderNET.Database;

namespace FinderNET.Modules {
    public class AdminModule : ModuleBase {
        public AdminModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
        
        [SlashCommand("purge", "Purge a number of messages", runMode: RunMode.Async)]
        public async Task PurgeCommand(int count) {
            var messages = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
            try {
                await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(messages);
            } catch (Exception e) {
                await ReplyAsync("An error occurred while purging messages.");
            }
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Purged",
                Color = Color.Orange,
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
        }

        [SlashCommand("slowmode", "Set the slowmode of a channel", runMode: RunMode.Async)]
        public async Task SlowmodeCommand(int seconds) {
            if (seconds < 0) {
                await ReplyAsync("The slowmode must be greater than or equal to 0.");
                return;
            }
            await ((SocketTextChannel)Context.Channel).ModifyAsync(x => x.SlowModeInterval = seconds);
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Slowmode set",
                Color = Color.Orange,
                Fields = {
                    new EmbedFieldBuilder() {
                        Name = "Channel",
                        Value = Context.Channel.Name,
                        IsInline = true
                    },
                    new EmbedFieldBuilder() {
                        Name = "Slowmode",
                        Value = seconds.ToString(),
                        IsInline = true
                    },
                    new EmbedFieldBuilder() {
                        Name = "By user",
                        Value = Context.User.Username,
                        IsInline = true
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
        }

        [SlashCommand("lockdown", "Lockdown a channel", runMode: RunMode.Async)]
        public async Task LockdownCommand() {
            OverwritePermissions overwrite = new OverwritePermissions(sendMessages: PermValue.Deny);
            foreach (SocketRole role in Context.Guild.Roles) {
                if (!role.Permissions.Administrator) {
                    await ((SocketTextChannel)Context.Channel).AddPermissionOverwriteAsync(role, overwrite);
                }
            }
            await RespondAsync("", embed: new EmbedBuilder() {
                Title = "Channel locked down",
                Color = Color.Orange,
                Fields = {
                    new EmbedFieldBuilder() {
                        Name = "Channel",
                        Value = Context.Channel.Name,
                        IsInline = true
                    },
                    new EmbedFieldBuilder() {
                        Name = "By user",
                        Value = Context.User.Username,
                        IsInline = true
                    }
                },
                Footer = new EmbedFooterBuilder() {
                    Text = "FinderBot"
                }
            }.Build());
        }
    }
}