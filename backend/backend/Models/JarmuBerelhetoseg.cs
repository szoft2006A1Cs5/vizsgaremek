using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    public enum BerelhetosegIsmetlodes
    {
        Nincs = 0,
        Hetente = 1,
        Kethetente = 2,
        Havonta = 3,
    }

    [PrimaryKey(nameof(JarmuId), nameof(Kezdet), nameof(Veg))]
    public class JarmuBerelhetoseg
    {
        public required string JarmuId { get; set; }
        public Jarmu Jarmu { get; set; }
        public DateTime Kezdet { get; set; }
        public DateTime Veg { get; set; }
        public BerelhetosegIsmetlodes Ismetlodes { get; set; }
        public int Oradij { get; set; }
    }
}
