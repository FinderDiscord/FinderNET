using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class TicketsModel {
        [Key]
        public Int64 guildId { get; set; }
        [Key]
        public Int64 supportChannelId { get; set; }
        public Int64? introMessageId { get; set; }
        public List<Int64?> userIds { get; set; }
        public string? name { get; set; }
        public List<Int64> claimedUserId { get; set; }
    }
}