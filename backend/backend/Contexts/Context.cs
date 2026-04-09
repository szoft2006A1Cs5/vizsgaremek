using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Org.BouncyCastle.Asn1.Mozilla;

namespace backend.Contexts
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
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

            modelBuilder.Entity<Message>()
                .HasOne(x => x.Rental)
                .WithMany()
                .HasForeignKey(x => x.RentalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .Property(x => x.Role)
                .HasConversion<string>();

            modelBuilder.Entity<UserToken>()
                .Property(x => x.Type)
                .HasConversion<string>();
            
            modelBuilder.Entity<User>()
                .Property(x => x.DateOfBirth)
                .HasColumnType("date")
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v)
                );

            // Minden DateTime legyen UTC
            var toUtc = new ValueConverter<DateTime, DateTime>(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
            
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
                foreach (var prop in entity.GetProperties())
                    if (prop.ClrType == typeof(DateTime))
                        prop.SetValueConverter(toUtc);
        }
    }
}
