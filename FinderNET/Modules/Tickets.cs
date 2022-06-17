using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FinderNET.Database.Models;
using FinderNET.Database.Repositories;

namespace FinderNET.Modules {
    public class TicketingModule {
        [Group("tickets", "Command For Managing Tickets")]
        public class TicketsModule : InteractionModuleBase<InteractionContext> {
            ulong _closeConfirmId;
            private readonly TicketsRepository ticketsRepository;
            public TicketsModule(TicketsRepository _ticketsRepository) {
                ticketsRepository = _ticketsRepository;
            }
            [SlashCommand("create", "Creates a ticket")]
            public async Task CreateTicket(string? name = null) {
                if (name == null) {
                    await RespondAsync("Please specify a name for the ticket.");
                    return;
                }
                if (name.Length > 32) {
                    await RespondAsync("The name of the ticket is too long.");
                    return;
                }
                var supportChannel = await Context.Guild.CreateTextChannelAsync($"ticket-");
                await supportChannel.AddPermissionOverwriteAsync(Context.User, new OverwritePermissions(
                    addReactions: PermValue.Allow,
                    attachFiles: PermValue.Allow,
                    embedLinks: PermValue.Allow,
                    readMessageHistory: PermValue.Allow,
                    sendMessages: PermValue.Allow,
                    viewChannel: PermValue.Allow,
                    useApplicationCommands: PermValue.Allow
                ));
                await supportChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(
                    readMessageHistory: PermValue.Deny,
                    sendMessages: PermValue.Deny,
                    viewChannel: PermValue.Deny
                ));
                var components = new ComponentBuilder()
                    .WithButton("Close Ticket", "close")
                    .WithButton("Claim Ticket", "claim")
                    .Build();
                var message = await supportChannel.SendMessageAsync(embed: new EmbedBuilder() {
                    Title = "Ticket",
                    Fields = new List<EmbedFieldBuilder>() {
                        new EmbedFieldBuilder() {
                            Name = name,
                            Value = $"Channel made by {Context.User.Username}"
                        }
                    },
                    Color = Color.Green
                }.Build(), components: components);
                await ticketsRepository.AddTicketAsync(Context.Guild.Id, supportChannel.Id, message.Id, new List<long?>() { (long)Context.User.Id }, name, new List<long>());
                await ticketsRepository.SaveAsync();
                await RespondAsync(embed: new EmbedBuilder() {
                    Title = "Ticket Created",
                    Description = $"Opened a new ticket: {supportChannel.Mention}",
                    Color = Color.Green
                }.Build());
            }

            [SlashCommand("close", "Closes a ticket")]
            public async Task CloseTicket() {
                var ticket = await ticketsRepository.GetTicketsAsync(Context.Guild.Id, Context.Channel.Id);
                if (ticket.introMessageId == null
                    || await Context.Channel.GetMessageAsync((ulong)ticket.introMessageId) == null) {
                    await RespondAsync("You are not in a ticket channel.", ephemeral: true);
                    return;
                }
                if (!ticket.userIds.Contains((long)Context.User.Id) || !ticket.claimedUserId.Contains((long)Context.User.Id)) {
                    await RespondAsync("You are not the owner of this ticket.", ephemeral: true);
                    return;
                }
                await RespondAsync("Ticket Closed");
                await ((SocketGuildChannel)Context.Channel).DeleteAsync();
                await ticketsRepository.RemoveTicketAsync(Context.Guild.Id, Context.Channel.Id);
                await ticketsRepository.SaveAsync();
            }

            [SlashCommand("claim", "Claims a ticket")]
            public async Task ClaimTicket() {
                if (!((SocketGuildUser)Context.User).GuildPermissions.Administrator) {
                    await RespondAsync("You do not have permission to use this command.", ephemeral: true);
                    return;
                }
                var ticket = await ticketsRepository.GetTicketsAsync(Context.Guild.Id, Context.Channel.Id);
                if (ticket.introMessageId == null
                    || await Context.Channel.GetMessageAsync((ulong)ticket.introMessageId) == null) {
                    await RespondAsync("You are not in a ticket channel.", ephemeral: true);
                    return;
                }
                if (ticket.claimedUserId.Contains((long)Context.User.Id)) {
                    await RespondAsync("You have already claimed this ticket.", ephemeral: true);
                    return;
                }
                await ((SocketGuildChannel)Context.Channel).AddPermissionOverwriteAsync(Context.User, new OverwritePermissions(
                    addReactions: PermValue.Allow,
                    attachFiles: PermValue.Allow,
                    embedLinks: PermValue.Allow,
                    readMessageHistory: PermValue.Allow,
                    sendMessages: PermValue.Allow,
                    viewChannel: PermValue.Allow,
                    useApplicationCommands: PermValue.Allow
                ));
                await ticketsRepository.AddTicketClaimedUserIdAsync(Context.Guild.Id, Context.Channel.Id, Context.User.Id);
                await ticketsRepository.SaveAsync();
                await Context.Channel.SendMessageAsync(embed: new EmbedBuilder() {
                    Title = "Ticket Claimed",
                    Description = $"{Context.User.Username} has claimed this ticket.",
                    Color = Color.Green
                }.Build());
                await RespondAsync("You have claimed this ticket.", ephemeral: true);

            }

            [SlashCommand("unclaim", "Unclaims a ticket")]
            public async Task UnclaimTicket() {
                var ticket = await ticketsRepository.GetTicketsAsync(Context.Guild.Id, Context.Channel.Id);
                if (ticket.introMessageId == null ||
                    await Context.Channel.GetMessageAsync((ulong)ticket.introMessageId) == null) {
                    await RespondAsync("You are not in a ticket channel.", ephemeral: true);
                    return;
                }
                if (!ticket.claimedUserId.Contains((long)Context.User.Id)) {
                    await RespondAsync("You have not claimed this ticket.", ephemeral: true);
                    return;
                }
                await ((SocketGuildChannel)Context.Channel).RemovePermissionOverwriteAsync(Context.User);
                await ticketsRepository.RemoveTicketClaimedUserIdAsync(Context.Guild.Id, Context.Channel.Id, Context.User.Id);
                await ticketsRepository.SaveAsync();
                await Context.Channel.SendMessageAsync(embed: new EmbedBuilder() {
                    Title = "Ticket Unclaimed",
                    Description = $"{Context.User.Username} has unclaimed this ticket.",
                    Color = Color.Green
                }.Build());
                await RespondAsync("You have unclaimed this ticket.", ephemeral: true);
            }

            [SlashCommand("adduser", "Adds a user to a ticket")]
            public async Task AddUserToTicket(IUser user) {
                var ticket = await ticketsRepository.GetTicketsAsync(Context.Guild.Id, Context.Channel.Id);
                if (ticket.introMessageId == null
                    || await Context.Channel.GetMessageAsync((ulong)ticket.introMessageId) == null) {
                    await RespondAsync("You are not in a ticket channel.", ephemeral: true);
                    return;
                }
                if (!(ticket.userIds.Contains((long)Context.User.Id) || ticket.claimedUserId.Contains((long)Context.User.Id))) {
                    await RespondAsync("You are not a member of this ticket.", ephemeral: true);
                    return;
                }
                if (ticket.userIds.Contains((long)user.Id) || ticket.claimedUserId.Contains((long)user.Id)) {
                    await RespondAsync("This user is already a member of this ticket.", ephemeral: true);
                    return;
                }
                await ticketsRepository.AddTicketUserIdAsync(Context.Guild.Id, Context.Channel.Id, user.Id);
                await ticketsRepository.SaveAsync();
                await ((SocketGuildChannel)Context.Channel).AddPermissionOverwriteAsync(user, new OverwritePermissions(
                    addReactions: PermValue.Allow,
                    attachFiles: PermValue.Allow,
                    embedLinks: PermValue.Allow,
                    readMessageHistory: PermValue.Allow,
                    sendMessages: PermValue.Allow,
                    viewChannel: PermValue.Allow,
                    useApplicationCommands: PermValue.Allow
                ));
                await Context.Channel.SendMessageAsync(embed: new EmbedBuilder() {
                    Title = "User Added",
                    Description = $"{user.Username} has been added to this ticket.",
                    Color = Color.Green
                }.Build());
                await RespondAsync("User added.", ephemeral: true);
            }

            [SlashCommand("removeuser", "Removes a user from a ticket")]
            public async Task RemoveUserFromTicket(IUser user) {
                var ticket = await ticketsRepository.GetTicketsAsync(Context.Guild.Id, Context.Channel.Id);
                if (ticket.introMessageId == null
                    || await Context.Channel.GetMessageAsync((ulong)ticket.introMessageId) == null) {
                    await RespondAsync("You are not in a ticket channel.", ephemeral: true);
                    return;
                }
                if (!(ticket.userIds.Contains((long)Context.User.Id) || ticket.claimedUserId.Contains((long)Context.User.Id))) {
                    await RespondAsync("You are not a member of this ticket.", ephemeral: true);
                    return;
                }
                if (!(ticket.userIds.Contains((long)user.Id) || ticket.claimedUserId.Contains((long)user.Id))) {
                    await RespondAsync("This user is not a member of this ticket.", ephemeral: true);
                    return;
                }
                await ticketsRepository.RemoveTicketClaimedUserIdAsync(Context.Guild.Id, Context.Channel.Id, user.Id);
                await ticketsRepository.RemoveTicketUserIdAsync(Context.Guild.Id, Context.Channel.Id, user.Id);
                await ticketsRepository.SaveAsync();
                await ((SocketGuildChannel)Context.Channel).RemovePermissionOverwriteAsync(user);
                await Context.Channel.SendMessageAsync(embed: new EmbedBuilder() {
                    Title = "User Removed",
                    Description = $"{user.Username} has been removed from this ticket.",
                    Color = Color.Green
                }.Build());
                await RespondAsync("User removed.", ephemeral: true);
            }

            [SlashCommand("leave", "Leaves a ticket")]
            public async Task LeaveTicket() {
                var ticket = await ticketsRepository.GetTicketsAsync(Context.Guild.Id, Context.Channel.Id);
                if (ticket.introMessageId == null
                    || await Context.Channel.GetMessageAsync((ulong)ticket.introMessageId) == null) {
                    await RespondAsync("You are not in a ticket channel.", ephemeral: true);
                    return;
                }
                if (!(ticket.userIds.Contains((long)Context.User.Id) || ticket.claimedUserId.Contains((long)Context.User.Id))) {
                    await RespondAsync("You are not a member of this ticket.", ephemeral: true);
                    return;
                }
                await ticketsRepository.RemoveTicketClaimedUserIdAsync(Context.Guild.Id, Context.Channel.Id, Context.User.Id);
                await ticketsRepository.RemoveTicketUserIdAsync(Context.Guild.Id, Context.Channel.Id, Context.User.Id);
                await ticketsRepository.SaveAsync();
                await Context.Channel.SendMessageAsync(embed: new EmbedBuilder() {
                    Title = "User Removed",
                    Description = $"{Context.User.Username} has left this ticket.",
                    Color = Color.Green
                }.Build());
                await RespondAsync("User removed.", ephemeral: true);
            }

            public async Task OnButtonExecutedEvent(SocketMessageComponent messageComponent) {
                SocketUserMessage message = messageComponent.Message;
                SocketGuildChannel channel = message.Channel as SocketGuildChannel ?? throw new ArgumentException("Channel is not a guild channel");
                SocketGuild guild = channel.Guild;
                SocketGuildUser user = (SocketGuildUser)messageComponent.User;
                Tickets ticket = await ticketsRepository.GetTicketsAsync(guild.Id, channel.Id);
                if (ticket.introMessageId == (long)message.Id) {
                    if (messageComponent.Message.Id ==  _closeConfirmId) {
                        if (messageComponent.Data.CustomId == "close-yes") {
                            await messageComponent.RespondAsync("Ticket Closed");
                            await ((SocketGuildChannel)message.Channel).DeleteAsync();
                            await ticketsRepository.RemoveTicketAsync(channel.Guild.Id, channel.Id);
                            await ticketsRepository.SaveAsync();
                        } else if (messageComponent.Data.CustomId == "close-no") {
                            await messageComponent.RespondAsync("You have cancelled closing this ticket.");
                        }
                    }
                    await messageComponent.RespondAsync("This ticket does not exist.", ephemeral: true);
                    return;
                }
                if (messageComponent.Data.CustomId == "close") {
                    await messageComponent.RespondAsync(embed: new EmbedBuilder() {
                        Title = "Are you sure?",
                        Description = $"Are you sure you want to close this ticket?\n{ticket.name}",
                        Color = Color.Red
                    }.Build(), components: new ComponentBuilder()
                        .WithButton("Yes", "close-yes")
                        .WithButton("No", "close-no")
                        .Build());
                    _closeConfirmId = (await messageComponent.GetOriginalResponseAsync()).Id;
                    return;
                } 
                if (messageComponent.Data.CustomId == "claim") {
                    if (!user.GuildPermissions.Administrator) {
                        await messageComponent.RespondAsync("You must be an administrator to claim a ticket.", ephemeral: true);
                        return;
                    }
                    var claimedUsers = await ticketsRepository.GetTicketsAsync(guild.Id, channel.Id);
                    if (claimedUsers.claimedUserId.Contains((long)user.Id)) {
                        await messageComponent.RespondAsync("You have already claimed this ticket.", ephemeral: true);
                        return;
                    }
                    await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(
                        addReactions: PermValue.Allow,
                        attachFiles: PermValue.Allow,
                        embedLinks: PermValue.Allow,
                        readMessageHistory: PermValue.Allow,
                        sendMessages: PermValue.Allow,
                        viewChannel: PermValue.Allow,
                        useApplicationCommands: PermValue.Allow
                    ));
                    await ticketsRepository.AddTicketClaimedUserIdAsync(channel.Guild.Id, channel.Id, user.Id);
                    await ticketsRepository.SaveAsync();
                    await message.Channel.SendMessageAsync(embed: new EmbedBuilder() {
                        Title = "Ticket Claimed",
                        Description = $"{user.Username} has claimed this ticket.",
                        Color = Color.Green
                    }.Build());
                    await messageComponent.RespondAsync("You have claimed this ticket.", ephemeral: true);
                }
            }
        }
    }
}
