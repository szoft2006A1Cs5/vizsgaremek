using backend.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Vehicle
    {
        public int Id { get; set; }

        [VisibilityKey]
        public int OwnerId { get; set; }

        public required User Owner { get; set; }

        [VisibleTo(VisibilityLevel.InRelation)]
        public required string VIN { get; set; }

        [VisibleTo(VisibilityLevel.InRelation)]
        public required string LicensePlate { get; set; }

        public required string Manufacturer { get; set; }

        public required string Model { get; set; }

        public int Year { get; set; }

        public required string Description { get; set; }

        public int OdometerReading { get; set; }

        public double AvgFuelConsumption { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public required string InsuranceNumber { get; set; }

        public ICollection<VehicleAvailability> Availabilities { get; set; } = [];
        public ICollection<VehicleImage> Images { get; set; } = [];
    }
}
