namespace backend.Models
{
    public class Message
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTime TimeSent { get; set; }
        public bool IsComplaint { get; set; }
        public int SenderId { get; set; }
        public required User Sender { get; set; }
        public int RentalId { get; set; }
        public required Rental Rental { get; set; }
    }
}
