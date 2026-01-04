using backend.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Mozilla;

namespace backend.Contexts
{
    public class Context : DbContext
    {
        public DbSet<Felhasznalo> Felhasznalok { get; set; }
        public DbSet<Jarmu> Jarmuvek { get; set; }
        public DbSet<JarmuKep> JarmuKepek { get; set; }
        public DbSet<JarmuBerelhetoseg> JarmuBerelhetosegek { get; set; }
        public DbSet<Berles> Berlesek { get; set; }
        public DbSet<Ertesites> Ertesitesek { get; set; }
        public DbSet<Uzenet> Uzenetek { get; set; }
        public DbSet<UzenetCsatolmany> UzenetCsatolmanyok { get; set; }

        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
