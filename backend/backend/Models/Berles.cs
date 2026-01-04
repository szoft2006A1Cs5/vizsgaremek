using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public enum BerlesAllapot { 
        BerloJavaslat = 0,
        BerbeadoJavaslat = 1,
        Elfogadva = 2,
        BerloAtvetelElfogadva = 3,
        BerbeadoAtvetelElfogadva = 4,
        Aktiv = 5,
        BerloLezarasElfogadva = 6,
        BerbeadoLezarasElfogadva = 7,
        Lezarva = 8,
        Visszamondva = 9,
    };

    public class Berles
    {
        public int Id { get; set; }
        public int TeljesAr { get; set; }
        public int Letet { get; set; }
        public DateTime Kezdet { get; set; }
        public DateTime Veg { get; set; }
        public BerlesAllapot Allapot { get; set; }
        public double AtveteliHelySzelesseg { get; set; }
        public double AtveteliHelyHosszusag { get; set; }

        [NotMapped]
        public Tuple<double, double> AtveteliHely
        {
            get
            {
                return Tuple.Create(AtveteliHelySzelesseg, AtveteliHelyHosszusag);
            }
            set
            {
                AtveteliHelySzelesseg = value.Item1;
                AtveteliHelyHosszusag = value.Item2;
            }
        }

        public double? Uzemanyagszint { get; set; }
        public double? BerloErtekeles { get; set; }
        public double? BerbeadoErtekeles { get; set; }

        public int BerloId { get; set; }
        public Felhasznalo Berlo { get; set; }
        public required string JarmuId { get; set; }
        public Jarmu Jarmu { get; set; }
    }
}
