namespace backend.Models
{
    public class Ertesites
    {
        public int Id { get; set; }
        public int FelhasznaloId { get; set; }
        public Felhasznalo Felhasznalo { get; set; }
        public required string Szoveg { get; set; }
        public DateTime KuldesIdeje { get; set; }
        public bool Olvasva { get; set; }
    }
}
