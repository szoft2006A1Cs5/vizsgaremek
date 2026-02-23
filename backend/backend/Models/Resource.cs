using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public enum ResourceType
    {
        Image,
        PDF,
        File
    }
    
    public class Resource
    {
        public int Id { get; set; }
        [MaxLength(2048)]
        public required string Path { get; set; }
        public ResourceType Type { get; set; }
    }
}
