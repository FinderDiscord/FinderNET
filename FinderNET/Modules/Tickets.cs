// using Discord;
// using Discord.Interactions;
// using Discord.WebSocket;
// using FinderNET.Database.Repositories;

// namespace FinderNET.Modules {
//     public class TicketingModule {
//         [Group("tickets", "Command For Managing Tickets")]
//         public class TicketsModule : InteractionModuleBase<InteractionContext> {
//             ulong _closeConfirmId;
//             private readonly TicketsRepository ticketsRepository;
//             public TicketsModule(TicketsRepository _ticketsRepository) {
//                 ticketsRepository = _ticketsRepository;
//             }
//             [SlashCommand("create", "Creates a ticket")]
//             public async Task CreateTicket(string name) {
//                 if (name == null) {
//                     await RespondAsync("Please specify a name for the ticket.");
//                     return;
//                 }
//                 if (name.Length > 32) {
//                     await RespondAsync("The name of the ticket is too long.");
//                     return;
//                 }
//                 var supportChannel = await Context.Guild.CreateTextChannelAsync($"ticket-");
//                 await supportChannel.AddPermissionOverwriteAsync(Context.User, new OverwritePermissions(
//                     addReactions: PermValue.Allow,
//                     attachFiles: PermValue.Allow,
//                     embedLinks: PermValue.Allow,
//                     readMessageHistory: PermValue.Allow,
//                     sendMessages: PermValue.Allow,
//                     viewChannel: PermValue.Allow,
//                     useApplicationCommands: PermValue.Allow
//                 ));
//                 await supportChannel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(
//                     readMessageHistory: PermValue.Deny,
//                     sendMessages: PermValue.Deny,
//                     viewChannel: PermValue.Deny
//                 ));
//                 var components = new ComponentBuilder()
//                     .WithButton("Close Ticket", "close")
//                     .WithButton("Claim Ticket", "claim")
//                     .Build();
//                 var message = await supportChannel.SendMessageAsync(embed: new EmbedBuilder() {
//                     Title = "Ticket",
//                     Fields = new List<EmbedFieldBuilder>() {
//                         new EmbedFieldBuilder() {
//                             Name = name,
//                             Value = $"Channel made by {Context.User.Username}"
//                         }
//                     },
//                     Color = Color.Green
//                 }.Build(), components: components);
//                 var tickets = await dataAccessLayer.GetTickets((long)Context.Guild.Id);
//                 List<long> ticketIds = new List<long>();
//                 foreach (var ticket in tickets) {
//                     ticketIds.Add(ticket.ticketId);
//                 }
//                 int _i = 0;
//                 for (int i = 0; i < ticketIds.Count; i++) {
//                     if (ticketIds[i] != i + 1) {
//                         await dataAccessLayer.AddTicket(i + 1, (long)Context.Guild.Id, (long)supportChannel.Id, (long)Context.User.Id, name, (long)message.Id);
//                         _i = i;
//                         break;
//                     }
//                     _i = i;
//                 }
//                 if (await dataAccessLayer.GetTicket(_i) == null) {
//                     await dataAccessLayer.AddTicket(_i + 1, (long)Context.Guild.Id, (long)supportChannel.Id, (long)Context.User.Id, name, (long)message.Id);
//                 }
//                 await RespondAsync(embed: new EmbedBuilder() {
//                     Title = "Ticket Created",
//                     Description = $"Opened a new ticket: {supportChannel.Mention}",
//                     Color = Color.Green
//                 }.Build());
//             }

//             [SlashCommand("close", "Closes a ticket")]
//             public async Task CloseTicket() {
//                 var tickets = await dataAccessLayer.GetTickets((long)Context.Guild.Id);
//                 var ticket = tickets.Find(x => x.supportChannelId == (long)Context.Channel.Id);
//                 if (ticket == null) {
//                     await RespondAsync("You are not in a ticket channel.", ephemeral: true);
//                     return;
//                 }
//                 if (!ticket.userIds.Contains((long)Context.User.Id) || !ticket.claimedUserId.Contains((long)Context.User.Id)) {
//                     await RespondAsync("You are not the owner of this ticket.", ephemeral: true);
//                     return;
//                 }
//                 await RespondAsync("Ticket Closed");
//                 await ((SocketGuildChannel)Context.Channel).DeleteAsync();
//                 await dataAccessLayer.RemoveTicket((long)ticket.ticketId);
//             }

//             [SlashCommand("claim", "Claims a ticket")]
//             public async Task ClaimTicket() {
//                 if (!((SocketGuildUser)Context.User).GuildPermissions.Administrator) {
//                     await RespondAsync("You do not have permission to use this command.", ephemeral: true);
//                     return;
//                 }
//                 var tickets = await dataAccessLayer.GetTickets((long)Context.Guild.Id);
//                 var ticket = tickets.Find(x => x.supportChannelId == (long)Context.Channel.Id);
//                 if (ticket == null) {
//                     await RespondAsync("You are not in a ticket channel.", ephemeral: true);
//                     return;
//                 }
//                 if (ticket.claimedUserId.Contains((long)Context.User.Id)) {
//                     await RespondAsync("You have already claimed this ticket.", ephemeral: true);
//                     return;
//                 }
//                 await ((SocketGuildChannel)Context.Channel).AddPermissionOverwriteAsync(Context.User, new OverwritePermissions(
//                     addReactions: PermValue.Allow,
//                     attachFiles: PermValue.Allow,
//                     embedLinks: PermValue.Allow,
//                     readMessageHistory: PermValue.Allow,
//                     sendMessages: PermValue.Allow,
//                     viewChannel: PermValue.Allow,
//                     useApplicationCommands: PermValue.Allow
//                 ));
//                 await dataAccessLayer.AddTicketUser((long)ticket.ticketId, (long)Context.User.Id);
//                 await dataAccessLayer.AddClaimedUserId((long)ticket.ticketId, (long)Context.User.Id);
//                 await Context.Channel.SendMessageAsync(embed: new EmbedBuilder() {
//                     Title = "Ticket Claimed",
//                     Description = $"{Context.User.Username} has claimed this ticket.",
//                     Color = Color.Green
//                 }.Build());
//                 await RespondAsync("You have claimed this ticket.", ephemeral: true);

//             }

//             [SlashCommand("unclaim", "Unclaims a ticket")]
//             public async Task UnclaimTicket() {
//                 var tickets = await dataAccessLayer.GetTickets((long)Context.Guild.Id);
//                 var ticket = tickets.Find(x => x.supportChannelId == (long)Context.Channel.Id);
//                 if (ticket == null) {
//                     await RespondAsync("You are not in a ticket channel.", ephemeral: true);
//                     return;
//                 }
//                 if (!ticket.claimedUserId.Contains((long)Context.User.Id)) {
//                     await RespondAsync("You have not claimed this ticket.", ephemeral: true);
//                     return;
//                 }
//                 await ((SocketGuildChannel)Context.Channel).RemovePermissionOverwriteAsync(Context.User);
//                 await dataAccessLayer.RemoveClaimedUserId((long)ticket.ticketId, (long)Context.User.Id);
//                 await Context.Channel.SendMessageAsync(embed: new EmbedBuilder() {
//                     Title = "Ticket Unclaimed",
//                     Description = $"{Context.User.Username} has unclaimed this ticket.",
//                     Color = Color.Green
//                 }.Build());
//                 await RespondAsync("You have unclaimed this ticket.", ephemeral: true);
//             }

//             [SlashCommand("adduser", "Adds a user to a ticket")]
//             public async Task AddUserToTicket(IUser user) {
//                 var tickets = await dataAccessLayer.GetTickets((long)Context.Guild.Id);
//                 var ticket = tickets.Find(x => x.supportChannelId == (long)Context.Channel.Id);
//                 if (ticket == null) {
//                     await RespondAsync("You are not in a ticket channel.", ephemeral: true);
//                     return;
//                 }
//                 if (!(ticket.userIds.Contains((long)Context.User.Id) || ticket.claimedUserId.Contains((long)Context.User.Id))) {
//                     await RespondAsync("You are not a member of this ticket.", ephemeral: true);
//                     return;
//                 }
//                 if (ticket.userIds.Contains((long)user.Id) || ticket.claimedUserId.Contains((long)user.Id)) {
//                     await RespondAsync("This user is already a member of this ticket.", ephemeral: true);
//                     return;
//                 }
//                 await dataAccessLayer.AddTicketUser((long)ticket.ticketId, (long)user.Id);
//                 await ((SocketGuildChannel)Context.Channel).AddPermissionOverwriteAsync(user, new OverwritePermissions(
//                     addReactions: PermValue.Allow,
//                     attachFiles: PermValue.Allow,
//                     embedLinks: PermValue.Allow,
//                     readMessageHistory: PermValue.Allow,
//                     sendMessages: PermValue.Allow,
//                     viewChannel: PermValue.Allow,
//                     useApplicationCommands: PermValue.Allow
//                 ));
//                 await Context.Channel.SendMessageAsync(embed: new EmbedBuilder() {
//                     Title = "User Added",
//                     Description = $"{user.Username} has been added to this ticket.",
//                     Color = Color.Green
//                 }.Build());
//                 await RespondAsync("User added.", ephemeral: true);
//             }

//             [SlashCommand("removeuser", "Removes a user from a ticket")]
//             public async Task RemoveUserFromTicket(IUser user) {
//                 var tickets = await dataAccessLayer.GetTickets((long)Context.Guild.Id);
//                 var ticket = tickets.Find(x => x.supportChannelId == (long)Context.Channel.Id);
//                 if (ticket == null) {
//                     await RespondAsync("You are not in a ticket channel.", ephemeral: true);
//                     return;
//                 }
//                 if (!(ticket.userIds.Contains((long)Context.User.Id) || ticket.claimedUserId.Contains((long)Context.User.Id))) {
//                     await RespondAsync("You are not a member of this ticket.", ephemeral: true);
//                     return;
//                 }
//                 if (!(ticket.userIds.Contains((long)user.Id) || ticket.claimedUserId.Contains((long)user.Id))) {
//                     await RespondAsync("This user is not a member of this ticket.", ephemeral: true);
//                     return;
//                 }
//                 await dataAccessLayer.RemoveTicketUser((long)ticket.ticketId, (long)user.Id);
//                 await ((SocketGuildChannel)Context.Channel).RemovePermissionOverwriteAsync(user);
//                 await Context.Channel.SendMessageAsync(embed: new EmbedBuilder() {
//                     Title = "User Removed",
//                     Description = $"{user.Username} has been removed from this ticket.",
//                     Color = Color.Green
//                 }.Build());
//                 await RespondAsync("User removed.", ephemeral: true);
//             }

//             [SlashCommand("leave", "Leaves a ticket")]
//             public async Task LeaveTicket() {
//                 var tickets = await dataAccessLayer.GetTickets((long)Context.Guild.Id);
//                 var ticket = tickets.Find(x => x.supportChannelId == (long)Context.Channel.Id);
//                 if (ticket == null) {
//                     await RespondAsync("You are not in a ticket channel.", ephemeral: true);
//                     return;
//                 }
//                 if (!(ticket.userIds.Contains((long)Context.User.Id) || ticket.claimedUserId.Contains((long)Context.User.Id))) {
//                     await RespondAsync("You are not a member of this ticket.", ephemeral: true);
//                     return;
//                 }
//                 await dataAccessLayer.RemoveTicketUser((long)ticket.ticketId, (long)Context.User.Id);
//                 await dataAccessLayer.RemoveClaimedUserId((long)ticket.ticketId, (long)Context.User.Id);
//                 await Context.Channel.SendMessageAsync(embed: new EmbedBuilder() {
//                     Title = "User Removed",
//                     Description = $"{Context.User.Username} has left this ticket.",
//                     Color = Color.Green
//                 }.Build());
//                 await RespondAsync("User removed.", ephemeral: true);
//             }

//             public async Task OnButtonExecutedEvent(SocketMessageComponent messageComponent) {
//                 SocketUserMessage message = messageComponent.Message;
//                 SocketGuildChannel channel = message.Channel as SocketGuildChannel ?? throw new ArgumentException("Channel is not a guild channel");
//                 SocketGuild guild = channel.Guild;
//                 SocketGuildUser user = (SocketGuildUser)messageComponent.User;
//                 List<Database.Tickets> tickets = await dataAccessLayer.GetTickets((long)guild.Id);
//                 Database.Tickets? ticket = tickets.Find(x => x.introMessageId == (long)message.Id && x.supportChannelId == (long)message.Channel.Id);
//                 if (ticket == null) {
//                     if (messageComponent.Message.Id ==  _closeConfirmId) {
//                         if (messageComponent.Data.CustomId == "close-yes") {
//                             await messageComponent.RespondAsync("Ticket Closed");
//                             await ((SocketGuildChannel)message.Channel).DeleteAsync();
//                             await dataAccessLayer.RemoveTicket((long)ticket.ticketId);
//                         } else if (messageComponent.Data.CustomId == "close-no") {
//                             await messageComponent.RespondAsync("You have cancelled closing this ticket.");
//                         }
//                     }
//                     await messageComponent.RespondAsync("This ticket does not exist.", ephemeral: true);
//                     return;
//                 }
//                 if (messageComponent.Data.CustomId == "close") {
//                     await messageComponent.RespondAsync(embed: new EmbedBuilder() {
//                         Title = "Are you sure?",
//                         Description = $"Are you sure you want to close this ticket?\n{ticket.name}",
//                         Color = Color.Red
//                     }.Build(), components: new ComponentBuilder()
//                         .WithButton("Yes", "close-yes")
//                         .WithButton("No", "close-no")
//                         .Build());
//                     _closeConfirmId = (await messageComponent.GetOriginalResponseAsync()).Id;
//                     return;
//                 } else if (messageComponent.Data.CustomId == "claim") {
//                     if (!user.GuildPermissions.Administrator) {
//                         await messageComponent.RespondAsync("You must be an administrator to claim a ticket.", ephemeral: true);
//                         return;
//                     }
//                     var claimedUsers = await dataAccessLayer.GetClaimedUserId((long)ticket.ticketId);
//                     if (claimedUsers.Contains((long)user.Id)) {
//                         await messageComponent.RespondAsync("You have already claimed this ticket.", ephemeral: true);
//                         return;
//                     }
//                     await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(
//                         addReactions: PermValue.Allow,
//                         attachFiles: PermValue.Allow,
//                         embedLinks: PermValue.Allow,
//                         readMessageHistory: PermValue.Allow,
//                         sendMessages: PermValue.Allow,
//                         viewChannel: PermValue.Allow,
//                         useApplicationCommands: PermValue.Allow
//                     ));
//                     await dataAccessLayer.AddTicketUser((long)ticket.ticketId, (long)user.Id);
//                     await dataAccessLayer.AddClaimedUserId((long)ticket.ticketId, (long)user.Id);
//                     await message.Channel.SendMessageAsync(embed: new EmbedBuilder() {
//                         Title = "Ticket Claimed",
//                         Description = $"{user.Username} has claimed this ticket.",
//                         Color = Color.Green
//                     }.Build());
//                     await messageComponent.RespondAsync("You have claimed this ticket.", ephemeral: true);
//                 }
//             }
//         }
//     }
// }
