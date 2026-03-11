using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using backend.Common;
using backend.VisibilityFiltering;
using Humanizer.DateTimeHumanizeStrategy;

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

        public int Horsepower { get; set; }

        public double AvgFuelConsumption { get; set; }

        [MaxLength(10)]
        public required string FuelType { get; set; } 

        [VisibleTo(VisibilityLevel.OwnerOnly), MaxLength(64)]
        public required string InsuranceNumber { get; set; }

        public ICollection<VehicleAvailability> Availabilities { get; set; } = [];
        public ICollection<VehicleImage> Images { get; set; } = [];

        [VisibleTo(VisibilityLevel.OwnerOnly)] 
        public ICollection<Rental> Rentals { get; set; } = [];

        [NotMapped]
        public double? Rating
        {
            get => this.Rentals.Average(x => x.OwnerRating);
        }
        
        [NotMapped] 
        [JsonExtensionData] 
        public Dictionary<string, object?> ExtensionData { get; } = new();
        
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
                                          (model.Rentals.Any(x => x.RenterId == auth.Id &&
                                                                  RentalStatus.OfferAccepted <= x.Status) ||
                                           model.OwnerId == auth.Id ||
                                           auth.Role == UserRole.Administrator);
                case VisibilityLevel.OwnerOnly:
                    return (obj, auth) => obj is Vehicle model &&
                                          auth != null &&
                                          (model.OwnerId == auth.Id ||
                                           auth.Role == UserRole.Administrator);
                default:
                    return (_, _) => false;
            }
        }
        
        public bool CheckAvailable(DateTime intervalStart, DateTime intervalEnd)
        {
            // Ha van mar berles amit elfogadtak es utkozik a megadott datummal,
            // akkor nyilvan nem elerheto az idoszakra, emellett a jarmu tulajdonosa
            // altal meghatarozott berelhetosegi idoszakban van-e a megadott intervallum.
            return !this.Rentals.Any(r => RentalStatus.OfferAccepted <= r.Status && 
                                          !(r.End < intervalStart || intervalEnd < r.Start)) &&
                   this.Availabilities.Any(a => a.Start <= intervalStart && intervalEnd <= a.End) &&
                   intervalStart < intervalEnd &&
                   DateTime.Now < intervalStart;
        }

        public VehicleRentalOffer? GetInitialRentalOffer(DateTime? intervalStart, DateTime? intervalEnd)
        {
            if (intervalStart == null || intervalEnd == null) return null;
            
            // Ha van mar az idoszakban berles, nyilvan nem berelheto
            if (this.Rentals.Any(r => RentalStatus.OfferAccepted <= r.Status &&
                                      !(r.End < intervalStart || intervalEnd < r.Start)))
                return null;

            var relevantAvailabilites = this.Availabilities
                .Where(a => !(a.End < intervalStart || intervalEnd < a.Start))
                .OrderBy(a => a.Start)
                .ToList();

            // Ha nem kapunk vissza semmit, vagy
            // a kezdeti/veg datumaink nem esnek bele
            // egyik elerhetosegbe sem.
            if (!relevantAvailabilites.Any() ||
                !(relevantAvailabilites.First().Start <= intervalStart &&
                 intervalEnd <= relevantAvailabilites.Last().End))
                return null;
            
            var fullPrice = 0.0;
            for (int i = 0; i < relevantAvailabilites.Count; i++)
            {
                var availability = relevantAvailabilites[i];
                DateTime start = i == 0 ? intervalStart.Value : availability.Start;
                DateTime end = i == relevantAvailabilites.Count - 1 ? intervalEnd.Value : availability.End;
                
                if (end != intervalEnd && availability.End != relevantAvailabilites[i + 1].Start)
                    return null;
                
                fullPrice += (end - start).TotalHours * availability.HourlyRate;
            }

            return new VehicleRentalOffer
            {
                Start = intervalStart.Value,
                End = intervalEnd.Value,
                Rates = relevantAvailabilites.Select(x => x.HourlyRate).ToList(),
                FullPrice = (int)Math.Round(fullPrice),
            };
        }
    }

    public struct VehicleRentalOffer
    {
        public List<int> Rates { get; set; }
        public int FullPrice { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
