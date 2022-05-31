using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Rest;
using FinderNET.Database;

namespace FinderNET.Modules {
   public class PollModule : ModuleBase {
      public PollModule(DataAccessLayer dataAccessLayer) : base(dataAccessLayer) { }
      // public List<Poll> polls = new List<Poll>();
      [SlashCommand("poll", "Create a poll for users to vote on.")]
      public async Task PollCommand(string question, string? answer1 = null, string? answer2 = null, string? answer3 = null, string? answer4 = null, string? answer5 = null, string? answer6 = null, string? answer7 = null, string? answer8 = null, string? answer9 = null, string? answer10 = null,
      string? answer11 = null, string? answer12 = null, string? answer13 = null, string? answer14 = null, string? answer15 = null, string? answer16 = null, string? answer17 = null, string? answer18 = null, string? answer19 = null, string? answer20 = null, string? answer21 = null, string? answer22 = null, 
      string? answer23 = null, string? answer24 = null) {
         ComponentBuilder builder = new ComponentBuilder();
         if (question == null) {
            await ReplyAsync("You must provide a question.");
            return;
         }
         var embed = new EmbedBuilder() {
            Title = question,
            Description = $"Poll created by {Context.User.Username}",
            Color = Color.Blue,
            Footer = new EmbedFooterBuilder() {
               Text = "FinderBot"
            }
         };
         if (answer1 != null) {
            embed.AddField(answer1, 0, true);
            builder.WithButton($"{answer1}", "0");
         } else {
            embed.AddField("Yes", 0, true);
            builder.WithButton("Yes", "0");
         }
         if (answer2 != null) {
            embed.AddField(answer2, 0, true);
            builder.WithButton($"{answer2}", "1");
         } else {
            embed.AddField("No", 0, true);
            builder.WithButton("No", "1");
         }
         if (answer3 != null) {
            embed.AddField(answer3, 0, true);
            builder.WithButton($"{answer3}", "2");
         }
         if (answer4 != null) {
            embed.AddField(answer4, 0, true);
            builder.WithButton($"{answer4}", "3");
         }
         if (answer5 != null) {
            embed.AddField(answer5, 0, true);
            builder.WithButton($"{answer5}", "4");
         }
         if (answer6 != null) {
            embed.AddField(answer6, 0, true);
            builder.WithButton($"{answer6}", "5");
         }
         if (answer7 != null) {
            embed.AddField(answer7, 0, true);
            builder.WithButton($"{answer7}", "6");
         }
         if (answer8 != null) {
            embed.AddField(answer8, 0, true);
            builder.WithButton($"{answer8}", "7");
         }
         if (answer9 != null) {
            embed.AddField(answer9, 0, true);
            builder.WithButton($"{answer9}", "8");
         }
         if (answer10 != null) {
            embed.AddField(answer10, 0, true);
            builder.WithButton($"{answer10}", "9");
         }
         if (answer11 != null) {
            embed.AddField(answer11, 0, true);
            builder.WithButton($"{answer11}", "10");
         }
         if (answer12 != null) {
            embed.AddField(answer12, 0, true);
            builder.WithButton($"{answer12}", "11");
         }
         if (answer13 != null) {
            embed.AddField(answer13, 0, true);
            builder.WithButton($"{answer13}", "12");
         }
         if (answer14 != null) {
            embed.AddField(answer14, 0, true);
            builder.WithButton($"{answer14}", "13");
         }
         if (answer15 != null) {
            embed.AddField(answer15, 0, true);
            builder.WithButton($"{answer15}", "14");
         }
         if (answer16 != null) {
            embed.AddField(answer16, 0, true);
            builder.WithButton($"{answer16}", "15");
         }
         if (answer17 != null) {
            embed.AddField(answer17, 0, true);
            builder.WithButton($"{answer17}", "16");
         }
         if (answer18 != null) {
            embed.AddField(answer18, 0, true);
            builder.WithButton($"{answer18}", "17");
         }
         if (answer19 != null) {
            embed.AddField(answer19, 0, true);
            builder.WithButton($"{answer19}", "18");
         }
         if (answer20 != null) {
            embed.AddField(answer20, 0, true);
            builder.WithButton($"{answer20}", "19");
         }
         if (answer21 != null) {
            embed.AddField(answer21, 0, true);
            builder.WithButton($"{answer21}", "20");
         }
         if (answer22 != null) {
            embed.AddField(answer22, 0, true);
            builder.WithButton($"{answer22}", "21");
         }
         if (answer23 != null) {
            embed.AddField(answer23, 0, true);
            builder.WithButton($"{answer23}", "22");
         }
         if (answer24 != null) {
            embed.AddField(answer24, 0, true);
            builder.WithButton($"{answer24}", "23");
         }
         var message = await ReplyAsync("", false, embed.Build(), components: builder.Build());
      }
   }

   // public class Poll {
   //    public string question;
   //    public Options options = new Options();
   //    // public List<Vote> votes = new List<Vote>();
   //    public Poll(string question, Options options) {
   //       this.question = question;
   //       this.options = options;
   //    }
   //    // public void AddVote(ulong userId, string option) {
   //    //    votes.Add(new Vote(userId, option));
   //    // }
   // }
}

//          public class Vote {
//             public ulong userId;
//             public string option;
//             public Vote(ulong userId, string option) {
//                this.userId = userId;
//                this.option = option;
//             }
//          }
//      }
// }