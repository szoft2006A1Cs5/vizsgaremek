using System.ComponentModel.DataAnnotations;
using backend.Contexts;
using backend.DTOs.Message;
using backend.Models;
using backend.Services;
using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Http;
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
        
        public RentalController(Context context, AuthService authSrv)
        {
            _context = context;
            _authSrv = authSrv;
        }
        
        // GET: api/<RentalController>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery, Range(1, int.MaxValue)] int limit = 10,
            [FromQuery, Range(1, int.MaxValue)] int page = 1
        )
        {
            var authUser = await _authSrv.GetUser(User, _context);
            if (authUser == null) return Unauthorized();

            return Ok(
                (await _context.Rentals
                    .AsNoTracking()
                    .IgnoreAutoIncludes()
                    .Include(x => x.Renter)
                    .Include(x => x.Vehicle)
                    .ThenInclude(x => x.Owner)
                    .Where(x => x.RenterId == authUser.Id)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync())
                    .FilterSerialize(authUser)
            );
        }

        // GET api/<RentalController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var authUser = await _authSrv.GetUser(User, _context);
            if (authUser == null) return Unauthorized();
            
            var rental = await _context.Rentals
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Owner)
                .Include(x => x.Renter)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (rental == null) return NotFound();
            
            if (rental.Vehicle.OwnerId != authUser.Id &&
                rental.RenterId != authUser.Id &&
                authUser.Role != UserRole.Administrator) 
                return Forbid();

            return Ok(rental.FilterSerialize(authUser));
        }

        // POST api/<RentalController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Rental offer)
        {
            var authUser = await _authSrv.GetUser(User, _context);
            if (authUser == null) return Unauthorized();

            if (authUser.DriversLicenseNumber == null)
                return Forbid("Nincs megadva jogosítványszám a fiókodban!");

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x => x.Id == offer.VehicleId);

            if (vehicle == null) return NotFound();
            if (vehicle.OwnerId == authUser.Id) return Forbid();

            var priceOffer = vehicle.GetPriceOffer(offer.Start, offer.End);
            if (priceOffer == null) return Conflict();

            if (authUser.Balance < priceOffer.Value.RentalPrice + (priceOffer.Value.RentalPrice * 0.05))
                return BadRequest();
           
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

            await Notification.Send(
                vehicle.OwnerId, 
                $"Új bérlési kérelem érkezett {vehicle.Manufacturer} {vehicle.Manufacturer} járművedre.",
                _context
            );

            return Ok(rental.FilterSerialize(authUser));
        }

        // PUT api/<RentalController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Rental modifications)
        {
            var authUser = await _authSrv.GetUser(User, _context);
            if (authUser == null) return Unauthorized();
            
            var existingRental = await _context.Rentals
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Rentals)
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Availabilities)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingRental == null) return NotFound();

            if (existingRental.RenterId != authUser.Id &&
                existingRental.Vehicle.OwnerId != authUser.Id)
                return Forbid();

            if (!existingRental.Vehicle.CheckAvailable(modifications.Start, modifications.End))
                return Conflict(new { Error = "Nem bérelhető a jármű az adott időszakban."});

            existingRental.Update(modifications, authUser);
            
            return Ok();
        }

        // DELETE api/<RentalController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            throw new NotImplementedException();
            return Ok();
        }
        
        [HttpGet("{id}/Message")]
        public async Task<IActionResult> GetMessages([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            throw new NotImplementedException();
            return Ok();
        }
        
        [HttpPost("{id}/Message")]
        public async Task<IActionResult> SendMessage([FromBody] MessageSendDTO content)
        {
            throw new NotImplementedException();
            return Ok();
        }
        
        [HttpPost("{id}/Message/Image")]
        public async Task<IActionResult> SendMessageImage(IFormFile file, [FromQuery] bool isComplaint = false)
        {
            throw new NotImplementedException();
            return Ok();
        }

        [HttpGet("Offer")]
        public async Task<IActionResult> GetOffers([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            throw new NotImplementedException();
            return Ok();
        }
    }
}
