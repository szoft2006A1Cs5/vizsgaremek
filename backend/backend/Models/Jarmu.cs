namespace backend.Models
{
    public class Jarmu
    {
        public required string Alvazszam { get; set; }
        public int TulajdonosId { get; set; }
        public Felhasznalo Tulajdonos { get; set; }
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
