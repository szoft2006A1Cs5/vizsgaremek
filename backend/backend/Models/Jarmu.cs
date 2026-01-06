using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Jarmu")]
    public class Jarmu
    {
        public int Id { get; set; }
        public int TulajdonosId { get; set; }
        public required Felhasznalo Tulajdonos { get; set; }
        public required string Alvazszam { get; set; }
        public required string Rendszam { get; set; }
        public required string Marka { get; set; }
        public required string Tipus { get; set; }
        public int Evjarat { get; set; }
        public required string Leiras { get; set; }
        public int KmAllas { get; set; }
        public double AtlagFogyasztas { get; set; }
        public required string BiztositasiSzam { get; set; }
    }
}
