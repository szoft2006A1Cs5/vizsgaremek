using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    [PrimaryKey(nameof(JarmuId), nameof(EleresiUt))]
    public class JarmuKep
    {
        public int JarmuId { get; set; }
        public required Jarmu Jarmu { get; set; }
        public required string EleresiUt { get; set; }
    }
}
