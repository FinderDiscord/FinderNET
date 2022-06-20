using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class Economy {
        [Key]
        public Int64 guildId { get; set; }
        [Key]
        public Int64 userId { get; set; }
        public int money { get; set; }
        public int bank { get; set; }
    }
}
