using System.ComponentModel.DataAnnotations;
using Discord;
namespace FinderNET.Database {
    public class Countdown {
        [Key]
        public Int64 messageId { get; set; }
        [Key]
        public Int64 channelId { get; set; }
        [Key]
        public Int64 guildId { get; set; }
        public DateTime dateTime { get; set; }
        public Int64? pingUserId { get; set; }
        public Int64? pingRoleId { get; set; }
    }
}