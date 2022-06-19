using FinderNET.Helpers.Enums;
namespace FinderNET.Helpers {
    public class ModerationMessage {
        public ulong messageId { get; set; }
        public ulong guildId { get; set; }
        public ulong channelId { get; set; }
        public ulong senderId { get; set; }
        public ulong userId { get; set; }
        public string reason { get; set; } = "No reason given.";
        public ModerationMessageType Type { get; set; }
        // add date?
    }
}