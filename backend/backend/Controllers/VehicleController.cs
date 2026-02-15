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
        public async Task<IActionResult> Get([FromQuery] int limit = 30, [FromQuery] int offset = 0)
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
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            
            return Ok(vehicles.FilterVisibility(authUser));
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

            return Ok(vehicle.FilterVisibility(authUser));
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

            return Ok(vehicles.FilterVisibility(authUser));
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

            return Created($"{Request.GetDisplayUrl()}/{vehicle.Id}", vehicle.FilterVisibility(authUser));
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
            throw new NotImplementedException();
            return Ok();
        }

        [HttpGet("{vehicleId}/availability/{availabilityId}")]
        public async Task<IActionResult> GetAvailability(int vehicleId, int availabilityId)
        {
            throw new NotImplementedException();
            return Ok();
        }
        
        [HttpPut("{vehicleId}/availability/{availabilityId}")]
        public async Task<IActionResult> EditAvailability(
            int vehicleId, 
            int availabilityId, 
            [FromBody] VehicleAvailability availability
        )
        {
            throw new NotImplementedException();
            return Ok();
        }

        [HttpDelete("{vehicleId}/availability/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(
            int vehicleId,
            int availabilityId,
            [FromBody] VehicleAvailability availability
        )
        {
            throw new NotImplementedException();
            return Ok();
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
