using FinderNET.Database.Contexts;
using FinderNET.Database.Models;

namespace FinderNET.Database.Repositories {
    public class PollsRepository : Repository<PollsModel> {
        public PollsRepository(FinderDatabaseContext context) : base(context) { }
        
        public async Task<PollsModel> GetPollsAsync(ulong messageId) {
            return await context.Set<PollsModel>().FindAsync((long)messageId) ?? new PollsModel();
        }

        public async Task AddPollsAsync(ulong messageId, List<string> answers, List<Int64> votersId) {
            var polls = await context.Set<PollsModel>().FindAsync((long)messageId);
            if (polls == null) {
                await context.Set<PollsModel>().AddAsync(new PollsModel {
                    messageId = (long)messageId,
                    answers = answers,
                    votersId = votersId,
                });
                return;
            }
            polls.messageId = (long)messageId;
            polls.answers = answers;
            polls.votersId = votersId;
            context.Set<PollsModel>().Update(polls);
        }

        public async Task RemovePollsAsync(ulong messageId) {
            var polls = await context.Set<PollsModel>().FindAsync((long)messageId);
            if (polls == null) return;
            context.Set<PollsModel>().Remove(polls);
        }

        public async Task<bool> PollExistsAsync(ulong messageId) {
            return await context.Set<PollsModel>().FindAsync((long)messageId) != null;
        }
    }
}