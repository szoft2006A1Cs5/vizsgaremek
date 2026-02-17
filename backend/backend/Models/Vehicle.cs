using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using backend.Common;
using backend.VisibilityFiltering;

namespace backend.Models
{
    public class Vehicle : IFilterable
    {
        public int Id { get; set; }

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

        public static Func<object?, User?, bool> GetVisibilityConditionLambda(VisibilityLevel visLevel)
        {
            switch (visLevel)
            {
                case VisibilityLevel.Public:
                    return (_, _) => true;
                case VisibilityLevel.AdminOnly:
                    return (_, auth) => auth != null && auth.Role == UserRole.Administrator;
                case VisibilityLevel.InRelation:
                    return (obj, auth) => obj is Vehicle model &&
                                          auth != null &&
                                          (model.Rentals.Any(x => x.RenterId == auth.Id) ||
                                           model.OwnerId == auth.Id);
                case VisibilityLevel.OwnerOnly:
                    return (obj, auth) => obj is Vehicle model &&
                                          auth != null &&
                                          model.OwnerId == auth.Id;
                default:
                    return (_, _) => false;
            }
        }
        
        
        public bool CheckAvailable(DateInterval interval)
        {
            // Ha van mar berles amit elfogadtak es utkozik a megadott datummal,
            // akkor nyilvan nem elerheto az idoszakra, emellett a jarmu tulajdonosa
            // altal meghatarozott berelhetosegi idoszakban van-e a megadott intervallum.
            return !this.Rentals.Any(x => RentalStatus.OfferAccepted <= x.Status &&
                                             x.DateInterval.DoesCollide(interval)) &&
                   this.Availabilities.Any(x => x.DateInterval.DoesContain(interval));
        }
    }
}
