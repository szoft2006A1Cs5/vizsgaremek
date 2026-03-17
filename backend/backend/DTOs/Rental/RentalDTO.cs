using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs.Rental
{
    public class RentalDTO
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public RentalStatus Status { get; set; }

        [MaxLength(512)]
        public required string PickupLocation { get; set; }

        public double? FuelLevel { get; set; }
        public double? RenterRating { get; set; }
        public double? OwnerRating { get; set; }
        
        public int VehicleId { get; set; }
    }
}
