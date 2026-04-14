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
        
        /// <summary>
        /// Kezeli a berlesek allapotvaltozasat.
        /// </summary>
        /// <param name="curr">A berles amelyen valtoztatni kivanunk</param>
        /// <param name="change">A valtozasok</param>
        /// <param name="authUser">A bejelentkezett felhasznalo, aki vegrehajtja a valtoztatast</param>
        /// <returns>Egy RentalResultot, ami elmondja milyen HTTP statuszkodot kell majd visszaadnunk</returns>
        private async Task<RentalResult> HandleStatusChange(Rental curr, RentalDTO change, User authUser)
        {
            if (RentalStatus.Cancelled == change.Status)
                return await this.TryCancel(curr, authUser);
            
            var vehicleName = $"{curr.Vehicle.Manufacturer} {curr.Vehicle.Model}";
            
            // Koszonhetoen a statuszok ketoldalu/szimmetrikus jellegenek
            // nem kell explicit minden statusz valtozast kulon kulon lekezelni,
            // hanem mivel tudjuk egy adott statusz csoportnak az elso statuszat,
            // abbol kiindulva egesz egyszeruen tudjuk eldonteni, mikor kell a kovetkezo
            // csoportra lepni.
            
            // Eloszor is megkeressuk az elso olyan csoport alapjat, amelyen belul jelenleg
            // van a berles statusza, illetve megnezzuk, hogy tovabb akarunk e lepni, vagy
            // (erre tulajdonkeppen csak egy eset van, az ajanlatkuldozgetes, mivel a visszamondast
            // kulon kezeljuk) visszaakarunk lepni.
            var b = new int?[] { 
                (int)RentalStatus.RenterOffer,
                (int)RentalStatus.RenterPickupAccepted,
                (int)RentalStatus.RenterFinishAccepted,
            }
            .FirstOrDefault(b => (int)curr.Status < b + 2 &&
                                 (int)change.Status == b + 2);

            // Ha null, akkor vagy visszafele lepunk, vagy invalid a statusz lepes, igy nem valtoztatunk a
            // statuszon
            if (b == null)
            {
                if (curr.Status < RentalStatus.OfferAccepted)
                    curr.Status = authUser.Id == curr.RenterId ? RentalStatus.RenterOffer : RentalStatus.OwnerOffer;
            
                return RentalResult.Ok(curr);
            }

            if ((RentalStatus)b == RentalStatus.RenterPickupAccepted &&
                _timePrv.GetUtcNow() < curr.Start)
                return RentalResult.BadRequest("A bérlés nem kezdődhet meg a megadott időpont előtt!");
                
            // Nyilvan attol fuggoen, hogy a berlesben, berlokent, vagy berbeadokent veszunk reszt,
            // tudjuk eldonteni, hogy a masik fel mar elfogadta-e.
            var otherAccepted = (int)curr.Status == (authUser.Id == curr.RenterId ? b + 1 : b);
            // Ha igen tovabblephetunk a masik csoportra, ha nem, akkor beallitjuk hogy mi mar elfogadtuk.
            var resStatus = otherAccepted
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
                            .Include(x => x.Vehicle)
                            .ThenInclude(x => x.Owner)
                            .Where(x =>
                                x.Id != curr.Id &&
                                x.VehicleId == curr.VehicleId &&
                                !(x.End < curr.Start || curr.End < x.Start)
                            )
                            .ToListAsync();
                        
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

                        foreach (var rental in conflict)
                            await this.TryCancel(rental, curr.Vehicle.Owner!);
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
        
        /// <summary>
        /// Kezeli a berlesek property-jeinek valtozasat
        /// </summary>
        /// <param name="curr">A berles amelyen valtoztatni kivanunk</param>
        /// <param name="changed">A valtozasok</param>
        /// <param name="authUser">A bejelentkezett felhasznalo, aki vegrehajtja a valtoztatasokat</param>
        /// <returns>Egy RentalResultot, ami elmondja milyen HTTP statuszkodot kell majd visszaadnunk</returns>
        public async Task<RentalResult> Update(Rental curr, RentalDTO changed, User authUser)
        {
            if (RentalStatus.Cancelled <= curr.Status) return RentalResult.Ok(curr);
            
            var startingStatus = curr.Status;
            
            var statRes = await HandleStatusChange(curr, changed, authUser);
            if (statRes.StatusCode != 200) return statRes;
            
            if (curr.Status != startingStatus &&
                RentalStatus.OfferAccepted <= curr.Status) 
                return RentalResult.Ok(curr);
            
            // Reflectionnel tudjuk allitani,
            // hogy statusz es a bejelentkezett felhasznalo alapjan
            // mely propertyk valtozzanak meg
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
            
            if (authUser.Role != UserRole.Administrator)
                if (curr.Status != RentalStatus.Finished)
                    props = props.Where(x => !(new[]
                    {
                        nameof(Rental.OwnerRating),
                        nameof(Rental.RenterRating)
                    }.Contains(x.Name)));
                else
                    props = props.Where(x =>
                        x.Name != (authUser.Id == curr.RenterId
                            ? nameof(Rental.OwnerRating)
                            : nameof(Rental.RenterRating)));

            if (authUser.Role != UserRole.Administrator &&
                RentalStatus.OfferAccepted <= curr.Status)
                props = props.Where(x => !(new[]
                {
                    nameof(Rental.RentalPrice),
                    nameof(Rental.Start),
                    nameof(Rental.End),
                    nameof(Rental.PickupLocation)
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

        /// <summary>
        /// Megprobalja visszamondani a berlest,
        /// ha a berles meg nem lepett ajanlatoknal tovabb,
        /// akkor megprobalja torolni is.
        /// </summary>
        /// <param name="rental">A visszamondando/torlendo berles</param>
        /// <param name="authUser">A bejelentkezett felhasznalo, aki visszamondja azt</param>
        /// <returns>Egy RentalResultot, ami elmondja milyen HTTP statuszkodot kell majd visszaadnunk</returns>
        public async Task<RentalResult> TryCancel(Rental rental, User authUser)
        {
            if (RentalStatus.Active <= rental.Status)
                return RentalResult.BadRequest("Már aktív/befejezett bérlés nem mondható le!");
            
            var vehicleName = $"{rental.Vehicle.Manufacturer} {rental.Vehicle.Model}";
            
            if (RentalStatus.OfferAccepted <= rental.Status)
            {
                rental.Renter.Balance += rental.FullPrice;
                rental.Vehicle.Owner!.Balance -= rental.RentalPrice;

                rental.Status = RentalStatus.Cancelled;

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
