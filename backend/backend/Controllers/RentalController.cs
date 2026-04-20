using System.ComponentModel.DataAnnotations;
using backend.Contexts;
using backend.DTOs.Message;
using backend.DTOs.Rental;
using backend.Models;
using backend.Services;
using backend.Services.RentalService;
using backend.Services.ResourceService;
using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthService _authSrv;
        private readonly IResourceService _resSrv;
        private readonly RentalService _rentSrv;
        private readonly TimeProvider _timePrv;
        
        public RentalController(
            Context context, 
            AuthService authSrv, 
            IResourceService resSrv, 
            RentalService rentSrv,
            TimeProvider timePrv
        )
        {
            _context = context;
            _authSrv = authSrv;
            _resSrv = resSrv;
            _rentSrv = rentSrv;
            _timePrv = timePrv;
        }
        
        /// <summary>
        /// Visszaadja a bejelentkezett felhasznalohoz tartozo
        /// berleseket, melyekben berloi szerepben vesz reszt
        /// </summary>
        /// <returns>
        /// 200-al a berleseket, ha sikeres,
        /// 401-et be nem jelentkezett felhasznalo eseten
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();

            return Ok(
                (await _context.Rentals
                    .AsNoTracking()
                    .IgnoreAutoIncludes()
                    .Include(x => x.Renter)
                    .Include(x => x.Vehicle)
                    .ThenInclude(x => x.Owner)
                    .Include(x => x.Vehicle)
                    .ThenInclude(x => x.Images)
                    .Where(x => x.RenterId == authUser.Id)
                    .ToListAsync())
                    .FilterSerialize(authUser)
            );
        }

        /// <summary>
        /// Visszaadja a megadott azonositoju berles adatait
        /// </summary>
        /// <param name="id">A berles azonositoja</param>
        /// <returns>
        /// 401-et ha nincs bejelentkezett felhasznalo,
        /// 403-at ha a bejelentkezett felhasznalo nem tartozik a berleshez,
        /// 404-et ha nincs ilyen berles,
        /// 200-at maskeppen.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();
            
            var rental = await _context.Rentals
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Owner)
                .ThenInclude(x => x.Vehicles)
                .ThenInclude(x => x.Rentals)
                .Include(x => x.Renter)
                .ThenInclude(x => x.Rentals)
                .ThenInclude(x => x.Vehicle)
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Availabilities)
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (rental == null) return NotFound();
            
            if (rental.Vehicle.OwnerId != authUser.Id &&
                rental.RenterId != authUser.Id &&
                authUser.Role != UserRole.Administrator) 
                return Forbid();

            return Ok(rental.FilterSerialize(authUser));
        }

        /// <summary>
        /// Letrehoz egy uj berlesi ajanlatot
        /// </summary>
        /// <param name="offer">A berlesi ajanlat</param>
        /// <returns>
        /// 400-at ha az idointervallum nem helyes,
        /// ha nincs megadva atveteli hely,
        /// ha nincs elegendo egyenlege a bejelentkezett felhasznalonak az ajanlattetelhez.
        ///
        /// 401-et ha nincs bejelentkezett felhasznalo.
        /// 
        /// 403-at ha a bejelentkezett felhasznalo nem adott meg jogositvanyszamot,
        /// vagy ha a bejelentkezett felhasznalo a sajat autojat probalta kiberelni.
        ///
        /// 404-et ha nincs a megadott azonositoju jarmu.
        ///
        /// 409-et ha a jarmu nem berelheto a megadott idoszakban.
        ///
        /// 201-et + a berlest, ha a berlesi ajanlat sikeresen letrejott.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RentalDTO offer)
        {
            if (offer.End <= offer.Start || offer.Start <= _timePrv.GetUtcNow())
                return BadRequest(new
                {
                    Error = "Nem lehet a bérlés vége előbb, mint a kezdete, illetve" +
                            "nem kezdhetsz a múltban bérlést!"
                });

            if (string.IsNullOrEmpty(offer.PickupLocation))
                return BadRequest(new { Error = "Nem javasoltál átvételi helyet!" });
            
            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();

            if (authUser.DriversLicenseNumber == null)
                return Forbid();

            var vehicle = await _context.Vehicles
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .FirstOrDefaultAsync(x => x.Id == offer.VehicleId);

            if (vehicle == null) return NotFound();
            if (vehicle.OwnerId == authUser.Id) return Forbid();

            var priceOffer = vehicle.GetQuote(offer.Start, offer.End);
            if (priceOffer == null) return Conflict();

            if (authUser.Balance < priceOffer.Value.FullPrice)
                return BadRequest(new { Error = "Nincs elegendő egyenleged ehhez a bérléshez!" });

            var rental = new Rental
            {
                Start = offer.Start,
                End = offer.End,
                VehicleId = vehicle.Id,
                Vehicle = vehicle,
                RentalPrice = priceOffer.Value.RentalPrice,
                Renter = authUser,
                Status = RentalStatus.RenterOffer,
                PickupLocation = offer.PickupLocation,
            };

            await _context.Rentals.AddAsync(rental);
            await Notification.Send(
                vehicle.OwnerId, 
                $"Új bérlési kérelem érkezett {vehicle.Manufacturer} {vehicle.Model} járművedre.",
                _context
            );
            
            await _context.SaveChangesAsync();

            return Created($"{Request.GetDisplayUrl()}/{rental.Id}", rental.FilterSerialize(authUser));
        }

        /// <summary>
        /// Frissiti a berles adatait,
        /// ez a berles statuszatol fuggoen
        /// mast-mast jelenthet
        /// </summary>
        /// <param name="id">A frissitendo berles azonositoja</param>
        /// <param name="modifications">A frissult adatok</param>
        /// <returns>
        /// 400-at, ha nem adtunk meg atveteli helyet,
        /// vagy az idointervallumot rosszul adtuk meg,
        /// illetve ervenytelen statuszvaltoztatas miatt
        /// (ez lehet egyenleghiany, korai berlesi kezdes, stb.).
        ///
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        ///
        /// 403-at, ha a bejelentkezett felhasznalo nem tartozik a berleshez (es nem admin).
        ///
        /// 404-et, ha nincs ilyen azonositoju berles.
        ///
        /// 409-et, ha a jarmu nem berelheto az adott idoszakban
        /// (ilyenkor a jelen, frissitendo berles nincs beleszamitva)
        ///
        /// 200-at sikeres frissites eseten.
        ///
        /// 204-et a berles torlodese eseten.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RentalDTO modifications)
        {
            if (string.IsNullOrEmpty(modifications.PickupLocation))
                return BadRequest(new { Error = "Nem javasoltál átvételi helyet!" });

            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();
            
            var existingRental = await _context.Rentals
                .Include(x => x.Renter)
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Rentals)
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Availabilities)
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingRental == null) return NotFound();

            if (existingRental.Status < RentalStatus.OfferAccepted &&
                (modifications.End <= modifications.Start ||
                modifications.Start <= _timePrv.GetUtcNow()))
                return BadRequest(new
                {
                    Error = "Nem lehet a bérlés vége előbb, mint a kezdete, illetve" +
                            "nem kezdhetsz a múltban bérlést!"
                });

            if (authUser.Role != UserRole.Administrator &&
                existingRental.RenterId != authUser.Id &&
                existingRental.Vehicle.OwnerId != authUser.Id)
                return Forbid();

            if ((existingRental.Start != modifications.Start ||
                existingRental.End != modifications.End) &&
                existingRental.Vehicle.GetQuote(modifications.Start, modifications.End, existingRental) == null)
                return Conflict(new { Error = "Nem bérelhető a jármű az adott időszakban."});

            var result = await _rentSrv.Update(existingRental, modifications, authUser);
            await _context.SaveChangesAsync();

            return result.StatusCode switch
            {
                200 => Ok(existingRental.FilterSerialize(authUser)),
                400 => BadRequest(new { Error = result.ErrorMessage }),
                204 => NoContent(),
                _ => StatusCode(500)
            };
        }

        /// <summary>
        /// Visszamondja a berlest,
        /// vagy amennyiben a felek meg az
        /// ajanlattevo idoszakban vannak, torli azt.
        /// </summary>
        /// <param name="id">A visszamondani/torolni kivant berles azonositoja.</param>
        /// <returns>
        /// 400-at, ha a berles mar aktiv vagy befejezett.
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 403-at, ha a bejelentkezett felhasznalo nem tartozik a berleshez (es nem admin).
        /// 404-et, ha nincs ilyen azonositoju berles.
        /// 200-at, ha sikeresen vissza lett mondva a berles.
        /// 204-et, ha a berlesi ajanlat meg nem lett elfogadva, es a berles torolve lett.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();
            
            var existingRental = await _context.Rentals
                .Include(x => x.Renter)
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (existingRental == null) return NotFound();

            if (authUser.Role != UserRole.Administrator &&
                existingRental.RenterId != authUser.Id &&
                existingRental.Vehicle.OwnerId != authUser.Id)
                return Forbid();

            var cancelRes = await _rentSrv.TryCancel(existingRental, authUser);
            await _context.SaveChangesAsync();
            return cancelRes.StatusCode switch
            {
                204 => NoContent(),
                400 => BadRequest(new { Error = cancelRes.ErrorMessage }),
                _ => Ok(existingRental.FilterSerialize(authUser))
            };
        }
        
        /// <summary>
        /// Lekeri az adott berleshez tartozo uzeneteket
        /// </summary>
        /// <param name="id">Az adott berles azonositoja</param>
        /// <returns>
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 403-at, ha a bejelentkezett felhasznalo nem tartozik a berleshez (es nem admin).
        /// 404-et, ha nincs ilyen azonositoju berles.
        /// 200-at + az uzeneteket maskepp.
        /// </returns>
        [HttpGet("{id}/Message")]
        public async Task<IActionResult> GetMessages(int id)
        {
            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();

            var rental = await _context.Rentals
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (rental == null) return NotFound();

            if (authUser.Role != UserRole.Administrator &&
                rental.Vehicle.OwnerId != authUser.Id &&
                rental.RenterId != authUser.Id)
                return Forbid();

            return Ok(
                (await _context.Messages
                    .AsNoTracking()
                    .IgnoreAutoIncludes()
                    .Include(x => x.Sender)
                    .Where(x => x.RentalId == rental.Id)
                    .OrderBy(x => x.TimeSent)
                    .ToListAsync())
                    .FilterSerialize(authUser)
            );
        }
        
        /// <summary>
        /// Kuld egy uzenetet a berles kontextusaban.
        /// </summary>
        /// <param name="id">A berles azonositoja</param>
        /// <param name="messageSent">A kuldott uzenet adatai</param>
        /// <returns>
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 403-at, ha a bejelentkezett felhasznalo nem tartozik a berleshez (es nem admin).
        /// 404-et, ha nincs ilyen azonositoju berles.
        /// 201-et + az uzenetet, ha sikeresen letrejott az uzenet.
        /// </returns>
        [HttpPost("{id}/Message")]
        public async Task<IActionResult> SendMessage(int id, [FromBody] MessageSendDTO messageSent)
        {
            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();

            var rental = await _context.Rentals
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (rental == null) return NotFound();

            if (authUser.Role != UserRole.Administrator &&
                rental.Vehicle.OwnerId != authUser.Id &&
                rental.RenterId != authUser.Id)
                return Forbid();

            var message = new Message
            {
                Content = messageSent.Content,
                TimeSent = _timePrv.GetUtcNow().UtcDateTime,
                Rental = rental,
                IsComplaint = messageSent.IsComplaint,
                IsImage = false,
                Sender = authUser,
            };
            
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return Created($"{Request.GetDisplayUrl()}", message.FilterSerialize(authUser));
        }
        
        /// <summary>
        /// Kuld egy kepet uzenetkent a berles kontextusaban.
        /// </summary>
        /// <param name="id">A berles azonositoja</param>
        /// <param name="file">A feltoltott kepfajl</param>
        /// <param name="isComplaint">A kep jelentes-e az adminok fele?</param>
        /// <returns>
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 403-at, ha a bejelentkezett felhasznalo nem tartozik a berleshez (es nem admin).
        /// 404-et, ha nincs ilyen azonositoju berles.
        /// 201-et + az uzenetet, ha sikeresen letrejott az uzenet.
        /// </returns>
        [HttpPost("{id}/Message/Image")]
        public async Task<IActionResult> SendMessageImage(int id, IFormFile file, [FromQuery] bool isComplaint = false)
        {
            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();

            var rental = await _context.Rentals
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (rental == null) return NotFound();

            if (authUser.Role != UserRole.Administrator &&
                rental.Vehicle.OwnerId != authUser.Id &&
                rental.RenterId != authUser.Id)
                return Forbid();

            var path = await _resSrv.Store(file);
            if (path == null) return StatusCode(500);
            
            var message = new Message
            {
                Content = path,
                TimeSent = _timePrv.GetUtcNow().UtcDateTime,
                Rental = rental,
                IsComplaint = isComplaint,
                IsImage = true,
                Sender = authUser,
            };
            
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return Created($"{Request.GetDisplayUrl()}", message.FilterSerialize(authUser));
        }

        /// <summary>
        /// Visszaadja a berleseket, amelyekben
        /// a bejelentkezett felhasznalo berbeadokent szerepel.
        /// </summary>
        /// <returns>
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 200-at + a berleseket maskepp.
        /// </returns>
        [HttpGet("Owned")]
        public async Task<IActionResult> GetOwned()
        {
            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();

            return Ok(
                (await _context.Rentals
                    .AsNoTracking()
                    .IgnoreAutoIncludes()
                    .Include(x => x.Renter)
                    .Include(x => x.Vehicle)
                    .ThenInclude(x => x.Owner)
                    .Where(x => x.Vehicle.OwnerId == authUser.Id)
                    .ToListAsync())
                .FilterSerialize(authUser)
            );
        }
    }
}
