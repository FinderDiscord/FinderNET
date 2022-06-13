using Discord;
using Discord.Interactions;
using FinderNET.Database.Repositories;

namespace FinderNET.Modules {
    public class EconomyModule : InteractionModuleBase<SocketInteractionContext> {
        private readonly EconomyRepository economyRepository;
        public EconomyModule(EconomyRepository _economyRepository) {
            economyRepository = _economyRepository;
        }
        [SlashCommand("balance", "Checks user's balance.")]
        public async Task Balance(IUser? user = null) {
            if (user == null) {
                user = Context.User;
            }
            var economy = await economyRepository.GetEconomyAsync(Context.Guild.Id, user.Id);
            await RespondAsync(embed: new EmbedBuilder() {
                Title = $"{user.Username}'s balance",
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Money",
                        Value = economy.money.ToString()
                    },
                    new EmbedFieldBuilder() {
                        Name = "Bank",
                        Value = economy.bank.ToString()
                    }
                },
                Color = Color.Green
            }.Build());
        }

        [SlashCommand("deposit", "Deposits money into your bank.")]
        public async Task Deposit(int amount) {
            var economy = await economyRepository.GetEconomyAsync(Context.Guild.Id, Context.User.Id);
            if (economy.money < amount) {
                await RespondAsync(embed: new EmbedBuilder() {
                    Title = "Error",
                    Description = "You don't have enough money.",
                    Color = Color.Red
                }.Build());
                return;
            }
            await economyRepository.AddEconomyAsync(Context.Guild.Id, Context.User.Id, -amount, amount);
            await economyRepository.SaveAsync();
            await RespondAsync(embed: new EmbedBuilder() {
                Title = "Deposit",
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "You deposited",
                        Value = amount.ToString()
                    }
                },
                Color = Color.Green
            }.Build());
        }

        [SlashCommand("withdraw", "Withdraws money from your bank.")]
        public async Task Withdraw(int amount) {
            var economy = await economyRepository.GetEconomyAsync(Context.Guild.Id, Context.User.Id);
            if (economy.bank < amount) {
                await RespondAsync(embed: new EmbedBuilder() {
                    Title = "Error",
                    Description = "You don't have enough money in your bank.",
                    Color = Color.Red
                }.Build());
                return;
            }
            await economyRepository.AddEconomyAsync(Context.Guild.Id, Context.User.Id, amount, -amount);
            await economyRepository.SaveAsync();
            await RespondAsync(embed: new EmbedBuilder() {
                Title = "Withdraw",
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "You withdrew",
                        Value = amount.ToString()
                    }
                },
                Color = Color.Green
            }.Build());
        }

        [SlashCommand("pay", "Pays money to another user.")]
        public async Task Pay(IUser user, int amount) {
            var economy = await economyRepository.GetEconomyAsync(Context.Guild.Id, Context.User.Id);
            if (economy.money < amount) {
                await RespondAsync(embed: new EmbedBuilder() {
                    Title = "Error",
                    Description = "You don't have enough money.",
                    Color = Color.Red
                }.Build());
                return;
            }
            await economyRepository.AddEconomyAsync(Context.Guild.Id, Context.User.Id, -amount, 0);
            await economyRepository.AddEconomyAsync(Context.Guild.Id, user.Id, amount, 0);
            await economyRepository.SaveAsync();
            await RespondAsync(embed: new EmbedBuilder() {
                Title = "Pay",
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Payee",
                        Value = user.Username
                    },
                    new EmbedFieldBuilder() {
                        Name = "Amount",
                        Value = amount.ToString()
                    }
                },
                Color = Color.Green
            }.Build());
        }

        [SlashCommand("transfer", "Transfers money to another user from your bank.")]
        public async Task Transfer(IUser user, int amount) {
            var economy = await economyRepository.GetEconomyAsync(Context.Guild.Id, Context.User.Id);
            if (economy.bank < amount) {
                await RespondAsync(embed: new EmbedBuilder() {
                    Title = "Error",
                    Description = "You don't have enough money in your bank.",
                    Color = Color.Red
                }.Build());
                return;
            }
            await economyRepository.AddEconomyAsync(Context.Guild.Id, Context.User.Id, 0, -amount);
            await economyRepository.AddEconomyAsync(Context.Guild.Id, user.Id, 0, amount);
            await economyRepository.SaveAsync();
            await RespondAsync(embed: new EmbedBuilder() {
                Title = "Transfer",
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Payee",
                        Value = user.Username
                    },
                    new EmbedFieldBuilder() {
                        Name = "Amount",
                        Value = amount.ToString()
                    }
                },
                Color = Color.Green
            }.Build());
        }

        [SlashCommand("setbalance", "Sets the balance of a user.")]
        public async Task SetBalance(IUser user, int amount) {
            await economyRepository.AddEconomyAsync(Context.Guild.Id, user.Id, amount, 0);
            await economyRepository.SaveAsync();
            await RespondAsync(embed: new EmbedBuilder() {
                Title = "Set Balance",
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "User",
                        Value = user.Username
                    },
                    new EmbedFieldBuilder() {
                        Name = "Amount",
                        Value = amount.ToString()
                    }
                },
                Color = Color.Green
            }.Build());
        }
    }
}
