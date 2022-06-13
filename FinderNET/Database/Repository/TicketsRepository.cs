using FinderNET.Database.Models;
using FinderNET.Database.Contexts;

namespace FinderNET.Database.Repositories {
    public class TicketsRepository : Repository<Tickets> {
        public TicketsRepository(FinderDatabaseContext context) : base(context) { }

        public async Task<Tickets> GetTicketsAsync(ulong ticketId) {
            return await context.Set<Tickets>().FindAsync((long)ticketId) ?? new Tickets();
        }

        public async Task AddTicketsAsync(ulong ticketId, ulong guildId, ulong supportChannelId, ulong introMessageId, List<long?> userIds, string name, List<long> claimedUserId) {
            var tickets = await context.Set<Tickets>().FindAsync((long)ticketId);
            if (tickets == null) {
                await context.Set<Tickets>().AddAsync(new Tickets {
                    ticketId = (long)ticketId,
                    guildId = (long)guildId,
                    supportChannelId = (long)supportChannelId,
                    introMessageId = (long)introMessageId,
                    userIds = userIds, // TODO: change to ulong
                    name = name,
                    claimedUserId = claimedUserId // TODO: Change to ulong
                });
                return;
            }
            tickets.ticketId = (long)ticketId;
            tickets.guildId = (long)guildId;
            tickets.supportChannelId = (long)supportChannelId;
            tickets.introMessageId = (long)introMessageId;
            tickets.userIds = userIds;
            tickets.name = name;
            tickets.claimedUserId = claimedUserId;
            context.Set<Tickets>().Update(tickets);
        }

        public async Task RemoveTicketsAsync(ulong ticketId) {
            var tickets = await context.Set<Tickets>().FindAsync((long)ticketId);
            if (tickets == null) return;
            context.Set<Tickets>().Remove(tickets);
        }

        public bool TicketsExists(ulong ticketId) {
            return context.Set<Tickets>().Find((long)ticketId) != null;
        }
    }
}