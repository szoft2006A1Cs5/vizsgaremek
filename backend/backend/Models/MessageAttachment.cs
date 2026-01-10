using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    [PrimaryKey(nameof(MessageId), nameof(AttachmentPath))]
    public class MessageAttachment
    {
       public int MessageId { get; set; }
       public required Message Message { get; set; }
       public required string AttachmentPath { get; set; }
    }
}
