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
using System.Text.RegularExpressions;
using backend.Common;
using backend.DTOs.Vehicle;
using backend.DTOs.VehicleImage;
using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Http.Extensions;
using backend.Services.ResourceService;
using Microsoft.CodeAnalysis.Differencing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthService _authSrv;
        private readonly IResourceService _resSrv;

        public VehicleController(Context ctx, AuthService authSrv, IResourceService resSrv)
        {
            _context = ctx;
            _authSrv = authSrv;
            _resSrv = resSrv;
        }

        // GET: api/<VehicleController>
        [HttpGet]
        public async Task<IActionResult> GetVehicles(
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
        public async Task<IActionResult> GetVehicleById(int id)
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
        public async Task<IActionResult> GetOwnedVehicles([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            var authUser = await _authSrv.GetUser(User, _context);

            if (authUser == null) return Unauthorized();

            var vehicles = _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AsSplitQuery()
                .Include(x => x.Owner)
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Renter)
                .Include(x => x.Images)
                .Where(x => x.OwnerId == authUser.Id)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return Ok(vehicles.FilterSerialize(authUser));
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] VehicleDTO vehicleData)
        {
            var authUser = await _authSrv.GetUser(User, _context);
            if (authUser == null) return Unauthorized();
            
            if (!Regex.IsMatch(vehicleData.VIN, "^[A-Z0-9]{18}$") ||
                !Regex.IsMatch(vehicleData.LicensePlate, "([A-Z]{4}[0-9]{3})|([A-Z]{3}[0-9]{3})") ||
                string.IsNullOrWhiteSpace(vehicleData.InsuranceNumber))
                return BadRequest();

            if (_context.Vehicles.Any(x => x.LicensePlate == vehicleData.LicensePlate ||
                                           x.VIN == vehicleData.VIN ||
                                           x.InsuranceNumber == vehicleData.InsuranceNumber))
                return Conflict();
            
            Vehicle vehicle = new Vehicle
            {
                OwnerId = authUser.Id,
                VIN = vehicleData.VIN,
                LicensePlate = vehicleData.LicensePlate,
                Manufacturer = vehicleData.Manufacturer,
                Model = vehicleData.Model,
                Year = vehicleData.Year,
                Description = vehicleData.Description,
                OdometerReading = vehicleData.OdometerReading,
                AvgFuelConsumption = vehicleData.AvgFuelConsumption,
                InsuranceNumber = vehicleData.InsuranceNumber,
            };
            
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();

            return Created($"{Request.GetDisplayUrl()}/{vehicle.Id}", vehicle.FilterSerialize(authUser));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] VehicleDTO vehicleData)
        {
            var authUser = await _authSrv.GetUser(User, _context);
            if (authUser == null) return Unauthorized();
            
            if (!Regex.IsMatch(vehicleData.VIN, "^[A-Z0-9]{18}$") ||
                !Regex.IsMatch(vehicleData.LicensePlate, "([A-Z]{4}[0-9]{3})|([A-Z]{3}[0-9]{3})") ||
                string.IsNullOrWhiteSpace(vehicleData.InsuranceNumber))
                return BadRequest();

            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);
            if (vehicle == null) return NotFound();

            if (vehicle.OwnerId != authUser.Id && authUser.Role != UserRole.Administrator) return Forbid();

            if (_context.Vehicles.Any(x => (x.LicensePlate == vehicleData.LicensePlate ||
                                           x.VIN == vehicleData.VIN ||
                                           x.InsuranceNumber == vehicleData.InsuranceNumber) &&
                                           x != vehicle))
                return Conflict();

            var vehicleProps = typeof(Vehicle).GetProperties();
            foreach (var prop in typeof(VehicleDTO).GetProperties())
            {
                var vehicleProp = vehicleProps
                    .FirstOrDefault(x => x.Name == prop.Name &&
                                         x.PropertyType == prop.PropertyType);
                
                if (vehicleProp != null)
                    vehicleProp.SetValue(vehicle, prop.GetValue(vehicleData));
            }

            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();

            return Ok(vehicle.FilterSerialize(authUser));
        }
        
        [HttpGet("{id}/Availability")]
        public async Task<IActionResult> GetAvailabilities(int id, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            var authUser = await _authSrv.GetUser(User, _context);
            
            return Ok(
                (await _context.VehicleAvailabilities
                    .Where(x => x.VehicleId == id)
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync())
                    .FilterSerialize(authUser)
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

            if (availability.End <= availability.Start || availability.HourlyRate <= 0)
                return BadRequest(new
                {
                    Error = "A bérelhetőség kezdete nem lehet később a végénél," +
                            " és a beállított ár nem lehet 0 vagy kevesebb!"
                });
            
            if (vehicle.Availabilities.Any(x => x.DateInterval.DoesCollide(availability.DateInterval)))
                return Conflict(new { Error = "A megadott időszakra már van bérelhetőség megadva!" });

            availability.AvailabilityId = vehicle.Availabilities.MaxOrZero(x => x.AvailabilityId) + 1;
            availability.VehicleId = vehicle.Id;
            
            await _context.VehicleAvailabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
            
            return Created($"{Request.GetDisplayUrl()}/{vehicle.Id}", availability.FilterSerialize(authUser));
        }

        [HttpGet("{vehicleId}/Availability/{availabilityId}")]
        public async Task<IActionResult> GetAvailability(int vehicleId, int availabilityId)
        {
            var authUser = await _authSrv.GetUser(User, _context);
            
            var availability = await _context.VehicleAvailabilities
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.AvailabilityId == availabilityId);
            
            if (availability == null) return NotFound();
            
            return Ok(availability.FilterSerialize(authUser));
        }
        
        [HttpPut("{vehicleId}/Availability/{availabilityId}")]
        public async Task<IActionResult> UpdateAvailability(
            int vehicleId, 
            int availabilityId, 
            [FromBody] VehicleAvailability replacement
        )
        {
            var authUser = await _authSrv.GetUser(User, _context);
            if (authUser == null) return Unauthorized();

            var availability = await _context.VehicleAvailabilities
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Availabilities)
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.AvailabilityId == availabilityId);
            
            if (availability == null || availability.Vehicle == null) return NotFound();
            if (availability.Vehicle.OwnerId != authUser.Id &&
                authUser.Role != UserRole.Administrator) return Forbid();

            if (replacement.End <= replacement.Start || replacement.HourlyRate < 0)
                return BadRequest(new
                {
                    Error = "A bérelhetőség kezdete nem lehet később a végénél," +
                            " és a beállított ár nem lehet 0 vagy kevesebb!"
                });
            
            if (availability.Vehicle.Availabilities.Any(x => x.DateInterval.DoesCollide(replacement.DateInterval) &&
                                                             x.AvailabilityId != availabilityId))
                return Conflict(new { Error = "A megadott időszakra már van bérelhetőség megadva!" });

            availability.Start = replacement.Start;
            availability.End = replacement.End;
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
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.AvailabilityId == availabilityId);

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
            var authUser = await _authSrv.GetUser(User, _context);
            
            return Ok(
                (await _context.VehicleImages
                    .Where(x => x.VehicleId == vehicleId)
                    .Skip(offset)
                    .Take(limit)
                    .OrderBy(x => x.SortIndex)
                    .ToListAsync())
                    .FilterSerialize(authUser)
            );
        }
        
        [HttpPost("{vehicleId}/Image")]
        public async Task<IActionResult> AddImage(int vehicleId, IFormFile file, [FromQuery] int? sortIndex = null)
        {
            var authUser = await _authSrv.GetUser(User, _context);

            var vehicle = await _context.Vehicles
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == vehicleId);

            if (vehicle == null) return NotFound();

            if (authUser == null) return Unauthorized();
            if (authUser.Role != UserRole.Administrator &&
                vehicle.OwnerId != authUser.Id) return Forbid();

            var path = await _resSrv.Store(file);
            if (path == null) return BadRequest();

            var vehicleImage = new VehicleImage
            {
                Vehicle = vehicle,
                ImageId = vehicle.Images.MaxOrZero(x => x.ImageId) + 1,
                Path = $"res/{path}",
                SortIndex = sortIndex ?? vehicle.Images.MaxOrZero(x => x.SortIndex) + 1,
            };

            await _context.VehicleImages.AddAsync(vehicleImage);
            await _context.SaveChangesAsync();

            return Created(
                $"{Request.GetDisplayUrl()}/{vehicleImage.ImageId}",
                vehicleImage.FilterSerialize(authUser)
            );
        }

        [HttpPut("{vehicleId}/Image/{imageId}")]
        public async Task<IActionResult> UpdateImage(int vehicleId, int imageId, [FromBody] int? sortIndex = null)
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

            image.SortIndex = sortIndex ?? image.SortIndex;
            
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

            if (!_resSrv.Delete(image.Path.Replace("res/", ""))) return StatusCode(500);

            _context.VehicleImages.Remove(image);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
