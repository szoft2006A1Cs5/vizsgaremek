namespace backend.Models
{
    public class Uzenet
    {
        public int Id { get; set; }
        public required string Tartalom { get; set; }
        public DateTime KuldesiIdo { get; set; }
        public bool Panasz { get; set; }
        public int KuldoId { get; set; }
        public required Felhasznalo Kuldo { get; set; }
        public int BerlesId { get; set; }
        public required Berles Berles { get; set; }
    }
}
