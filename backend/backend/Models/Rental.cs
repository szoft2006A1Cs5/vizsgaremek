using backend.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public enum RentalStatus
    {
        RenterOffer = 0,
        OwnerOffer = 1,
        OfferAccepted = 2,
        RenterPickupAccepted = 3,
        OwnerPickupAccepted = 4,
        Active = 5,
        RenterFinishAccepted = 6,
        OwnerFinishAccepted = 7,
        Finished = 8,
        RenterCancelled = 9,
        OwnerCancelled = 10,
    };

    public class Rental
    {
        public int Id { get; set; }
        public int FullPrice { get; set; }
        public int Downpayment { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        
        [JsonIgnore]
        [NotMapped]
        public DateInterval DateInterval { 
            get => new DateInterval(Start, End);
        }

        public RentalStatus Status { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongtitude { get; set; }

        [JsonIgnore]
        [NotMapped]
        public Tuple<double, double> Pickup
        {
            get
            {
                return Tuple.Create(PickupLatitude, PickupLongtitude);
            }
            set
            {
                PickupLatitude = value.Item1;
                PickupLongtitude = value.Item2;
            }
        }

        public double? FuelLevel { get; set; }
        public double? RenterRating { get; set; }
        public double? OwnerRating { get; set; }

        public int RenterId { get; set; }
        public required User Renter { get; set; }
        public int VehicleId { get; set; }
        public required Vehicle Vehicle { get; set; }
    }
}
