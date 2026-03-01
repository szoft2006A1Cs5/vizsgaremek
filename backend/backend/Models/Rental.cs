using backend.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
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
        public User? Renter { get; set; }
        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        private void HandleStatusChange(RentalStatus to, User authUser)
        {
            
        }
        
        public bool Update(Rental to, User authUser)
        {
            if (this.Status != to.Status)
            {
                // Statusz valtozasnal mas nem valtozhat,
                // pl. ha elfogadjuk a masik ajanlatat,
                // akkor nyilvan nem modosithatjuk a sajat
                // valtoztatasainkra es fogadhatjuk el
                // egyszerre.
                
                HandleStatusChange(to.Status, authUser);
            }
            
            IEnumerable<PropertyInfo> props = typeof(Rental)
                .GetProperties()
                .Where(x => authUser.Role != UserRole.Administrator ? !(new[]
                {
                    nameof(Id),
                    nameof(VehicleId),
                    nameof(RenterId),
                    nameof(Status), // Status kulon lesz kezelve
                }.Contains(x.Name)) : true);
            
            if (authUser.Role != UserRole.Administrator &&
                this.Status != RentalStatus.Finished)
                props = props.Where(x => !(new[]
                {
                    nameof(OwnerRating), nameof(RenterRating)
                }.Contains(x.Name)));

            if (authUser.Role != UserRole.Administrator &&
                RentalStatus.OfferAccepted <= this.Status)
                props = props.Where(x => !(new[]
                {
                    nameof(FullPrice),
                    nameof(Downpayment),
                    nameof(Start),
                    nameof(End),
                    nameof(PickupLatitude),
                    nameof(PickupLongtitude),
                    nameof(FuelLevel)
                }.Contains(x.Name)));

            foreach (var prop in props)
                prop.SetValue(this, prop.GetValue(to));
            
            return false;
        }
    }
}
