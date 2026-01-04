using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    [PrimaryKey(nameof(JarmuId), nameof(EleresiUt))]
    public class JarmuKep
    {
        public required string JarmuId { get; set; }
        public required string EleresiUt { get; set; }
    }
}
