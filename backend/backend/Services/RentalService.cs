using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class RentalService
    {
        private readonly Context _context;

        public RentalService(Context context) 
        {
            _context = context;
        }

        public async Task<Rental?> Add(Rental offer, User authUser)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x => x.Id == offer.VehicleId);

            if (vehicle == null) return null;
            if (vehicle.OwnerId == authUser.Id) return null;

            var priceOffer = vehicle.GetPriceOffer(offer.Start, offer.End);
            if (priceOffer == null) return null;

            var rental = new Rental
            {
                Start = offer.Start,
                End = offer.End,
                VehicleId = vehicle.Id,
                Vehicle = vehicle,
                FuelLevel = offer.FuelLevel,
                RentalPrice = priceOffer.Value.RentalPrice,
                Renter = authUser,
                Status = RentalStatus.RenterOffer,
            };

            await _context.Rentals.AddAsync(rental);
            await _context.SaveChangesAsync();
            
            return null;
        }
        
        public bool Update(Rental rental, User authUser)
        {
            return true;
        }
    }
}
