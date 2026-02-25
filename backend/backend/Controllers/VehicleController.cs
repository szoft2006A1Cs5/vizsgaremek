using backend.Services;
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
using backend.DTOs.VehicleImage;
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
        private readonly AuthService _authSrv;

        public VehicleController(Context ctx, AuthService authSrv)
        {
            _context = ctx;
            _authSrv = authSrv;
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
            [FromQuery] int? maxRate = null,
            [FromQuery] bool showOwned = false
        )
        {
            if (rentalStart != null && rentalEnd != null && rentalEnd < rentalStart)
                return BadRequest();
            
            var authUser = await _authSrv.GetUser(User, _context);

            // TODO: Lehet, hogy egy berles ativelne tobb elerhetosegen is, es igazabol csak arbeli elteres lenne,
            //       igy at kell neznunk azt hogy esetleg atlog-e tobb elerhetosegen.
            var vehicles = (await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Owner)
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Renter)
                .Include(x => x.Images)
                .Where(x => 
                    (rentalStart != null && rentalEnd != null ? 
                        !x.Rentals.Any(r => RentalStatus.OfferAccepted <= r.Status &&
                                            !(r.End < rentalStart.Value || rentalEnd.Value < r.Start)) &&
                        x.Availabilities.Any(a => a.Start <= rentalStart && rentalEnd <= a.End)
                    : true) &&
                    (manufacturer != null ? x.Manufacturer == manufacturer : true) &&
                    (model != null ? x.Model == model : true) &&
                    (year != null ? x.Year == year : true) &&
                    (settlement != null && x.Owner != null ? x.Owner.AddressSettlement == settlement : true) &&
                    (!showOwned && authUser != null ? x.OwnerId != authUser.Id : true)
                )
                .Skip(offset)
                .Take(limit)
                .Select(v => new
                {
                     v,
                     MinRate = v.Availabilities.Min(x => x.HourlyRate),
                     MaxRate = v.Availabilities.Max(x => x.HourlyRate),
                })
                .ToListAsync())
                .Select(x =>
                {
                    x.v.ExtensionData.Add("minRate", x.MinRate);
                    x.v.ExtensionData.Add("maxRate", x.MaxRate);
                    return x.v;
                });
            
            return Ok(vehicles.FilterSerialize(authUser));
        }

        // GET api/<VehicleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var authUser = await _authSrv.GetUser(User, _context);

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
        [HttpGet("Owned")]
        public async Task<IActionResult> GetOwned([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            var authUser = await _authSrv.GetUser(User, _context);

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
            var authUser = await _authSrv.GetUser(User, _context);

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
        
        [HttpGet("{id}/Availability")]
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

        [HttpPost("{vehicleId}/Availability")]
        public async Task<IActionResult> AddAvailability(int vehicleId, [FromBody] VehicleAvailability availability)
        {
            var authUser = await _authSrv.GetUser(User, _context);
            
            if (authUser == null) return Unauthorized();
            
            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AsSplitQuery()
                .Include(x => x.Availabilities)
                .FirstOrDefaultAsync(x => x.Id == vehicleId);
            
            if (vehicle == null) return NotFound();
            if (vehicle.OwnerId != authUser.Id 
                && authUser.Role != UserRole.Administrator) return Forbid();

            if (availability.End < availability.Start || availability.HourlyRate < 0)
                return BadRequest(new
                {
                    Error = "A bérelhetőség kezdete nem lehet később a végénél," +
                            " és a beállított ár nem lehet kevesebb 0-nál!"
                });
            
            if (vehicle.Availabilities.Any(x => x.DateInterval.DoesCollide(availability.DateInterval)))
                return Conflict(new { Error = "A megadott időszakra már van bérelhetőség megadva!" });

            availability.Id = vehicle.Availabilities.MaxOrZero(x => x.Id) + 1;
            availability.VehicleId = vehicle.Id;
            
            await _context.VehicleAvailabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
            
            return Created($"{Request.GetDisplayUrl()}/{vehicle.Id}", availability.FilterSerialize(authUser));
        }

        [HttpGet("{vehicleId}/Availability/{availabilityId}")]
        public async Task<IActionResult> GetAvailability(int vehicleId, int availabilityId)
        {
            var availability = await _context.VehicleAvailabilities
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.Id == availabilityId);
            
            if (availability == null) return NotFound();
            
            return Ok(availability);
        }
        
        [HttpPut("{vehicleId}/Availability/{availabilityId}")]
        public async Task<IActionResult> EditAvailability(
            int vehicleId, 
            int availabilityId, 
            [FromBody] VehicleAvailability replacement
        )
        {
            var authUser = await _authSrv.GetUser(User, _context);
            
            if (authUser == null) return Unauthorized();

            var availability = await _context.VehicleAvailabilities
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.Id == availabilityId);
            
            if (availability == null || availability.Vehicle == null) return NotFound();
            if (availability.Vehicle.OwnerId != authUser.Id &&
                authUser.Role != UserRole.Administrator) return Forbid();

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

        [HttpDelete("{vehicleId}/Availability/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(
            int vehicleId,
            int availabilityId
        )
        {
            var authUser = await _authSrv.GetUser(User, _context);

            if (authUser == null) return Unauthorized();

            var availability = await _context.VehicleAvailabilities
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.Id == availabilityId);

            if (availability == null || availability.Vehicle == null) return NotFound();
            if (availability.Vehicle.OwnerId != authUser.Id &&
                authUser.Role != UserRole.Administrator) return Forbid();

            _context.VehicleAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{vehicleId}/Image")]
        public async Task<IActionResult> GetImages(int vehicleId, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            return Ok(
                await _context.VehicleImages
                    .Where(x => x.VehicleId == vehicleId)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync()
            );
        }

        [HttpPost("{vehicleId}/Image")]
        public async Task<IActionResult> AddImage(int vehicleId, [FromBody] VehicleAddImageDTO dto)
        {
            var authUser = await _authSrv.GetUser(User, _context);

            var vehicle = await _context.Vehicles
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == vehicleId);
            
            if (vehicle == null) return NotFound();
            
            if (authUser == null) return Unauthorized();
            if (authUser.Role != UserRole.Administrator &&
                vehicle.OwnerId != authUser.Id) return Forbid();

            if (dto.Path == null) return BadRequest();
            
            var vehicleImage = new VehicleImage
            {
                Vehicle = vehicle,
                ImageId = vehicle.Images.MaxOrZero(x => x.ImageId) + 1,
                Path = dto.Path,
                SortIndex = dto.SortIndex ?? vehicle.Images.MaxOrZero(x => x.SortIndex) + 1,
            };

            await _context.VehicleImages.AddAsync(vehicleImage);
            await _context.SaveChangesAsync();
            
            return Created($"{Request.GetDisplayUrl()}/{vehicleId}", vehicleImage.FilterSerialize(authUser));
        }

        [HttpPut("{vehicleId}/Image/{imageId}")]
        public async Task<IActionResult> PutImage(int vehicleId, int imageId, [FromBody] VehicleAddImageDTO dto)
        {
            var authUser = await _authSrv.GetUser(User, _context);

            var image = await _context.VehicleImages
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.ImageId == imageId);
            
            if (image == null) return NotFound();
            
            if (authUser == null) return Unauthorized();
            if (authUser.Role != UserRole.Administrator &&
                image.Vehicle.OwnerId != authUser.Id) return Forbid();

            image.Path = string.IsNullOrWhiteSpace(dto.Path) ? image.Path : dto.Path;
            image.SortIndex = dto.SortIndex ?? image.SortIndex;
            
            await _context.SaveChangesAsync();
            
            return Ok(image.FilterSerialize(authUser));
        }
        

        [HttpDelete("{vehicleId}/Image/{imageId}")]
        public async Task<IActionResult> DeleteImage(int vehicleId, int imageId)
        {
            var authUser = await _authSrv.GetUser(User, _context);

            var image = await _context.VehicleImages
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.ImageId == imageId);
            
            if (image == null) return NotFound();
            
            if (authUser == null) return Unauthorized();
            if (authUser.Role != UserRole.Administrator &&
                image.Vehicle.OwnerId != authUser.Id) return Forbid();

            _context.VehicleImages.Remove(image);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
