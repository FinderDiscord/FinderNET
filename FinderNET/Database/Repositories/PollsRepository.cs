using FinderNET.Database.Contexts;
using FinderNET.Database.Models;

namespace FinderNET.Database.Repositories {
    public class PollsRepository : Repository<Polls> {
        public PollsRepository(FinderDatabaseContext context) : base(context) { }
        
        public async Task<Polls> GetPollsAsync(ulong messageId) {
            return await context.Set<Polls>().FindAsync((long)messageId) ?? new Polls();
        }

        public async Task AddPollsAsync(ulong messageId, List<string> answers, List<Int64> votersId) {
            var polls = await context.Set<Polls>().FindAsync((long)messageId);
            if (polls == null) {
                await context.Set<Polls>().AddAsync(new Polls {
                    messageId = (long)messageId,
                    answers = answers,
                    votersId = votersId,
                });
                return;
            }
            polls.messageId = (long)messageId;
            polls.answers = answers;
            polls.votersId = votersId;
            context.Set<Polls>().Update(polls);
        }

        public async Task RemovePollsAsync(ulong messageId) {
            var polls = await context.Set<Polls>().FindAsync((long)messageId);
            if (polls == null) return;
            context.Set<Polls>().Remove(polls);
        }

        public async Task<bool> PollExistsAsync(ulong messageId) {
            return await context.Set<Polls>().FindAsync((long)messageId) != null;
        }
    }
}