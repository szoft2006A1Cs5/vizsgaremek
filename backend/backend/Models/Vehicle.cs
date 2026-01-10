using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public required User Owner { get; set; }
        public required string VIN { get; set; }
        public required string LicensePlate { get; set; }
        public required string Manufacturer { get; set; }
        public required string Model { get; set; }
        public int Year { get; set; }
        public required string Description { get; set; }
        public int OdometerReading { get; set; }
        public double AvgFuelConsumption { get; set; }
        public required string InsuranceNumber { get; set; }
    }
}
