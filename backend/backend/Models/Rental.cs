using backend.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json.Serialization;
using backend.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace backend.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
        Cancelled = 9,
    };

    public class Rental
    {
        public int Id { get; set; }
        public int RentalPrice { get; set; }
        [NotMapped] public int Commission => (int)(RentalPrice * 0.05);
        [NotMapped] public int FullPrice => RentalPrice + Commission;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public RentalStatus Status { get; set; }

        [MaxLength(512)]
        public required string PickupLocation { get; set; }

        public double? RenterRating { get; set; }
        public double? OwnerRating { get; set; }

        public int RenterId { get; set; }
        public required User Renter { get; set; }
        public int VehicleId { get; set; }
        public required Vehicle Vehicle { get; set; }
    }
}
