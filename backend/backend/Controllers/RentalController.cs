using System.ComponentModel.DataAnnotations;
using backend.Contexts;
using backend.DTOs.Message;
using backend.DTOs.Rental;
using backend.Models;
using backend.Services;
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
        
        public RentalController(Context context, AuthService authSrv, IResourceService resSrv)
        {
            _context = context;
            _authSrv = authSrv;
            _resSrv = resSrv;
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
        public async Task<IActionResult> Post([FromBody] RentalDTO offer)
        {
            var authUser = await _authSrv.GetUser(User, _context);
            if (authUser == null) return Unauthorized();

            if (authUser.DriversLicenseNumber == null)
                return Forbid("Nincs megadva jogosítványszám a fiókodban!");

            var vehicle = await _context.Vehicles
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .FirstOrDefaultAsync(x => x.Id == offer.VehicleId);

            if (vehicle == null) return NotFound();
            if (vehicle.OwnerId == authUser.Id) return Forbid();

            var priceOffer = vehicle.GetPriceOffer(offer.Start, offer.End);
            if (priceOffer == null) return Conflict();

            if (authUser.Balance < priceOffer.Value.FullPrice)
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
                PickupLocation = offer.PickupLocation,
            };

            await _context.Rentals.AddAsync(rental);
            await _context.SaveChangesAsync();

            await Notification.Send(
                vehicle.OwnerId, 
                $"Új bérlési kérelem érkezett {vehicle.Manufacturer} {vehicle.Model} járművedre.",
                _context
            );

            return Ok(rental.FilterSerialize(authUser));
        }

        // PUT api/<RentalController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RentalDTO modifications)
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

            if (authUser.Role != UserRole.Administrator &&
                existingRental.RenterId != authUser.Id &&
                existingRental.Vehicle.OwnerId != authUser.Id)
                return Forbid();

            if (!existingRental.Vehicle.CheckAvailable(modifications.Start, modifications.End))
                return Conflict(new { Error = "Nem bérelhető a jármű az adott időszakban."});

            // TODO: State management
            
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
        public async Task<IActionResult> GetMessages(
            int id,
            [FromQuery, Range(1, int.MaxValue)] int limit = 10,
            [FromQuery, Range(1, int.MaxValue)] int page = 1
        )
        {
            var authUser = await _authSrv.GetUser(User, _context);
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
                    .Where(x => x.RentalId == rental.Id)
                    .OrderByDescending(x => x.TimeSent)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync())
                    .FilterSerialize(authUser)
            );
        }
        
        [HttpPost("{id}/Message")]
        public async Task<IActionResult> SendMessage(int id, [FromBody] MessageSendDTO messageSent)
        {
            var authUser = await _authSrv.GetUser(User, _context);
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
                TimeSent = DateTime.Now,
                Rental = rental,
                IsComplaint = messageSent.IsComplaint,
                IsImage = false,
                Sender = authUser,
            };
            
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return Created($"{Request.GetDisplayUrl()}", message.FilterSerialize(authUser));
        }
        
        [HttpPost("{id}/Message/Image")]
        public async Task<IActionResult> SendMessageImage(int id, IFormFile file, [FromQuery] bool isComplaint = false)
        {
            var authUser = await _authSrv.GetUser(User, _context);
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
            if (path == null) return BadRequest();
            
            var message = new Message
            {
                Content = path,
                TimeSent = DateTime.Now,
                Rental = rental,
                IsComplaint = isComplaint,
                IsImage = true,
                Sender = authUser,
            };
            
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            return Created($"{Request.GetDisplayUrl()}", message.FilterSerialize(authUser));
        }

        [HttpGet("Owned")]
        public async Task<IActionResult> GetOwned(
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
                    .Where(x => x.Vehicle.OwnerId == authUser.Id)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync())
                .FilterSerialize(authUser)
            );
        }
    }
}
