using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FinderNET.Database;

namespace FinderNET.Modules {
    public class EconomyModule : ModuleBase {
        public EconomyModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
        
        [SlashCommand("balance", "Checks user's balance.")]
        public async Task Balance(IUser? user = null) {
            if (user == null) {
                user = Context.User;
            }
            var balance = await dataAccessLayer.GetBalance((long)Context.Guild.Id, (long)user.Id);
            var bank = await dataAccessLayer.GetBank((long)Context.Guild.Id, (long)user.Id);
            await RespondAsync(embed: new EmbedBuilder() {
                Title = $"{user.Username}'s balance",
                Fields = new List<EmbedFieldBuilder> {
                    new EmbedFieldBuilder() {
                        Name = "Money",
                        Value = balance.ToString()
                    },
                    new EmbedFieldBuilder() {
                        Name = "Bank",
                        Value = bank.ToString()
                    }
                },
                Color = Color.Green
            }.Build());
        }

        [SlashCommand("deposit", "Deposits money into your bank.")]
        public async Task Deposit(int amount) {
            var balance = await dataAccessLayer.GetBalance((long)Context.Guild.Id, (long)Context.User.Id);
            if (balance < amount) {
                await RespondAsync(embed: new EmbedBuilder() {
                    Title = "Error",
                    Description = "You don't have enough money.",
                    Color = Color.Red
                }.Build());
                return;
            }
            await dataAccessLayer.AddBank((long)Context.Guild.Id, (long)Context.User.Id, amount);
            await dataAccessLayer.RemoveBalance((long)Context.Guild.Id, (long)Context.User.Id, amount);
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
            var bank = await dataAccessLayer.GetBank((long)Context.Guild.Id, (long)Context.User.Id);
            if (bank < amount) {
                await RespondAsync(embed: new EmbedBuilder() {
                    Title = "Error",
                    Description = "You don't have enough money.",
                    Color = Color.Red
                }.Build());
                return;
            }
            await dataAccessLayer.AddBalance((long)Context.Guild.Id, (long)Context.User.Id, amount);
            await dataAccessLayer.RemoveBank((long)Context.Guild.Id, (long)Context.User.Id, amount);
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
            var balance = await dataAccessLayer.GetBalance((long)Context.Guild.Id, (long)Context.User.Id);
            if (balance < amount) {
                await RespondAsync(embed: new EmbedBuilder() {
                    Title = "Error",
                    Description = "You don't have enough money.",
                    Color = Color.Red
                }.Build());
                return;
            }
            await dataAccessLayer.AddBalance((long)Context.Guild.Id, (long)user.Id, amount);
            await dataAccessLayer.RemoveBalance((long)Context.Guild.Id, (long)Context.User.Id, amount);
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
            var bank = await dataAccessLayer.GetBank((long)Context.Guild.Id, (long)Context.User.Id);
            if (bank < amount) {
                await RespondAsync(embed: new EmbedBuilder() {
                    Title = "Error",
                    Description = "You don't have enough money in your bank.",
                    Color = Color.Red
                }.Build());
                return;
            }
            await dataAccessLayer.AddBank((long)Context.Guild.Id, (long)user.Id, amount);
            await dataAccessLayer.RemoveBank((long)Context.Guild.Id, (long)Context.User.Id, amount);
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
            await dataAccessLayer.SetBalance((long)Context.Guild.Id, (long)user.Id, amount);
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
