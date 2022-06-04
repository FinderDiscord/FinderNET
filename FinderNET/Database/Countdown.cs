using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database {
    public class Countdown {
        [Key]
        public Int64 messageId { get; set; }
        [Key]
        public Int64 channelId { get; set; }
        [Key]
        public Int64 guildId { get; set; }
        public DateTime dateTime { get; set; }
        // public List<Int64> pingUsersId { get; set; }
        // todo add ping roles support
    }
}