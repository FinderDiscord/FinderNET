using FinderNET.Database.Models;
using FinderNET.Database.Contexts;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;

namespace FinderNET.Database.Repositories {
    public class TicketsRepository : Repository<Tickets> {
        public TicketsRepository(FinderDatabaseContext context) : base(context) { }
        public async Task<Tickets> GetTicketsAsync(ulong guildId, ulong supportChannelId) {
            return await context.Set<Tickets>().FindAsync((long)guildId, (long)supportChannelId) ?? new Tickets();
        }

        public async Task AddTicketAsync(ulong guildId, ulong supportChannelId, ulong? introMessageId, List<Int64?> userIds, string? name, List<Int64> claimedUserIds) {
            var tickets = await context.Set<Tickets>().FindAsync((long)guildId, (long)supportChannelId);
            if (tickets == null) {
                await context.Set<Tickets>().AddAsync(new Tickets() {
                    guildId = (long)guildId,
                    supportChannelId = (long)supportChannelId,
                    introMessageId = (long?)introMessageId ?? null,
                    name = name,
                    claimedUserId = claimedUserIds,
                    userIds = userIds
                });
                return;
            }
            tickets.guildId = (long)guildId;
            tickets.supportChannelId = (long)supportChannelId;
            tickets.introMessageId = (long)introMessageId;
            tickets.userIds = userIds;
            tickets.name = name;
            tickets.claimedUserId = claimedUserIds;
        }

        public async Task RemoveTicketAsync(ulong guildId, ulong supportChannelId) {
            var tickets = await context.Set<Tickets>().FindAsync((long)guildId, (long)supportChannelId);
            if (tickets == null) return;
            context.Set<Tickets>().Remove(tickets);
        }
        
        public async Task AddTicketUserIdAsync(ulong guildId, ulong supportChannelId, ulong userId) {
            var tickets = await context.Set<Tickets>().FindAsync((long)guildId, (long)supportChannelId);
            if (tickets == null) return;
            tickets.userIds.Add((long)userId);
        }
        
        public async Task RemoveTicketUserIdAsync(ulong guildId, ulong supportChannelId, ulong userId) {
            var tickets = await context.Set<Tickets>().FindAsync((long)guildId, (long)supportChannelId);
            if (tickets == null) return;
            tickets.userIds.Remove((long)userId);
        }
        
        public async Task AddTicketClaimedUserIdAsync(ulong guildId, ulong supportChannelId, ulong userId) {
            var tickets = await context.Set<Tickets>().FindAsync((long)guildId, (long)supportChannelId);
            if (tickets == null) return;
            tickets.claimedUserId.Add((long)userId);
        }
        
        public async Task RemoveTicketClaimedUserIdAsync(ulong guildId, ulong supportChannelId, ulong userId) {
            var tickets = await context.Set<Tickets>().FindAsync((long)guildId, (long)supportChannelId);
            if (tickets == null) return;
            tickets.claimedUserId.Remove((long)userId);
        }
    }
}