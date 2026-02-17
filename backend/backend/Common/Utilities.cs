using backend.Models;

namespace backend.Common
{
    public static class Utilities
    {
        public static bool DoesCollide(this DateInterval lhs, DateInterval rhs)
        {
            return !((lhs.End < rhs.Start && lhs.End < rhs.End) ||
                     (lhs.Start < rhs.Start && lhs.End < rhs.Start) ||
                     (rhs.End < lhs.Start && rhs.End < lhs.End) ||
                     (rhs.Start < lhs.Start && rhs.End < lhs.Start));
        }

        /// <summary>
        /// Checks if the DateInterval on the left-hand side (lhs) contains
        /// the DateInterval on the right-hand side (rhs), so it will return true if
        /// the timeline builds up like this:
        /// lhs.Start -> rhs.Start -> rhs.End -> lhs.End
        /// </summary>
        /// <param name="lhs">The DateInterval it checks if the other is contained in.</param>
        /// <param name="rhs">The DateInterval it checks if it is contained in the other.</param>
        /// <param name="inclusive">Does it count as containment, if they start and end at the same time</param>
        /// <returns>If lhs contains rhs.</returns>
        public static bool DoesContain(this DateInterval lhs, DateInterval rhs, bool inclusive = true)
        {
            return (inclusive ? 
                lhs.Start <= rhs.Start && rhs.End <= lhs.End : 
                lhs.Start < rhs.Start && rhs.End < lhs.End);
        }

        public static bool CheckAvailable(this Vehicle vehicle, DateInterval interval)
        {
            // Ha van mar berles amit elfogadtak es utkozik a megadott datummal,
            // akkor nyilvan nem elerheto az idoszakra, emellett a jarmu tulajdonosa
            // altal meghatarozott berelhetosegi idoszakban van-e a megadott intervallum.
            return !vehicle.Rentals.Any(x => RentalStatus.OfferAccepted <= x.Status &&
                                             x.DateInterval.DoesCollide(interval)) &&
                   vehicle.Availabilities.Any(x => x.DateInterval.DoesContain(interval));
        }
    }
}
