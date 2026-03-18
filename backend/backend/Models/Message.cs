using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class Message
    {
        [JsonIgnore]
        public int Id { get; set; }
        [MaxLength(2048)]
        public required string Content { get; set; }
        public required bool IsImage { get; set; }
        public DateTime TimeSent { get; set; }
        public bool IsComplaint { get; set; }
        public int SenderId { get; set; }
        [JsonIgnore]
        public User Sender { get; set; }
        public int RentalId { get; set; }
        [JsonIgnore]
        public Rental Rental { get; set; }
    }
}
