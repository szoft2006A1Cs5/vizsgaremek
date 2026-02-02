using System.ComponentModel.DataAnnotations;
using backend.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using backend.VisibilityFiltering;

namespace backend.Models
{
    public class Vehicle : IFilterable<Vehicle>
    {
        public int Id { get; set; }

        [VisibilityKey]
        public int OwnerId { get; set; }

        public User? Owner { get; set; }

        [VisibleTo(VisibilityLevel.InRelation), MaxLength(17)]
        public required string VIN { get; set; }

        [VisibleTo(VisibilityLevel.InRelation), MaxLength(7)]
        public required string LicensePlate { get; set; }

        [MaxLength(16)]
        public required string Manufacturer { get; set; }

        [MaxLength(32)]
        public required string Model { get; set; }

        public int Year { get; set; }

        [MaxLength(512)]
        public required string Description { get; set; }

        public int OdometerReading { get; set; }

        public double AvgFuelConsumption { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly), MaxLength(64)]
        public required string InsuranceNumber { get; set; }

        public ICollection<VehicleAvailability> Availabilities { get; set; } = [];
        public ICollection<VehicleImage> Images { get; set; } = [];
        [VisibleTo(VisibilityLevel.OwnerOnly)] 
        public ICollection<Rental> Rentals { get; set; } = [];

        public static Expression<Func<Vehicle, bool>>? GetVisibilityConditionExpression(VisibilityLevel visLevel)
        {
            throw new NotImplementedException();
        }
    }
}
