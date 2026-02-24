using backend.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Mozilla;

namespace backend.Contexts
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }
        public DbSet<VehicleAvailability> VehicleAvailabilities { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }

        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rental>()
                .Property(x => x.Status)
                .HasConversion<int>();

            modelBuilder.Entity<VehicleAvailability>()
                .Property(x => x.Recurrence)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .Property(x => x.Role)
                .HasConversion<string>();
        }
    }
}
