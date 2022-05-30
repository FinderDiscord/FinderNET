namespace FinderNET.Database {
    public class UserLogs {
        public Int64 guildId { get; set; }
        public Int64 userId { get; set; }
        public int bans { get; set; }
        public int kicks { get; set; }
        public int warns { get; set; }
        public int mutes { get; set; }
    }
}