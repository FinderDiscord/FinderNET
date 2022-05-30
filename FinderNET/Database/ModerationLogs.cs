using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database {
    public class ModerationLogs {
        [Key]
        public Int64 guildId { get; set; }
        public UserLogs userLogs { get; set; } = new UserLogs();
    }
    public class UserLogs {
        [Key]
        public Int64 userId { get; set; }
        public int bans { get; set; }
        public int kicks { get; set; }
        public int warns { get; set; }
        public int mutes { get; set; }
    }
}