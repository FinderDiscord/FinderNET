using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class UserLogsModel {
        [Key]
        public Int64 guildId { get; set; }
        [Key]
        public Int64 userId { get; set; }
        public int bans { get; set; }
        public int kicks { get; set; }
        public int warns { get; set; }
        public int mutes { get; set; }
    }
}