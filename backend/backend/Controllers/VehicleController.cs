using backend.Auth;
using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using backend.Common;
using backend.DTOs.Vehicle;
using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Http.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthManager _authMgr;

        public VehicleController(Context ctx, AuthManager authMgr)
        {
            _context = ctx;
            _authMgr = authMgr;
        }

        // GET: api/<VehicleController>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int limit = 30,
            [FromQuery] int offset = 0,
            [FromQuery] DateTime? rentalStart = null,
            [FromQuery] DateTime? rentalEnd = null,
            [FromQuery] string? manufacturer = null,
            [FromQuery] string? model = null,
            [FromQuery] int? year = null,
            [FromQuery] string? settlement = null,
            [FromQuery] int? minRate = null,
            [FromQuery] int? maxRate = null)
        {
            var authUser = await _authMgr.GetUser(User, _context);

            var vehicles = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Owner)
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Renter)
                .Include(x => x.Images)
                .Where(x => 
                    (rentalStart != null && rentalEnd != null ? x.CheckAvailable(new DateInterval(rentalStart.Value, rentalEnd.Value)) : true) && // TODO: nem tudja SQL-be forditani
                    (manufacturer != null ? x.Manufacturer == manufacturer : true) &&
                    (model != null ? x.Model == model : true) &&
                    (year != null ? x.Year == year : true) &&
                    (settlement != null && x.Owner != null ? x.Owner.AddressSettlement == settlement : true)
                )
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            
            return Ok(vehicles.FilterSerialize(authUser));
        }

        // GET api/<VehicleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var authUser = await _authMgr.GetUser(User, _context);

            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Owner)
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Renter)
                .Include(x => x.Images)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
                
            if (vehicle == null) return NotFound();

            return Ok(vehicle.FilterSerialize(authUser));
        }

        [Authorize(Roles = "User")]
        [HttpGet("owned")]
        public async Task<IActionResult> GetOwned([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            var authUser = await _authMgr.GetUser(User, _context);

            if (authUser == null) return Unauthorized();

            var vehicles = _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AsSplitQuery()
                .Include(x => x.Availabilities)
                .Include(x => x.Images)
                .Where(x => x.OwnerId == authUser.Id)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return Ok(vehicles.FilterSerialize(authUser));
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleDTO vehicledata)
        {
            var authUser = await _authMgr.GetUser(User, _context);

            if (authUser == null) return Unauthorized();

            Vehicle vehicle = new Vehicle
            {
                OwnerId = authUser.Id,
                VIN = vehicledata.VIN,
                LicensePlate = vehicledata.LicensePlate,
                Manufacturer = vehicledata.Manufacturer,
                Model = vehicledata.Model,
                Year = vehicledata.Year,
                Description = vehicledata.Description,
                OdometerReading = vehicledata.OdometerReading,
                AvgFuelConsumption = vehicledata.AvgFuelConsumption,
                InsuranceNumber = vehicledata.InsuranceNumber,
            };
            
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();

            return Created($"{Request.GetDisplayUrl()}/{vehicle.Id}", vehicle.FilterSerialize(authUser));
        }
        
        [HttpGet("{id}/availability")]
        public async Task<IActionResult> GetAvailabilities(int id, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            return Ok(
                await _context.VehicleAvailabilities
                .Where(x => x.VehicleId == id)
                .Skip(offset)
                .Take(limit)
                .ToListAsync()
            );
        }

        [HttpPost("{vehicleId}/availability")]
        public async Task<IActionResult> AddAvailability(int vehicleId, [FromBody] VehicleAvailability availability)
        {
            var authUser = await _authMgr.GetUser(User, _context);
            
            if (authUser == null) return Unauthorized();
            
            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AsSplitQuery()
                .Include(x => x.Availabilities)
                .FirstOrDefaultAsync(x => x.Id == vehicleId);
            
            if (vehicle == null) return NotFound();
            if (vehicle.OwnerId != authUser.Id) return Forbid();

            if (availability.End < availability.Start || availability.HourlyRate < 0)
                return BadRequest(new
                {
                    Error = "A bérelhetőség kezdete nem lehet később a végénél," +
                            " és a beállított ár nem lehet kevesebb 0-nál!"
                });
            
            if (vehicle.Availabilities.Any(x => x.DateInterval.DoesCollide(availability.DateInterval)))
                return Conflict(new { Error = "A megadott időszakra már van bérelhetőség megadva!" });

            availability.Id = vehicle.Availabilities.Max(x => x.Id) + 1;
            availability.VehicleId = vehicle.Id;
            
            await _context.VehicleAvailabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
            
            return Created($"{Request.GetDisplayUrl()}/{vehicle.Id}", availability.FilterSerialize(authUser));
        }

        [HttpGet("{vehicleId}/availability/{availabilityId}")]
        public async Task<IActionResult> GetAvailability(int vehicleId, int availabilityId)
        {
            var availability = await _context.VehicleAvailabilities
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.Id == vehicleId);
            
            if (availability == null) return NotFound();
            
            return Ok(availability);
        }
        
        [HttpPut("{vehicleId}/availability/{availabilityId}")]
        public async Task<IActionResult> EditAvailability(
            int vehicleId, 
            int availabilityId, 
            [FromBody] VehicleAvailability replacement
        )
        {
            var authUser = await _authMgr.GetUser(User, _context);
            
            if (authUser == null) return Unauthorized();

            var availability = await _context.VehicleAvailabilities
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.Id == availabilityId);
            
            if (availability == null || availability.Vehicle == null) return NotFound();
            if (availability.Vehicle.OwnerId != authUser.Id) return Forbid();

            if (availability.End < availability.Start || availability.HourlyRate < 0)
                return BadRequest(new
                {
                    Error = "A bérelhetőség kezdete nem lehet később a végénél," +
                            " és a beállított ár nem lehet kevesebb 0-nál!"
                });
            
            if (availability.Vehicle.Availabilities.Any(x => x.DateInterval.DoesCollide(replacement.DateInterval) &&
                                                             x.Id != availabilityId))
                return Conflict(new { Error = "A megadott időszakra már van bérelhetőség megadva!" });

            availability.Start = replacement.Start;
            availability.End = replacement.End;
            availability.Recurrence = replacement.Recurrence;
            availability.HourlyRate = replacement.HourlyRate;
            
            await _context.SaveChangesAsync();
            
            return Ok(availability.FilterSerialize(authUser));
        }

        [HttpDelete("{vehicleId}/availability/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(
            int vehicleId,
            int availabilityId
        )
        {
            var authUser = await _authMgr.GetUser(User, _context);

            if (authUser == null) return Unauthorized();

            var availability = await _context.VehicleAvailabilities
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.Id == availabilityId);

            if (availability == null || availability.Vehicle == null) return NotFound();
            if (availability.Vehicle.OwnerId != authUser.Id) return Forbid();

            _context.VehicleAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{vehicleId}/image")]
        public async Task<IActionResult> GetImages(int vehicleId, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            throw new NotImplementedException();
            return Ok();
        }

        [HttpPost("{vehicleId}/image")]
        public async Task<IActionResult> AddImage(int vehicleId, [FromBody] string imageUrl)
        {
            throw new NotImplementedException();
            return Ok();
        }

        [HttpDelete("{vehicleId}/image/{imageId}")]
        public async Task<IActionResult> DeleteImage(int vehicleId, int imageId)
        {
            throw new NotImplementedException();
        }
    }
}
