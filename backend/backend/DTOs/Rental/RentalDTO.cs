namespace backend.DTOs.Rental
{
    public class RentalDTO
    {
        public int VehicleId { get; set; }
        public double? FuelLevel { get; set; }
        public double? RenterRating { get; set; }
        public double? OwnerRating { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
