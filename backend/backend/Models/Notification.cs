using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        [MaxLength(512)]
        public required string Content { get; set; }
        public DateTime TimeSent { get; set; }
        public bool Read { get; set; }
    }
}
