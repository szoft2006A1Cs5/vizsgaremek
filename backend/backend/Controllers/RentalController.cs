using backend.Contexts;
using backend.DTOs.Message;
using backend.Models;
using backend.Services;
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
        public async Task<IActionResult> Get([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            throw new NotImplementedException();
            return Ok();
        }

        // GET api/<RentalController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            throw new NotImplementedException();
            return Ok();
        }

        // POST api/<RentalController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Rental rentalRequest)
        {
            var authUser = await _authSrv.GetUser(User, _context);
            if (authUser == null) return Unauthorized();

            if (authUser.DriversLicenseNumber == null) return Forbid();
            
            var rentedVehicle = await _context.Vehicles
                .Include(x => x.Rentals)
                .Include(x => x.Availabilities)
                .FirstOrDefaultAsync(x => x.Id == rentalRequest.VehicleId);
            
            if (rentedVehicle == null ||
                authUser.Id == rentedVehicle.OwnerId) 
                return BadRequest();

            if (!rentedVehicle.CheckAvailable(rentalRequest.Start, rentalRequest.End))
                return Conflict(new { Error = "Nem bérelhető a jármű az adott időszakban."});
            
            rentalRequest.RenterId = authUser.Id;
            rentalRequest.Status = RentalStatus.RenterOffer;
            
            // TODO: PAYMENT CALCULATION
            
            return Ok();
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
