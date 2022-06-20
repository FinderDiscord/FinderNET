using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class LevelingModel {
        [Key]
        public Int64 guildId { get; set; }
        [Key]
        public Int64 userId { get; set; }
        public int level { get; set; }
        public int exp { get; set; }
    }
}