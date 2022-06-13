using System.ComponentModel.DataAnnotations;
namespace FinderNET.Database.Models {
    public class Polls {
        [Key]
        public Int64 messageId { get; set; }
        public List<string> answers { get; set; }
        public List<Int64> votersId { get; set; }
    }
}