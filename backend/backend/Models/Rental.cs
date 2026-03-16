using backend.Common;
using Swashbuckle.AspNetCore.Annotations;
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
        public int RentalPrice { get; set; }
        [NotMapped] public int Commission => (int)(RentalPrice * 0.05);
        [NotMapped] public int FullPrice => RentalPrice + Commission;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

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
        [SwaggerIgnore]
        public required User Renter { get; set; }
        public int VehicleId { get; set; }
        [SwaggerIgnore]
        public required Vehicle Vehicle { get; set; }

        private void HandleStatusChange(RentalStatus to, User authUser)
        {
            if (RentalStatus.Finished < to)
            {
                Status = to;
                return;
            }
            
            var bases = new[] { 
                (int)RentalStatus.RenterOffer,
                (int)RentalStatus.RenterPickupAccepted,
                (int)RentalStatus.RenterFinishAccepted,
            };

            foreach (var b in bases)
            {
                if ((int)to != b + 2) continue;
                
                bool otherWaiting = (int)this.Status == (authUser.Id == this.RenterId ? b + 1 : b);
                this.Status = otherWaiting
                    ? (RentalStatus)(b + 2)
                    : (RentalStatus)(authUser.Id == this.RenterId ? b : b + 1);

                // TODO: Ha OfferAccepted, akkor itt elmeletileg valahogy torolni
                //       kene az osszes tobbi erre az idoszakra vontakozo rental offert,
                //       es kuldeni azok kuldoinek egy ertesitest, hogy nem az o ajanlatukat
                //       fogadtak el.
                
                return;
            }
            
            if (this.Status < RentalStatus.OfferAccepted)
                this.Status = authUser.Id == this.RenterId ? RentalStatus.RenterOffer : RentalStatus.OwnerOffer;
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
                    nameof(Status), // Status kulon kezelve
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
                    nameof(RentalPrice),
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
