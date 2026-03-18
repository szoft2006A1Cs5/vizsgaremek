namespace backend.DTOs.Message;

public class MessageSendDTO
{
    public required string Content { get; set; }
    public bool IsComplaint { get; set; }
}