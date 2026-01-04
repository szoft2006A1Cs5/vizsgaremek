using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public enum FelhasznaloJogosultsag { 
        Felhasznalo = 0,
        Admin = 1
    };

    public class Felhasznalo
    {
        public int Id { get; set; }
        public required string SzemelyiSzam { get; set; }
        public required string Nev { get; set; }
        public required string Telefonszam { get; set; }
        public DateTime SzuletesiDatum { get; set; }
        public string? ProfilKepEleresiUt { get; set; }
        public required string Email { get; set; }
        public required string Jelszo { get; set; } // HASH
        public FelhasznaloJogosultsag Jogosultsag { get; set; }
        public required string JogositvanySzam { get; set; }
        public DateTime JogositvanyKiallitasDatum { get; set; }
        public required string cimIranyitoszam { get; set; }
        public required string cimTelepules { get; set; }
        public required string cimUtcaHazszam { get; set; }
        public int Egyenleg { get; set; }
    }
}
