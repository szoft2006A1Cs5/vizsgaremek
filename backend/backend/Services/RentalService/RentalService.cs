using System.Reflection;
using backend.Contexts;
using backend.DTOs.Rental;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.RentalService
{
    public class RentalService
    {
        private readonly Context _context;
        private readonly TimeProvider _timePrv;
        
        public RentalService(
            Context context,
            TimeProvider timePrv
        ) 
        {
            _context = context;
            _timePrv = timePrv;
        }
        
        private async Task<RentalResult> HandleStatusChange(Rental curr, RentalDTO change, User authUser)
        {
            if (RentalStatus.RenterCancelled <= change.Status)
                return await this.TryCancel(curr, authUser);
            
            var vehicleName = $"{curr.Vehicle.Manufacturer} {curr.Vehicle.Model}";
            
            var b = new int?[] { 
                (int)RentalStatus.RenterOffer,
                (int)RentalStatus.RenterPickupAccepted,
                (int)RentalStatus.RenterFinishAccepted,
            }
            .FirstOrDefault(b => (int)curr.Status < b + 2 &&
                                 (int)change.Status == b + 2);

            if (b == null)
            {
                if (curr.Status < RentalStatus.OfferAccepted)
                    curr.Status = authUser.Id == curr.RenterId ? RentalStatus.RenterOffer : RentalStatus.OwnerOffer;
            
                return RentalResult.Ok(curr);
            }

            if ((RentalStatus)b == RentalStatus.RenterPickupAccepted &&
                _timePrv.GetUtcNow() < curr.Start)
                return RentalResult.BadRequest("A bérlés nem kezdődhet meg a megadott időpont előtt!");
                
            var otherWaiting = (int)curr.Status == (authUser.Id == curr.RenterId ? b + 1 : b);
            var resStatus = otherWaiting
                ? (RentalStatus)(b + 2)
                : (RentalStatus)(authUser.Id == curr.RenterId ? b : b + 1);

            switch (resStatus)
            {
                case RentalStatus.OfferAccepted:
                    {
                        if (curr.Renter.Balance < curr.FullPrice)
                            return RentalResult.BadRequest("A bérlő egyenlege nem elég a bérlés megfizetéséhez!");
                        
                        curr.Renter.Balance -= curr.FullPrice;
                        curr.Vehicle.Owner!.Balance += curr.RentalPrice;
                        
                        var conflict = await _context.Rentals
                            .Include(x => x.Renter)
                            .Where(x =>
                                x.Id != curr.Id &&
                                x.VehicleId == curr.VehicleId &&
                                !(x.End < curr.Start || curr.End < x.Start)
                            )
                            .ToListAsync();

                        _context.Rentals.RemoveRange(conflict);
                        
                        await Notification.Send(
                            curr.RenterId,
                            $"A bérlési ajánlat {curr.Vehicle.Owner.Name}-val/vel, a(z) {vehicleName}-ra/re " +
                            $"elfogadásra került.",
                            _context
                        );
                        
                        await Notification.Send(
                            curr.Vehicle.OwnerId,
                            $"A bérlési ajánlat {curr.Renter.Name}-val/vel, a(z) {vehicleName}-ra/re " +
                            $"elfogadásra került.",
                            _context
                        );

                        foreach (var renterId in conflict.Select(x => x.RenterId))
                            await Notification.Send(
                                renterId,
                                $"A bérlési javaslatodat a(z) {vehicleName}-ra/re visszautasították.",
                                _context
                            );
                    }
                    break;
                case RentalStatus.Active:
                    {
                        await Notification.Send(
                            curr.RenterId,
                            $"A {vehicleName}-ra/re vonatkozó bérlés megkezdődött! Jó utat!",
                            _context
                        );
                        
                        await Notification.Send(
                            curr.Vehicle.OwnerId,
                            $"A {vehicleName}-ra/re vonatkozó bérlés megkezdődött!",
                            _context
                        );
                    }
                    break;
                case RentalStatus.Finished:
                    {
                        await Notification.Send(
                            curr.RenterId,
                            $"A {vehicleName}-ra/re vonatkozó bérlés lezárult!",
                            _context
                        );
                        
                        await Notification.Send(
                            curr.Vehicle.OwnerId,
                            $"A {vehicleName}-ra/re vonatkozó bérlés lezárult!",
                            _context
                        );
                    }
                    break;
            }

            curr.Status = resStatus;
            return RentalResult.Ok(curr);
        }
        
        public async Task<RentalResult> Update(Rental curr, RentalDTO changed, User authUser)
        {
            if (RentalStatus.RenterCancelled <= curr.Status) return RentalResult.Ok(curr);
            
            var startingStatus = curr.Status;
            
            var statRes = await HandleStatusChange(curr, changed, authUser);
            if (statRes.StatusCode != 200) return statRes;
            
            if (curr.Status != startingStatus &&
                RentalStatus.OfferAccepted <= curr.Status) 
                return RentalResult.Ok(curr);
            
            IEnumerable<PropertyInfo> props = typeof(Rental)
                .GetProperties()
                .Where(x => (authUser.Role != UserRole.Administrator ? !(new[]
                            {
                                nameof(Rental.VehicleId),
                                nameof(Rental.RenterId),
                                nameof(Rental.Status), // Status kulon kezelve
                            }.Contains(x.Name)) : true) &&
                            !(new[]
                            {
                                nameof(Rental.Id),
                                nameof(Rental.Vehicle),
                                nameof(Rental.Renter)
                            }.Contains(x.Name)));
            
            if (authUser.Role != UserRole.Administrator &&
                curr.Status != RentalStatus.Finished)
                props = props.Where(x => !(new[]
                {
                    nameof(Rental.OwnerRating), nameof(Rental.RenterRating)
                }.Contains(x.Name)));

            if (authUser.Role != UserRole.Administrator &&
                RentalStatus.OfferAccepted <= curr.Status)
                props = props.Where(x => !(new[]
                {
                    nameof(Rental.RentalPrice),
                    nameof(Rental.Start),
                    nameof(Rental.End),
                    nameof(Rental.PickupLocation),
                    nameof(Rental.FuelLevel)
                }.Contains(x.Name)));

            var dtoProps = typeof(RentalDTO).GetProperties();
            foreach (var prop in props)
            {
                var dtoProp = dtoProps.FirstOrDefault(x => x.Name == prop.Name &&
                                                           x.PropertyType == prop.PropertyType);
                if (dtoProp != null)
                    prop.SetValue(curr, dtoProp.GetValue(changed));
            }

            return RentalResult.Ok(curr);
        }

        public async Task<RentalResult> TryCancel(Rental rental, User authUser)
        {
            if (RentalStatus.Active <= rental.Status)
                return RentalResult.BadRequest("Már aktív bérlés nem mondható le!");
            
            var vehicleName = $"{rental.Vehicle.Manufacturer} {rental.Vehicle.Model}";
            
            if (RentalStatus.OfferAccepted <= rental.Status)
            {
                rental.Renter.Balance += rental.FullPrice;
                rental.Vehicle.Owner!.Balance -= rental.RentalPrice;
                    
                rental.Status = authUser.Id == rental.RenterId ? 
                    RentalStatus.RenterCancelled : 
                    RentalStatus.OwnerCancelled;

                await Notification.Send(
                    rental.RenterId,
                    $"A {vehicleName}-ra/re vonatkozó bérlés vissza lett mondva!",
                    _context
                );
                
                await Notification.Send(
                    rental.Vehicle.OwnerId,
                    $"A {vehicleName}-ra/re vonatkozó bérlés vissza lett mondva!",
                    _context
                );
                
                return RentalResult.Ok(rental);
            }
            
            await Notification.Send(
                rental.RenterId,
                $"A {vehicleName}-ra/re vonatkozó bérlés törölve lett!",
                _context
            );
                
            await Notification.Send(
                rental.Vehicle.OwnerId,
                $"A {vehicleName}-ra/re vonatkozó bérlés törölve lett!",
                _context
            );
            
            _context.Rentals.Remove(rental);
            return RentalResult.NoContent();
        }
    }
}
