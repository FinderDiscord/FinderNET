using FinderNET.Database.Contexts;
using FinderNET.Database.Models;

namespace FinderNET.Database.Repositories {
    public class EconomyRepository : Repository<EconomyModel> {
        public EconomyRepository(FinderDatabaseContext context) : base(context) { }
        
        public async Task<EconomyModel> GetEconomyAsync(ulong guildId, ulong userId) {
            return await context.Set<EconomyModel>().FindAsync((long)guildId, (long)userId) ?? new EconomyModel();
        }

        public async Task AddEconomyAsync(ulong guildId, ulong userId, int money, int bank) {
            var economy = await context.Set<EconomyModel>().FindAsync((long)guildId, (long)userId);
            if (economy == null) {
                await context.Set<EconomyModel>().AddAsync(new EconomyModel {
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
            context.Set<EconomyModel>().Update(economy);
        }

        public async Task SubtractEconomyAsync(ulong guildId, ulong userId, int money, int bank) {
            var economy = await context.Set<EconomyModel>().FindAsync((long)guildId, (long)userId);
            if (economy == null) {
                await context.Set<EconomyModel>().AddAsync(new EconomyModel {
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
            context.Set<EconomyModel>().Update(economy);
        }

        public async Task RemoveEconomyAsync(ulong guildId, ulong userId) {
            var economy = await context.Set<EconomyModel>().FindAsync((long)guildId, (long)userId);
            if (economy == null) return;
            context.Set<EconomyModel>().Remove(economy);
        }
    }
}