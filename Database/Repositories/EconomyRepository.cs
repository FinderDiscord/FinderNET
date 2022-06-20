using FinderNET.Database.Contexts;
using FinderNET.Database.Models;

namespace FinderNET.Database.Repositories {
    public class EconomyRepository : Repository<Economy> {
        public EconomyRepository(FinderDatabaseContext context) : base(context) { }
        
        public async Task<Economy> GetEconomyAsync(ulong guildId, ulong userId) {
            return await context.Set<Economy>().FindAsync((long)guildId, (long)userId) ?? new Economy();
        }

        public async Task AddEconomyAsync(ulong guildId, ulong userId, int money, int bank) {
            var economy = await context.Set<Economy>().FindAsync((long)guildId, (long)userId);
            if (economy == null) {
                await context.Set<Economy>().AddAsync(new Economy {
                    guildId = (long)guildId,
                    userId = (long)userId,
                    money = money,
                    bank = bank
                });
                return;
            }
            economy.guildId = (long)guildId;
            economy.userId = (long)userId;
            economy.money = economy.money + money;
            economy.bank = economy.bank + bank;
            context.Set<Economy>().Update(economy);
        }

        public async Task SubtractEconomyAsync(ulong guildId, ulong userId, int money, int bank) {
            var economy = await context.Set<Economy>().FindAsync((long)guildId, (long)userId);
            if (economy == null) {
                await context.Set<Economy>().AddAsync(new Economy {
                    guildId = (long)guildId,
                    userId = (long)userId,
                    money = money,
                    bank = bank
                });
                return;
            } else {
                economy.guildId = (long)guildId;
                economy.userId = (long)userId;
                economy.money = economy.money - money;
                economy.bank = economy.bank - bank;
            }
            context.Set<Economy>().Update(economy);
        }

        public async Task RemoveEconomyAsync(ulong guildId, ulong userId) {
            var economy = await context.Set<Economy>().FindAsync((long)guildId, (long)userId);
            if (economy == null) return;
            context.Set<Economy>().Remove(economy);
        }
    }
}