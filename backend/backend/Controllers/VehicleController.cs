using System.ComponentModel.DataAnnotations;
using backend.Services;
using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using backend.Common;
using backend.DTOs.Vehicle;
using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Http.Extensions;
using backend.Services.ResourceService;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthService _authSrv;
        private readonly IResourceService _resSrv;
        private readonly TimeProvider _timePrv;

        public VehicleController(Context ctx, AuthService authSrv, IResourceService resSrv, TimeProvider timePrv)
        {
            _context = ctx;
            _authSrv = authSrv;
            _resSrv = resSrv;
            _timePrv = timePrv;
        }

        /// <summary>
        /// Visszaadja az adatbazisban talalhato jarmuveket
        /// (opcionalisan szurve)
        /// </summary>
        /// <param name="rentalStart">Opcionalis, a berles kezdeti idopontja</param>
        /// <param name="rentalEnd">
        /// Opcionalis, a berles vegenek idopontja
        /// (mindenkeppen a berles kezdete utan kell kovetkeznie)
        /// </param>
        /// <param name="manufacturer">Opcionalis, jarmu gyarto/marka</param>
        /// <param name="model">Opcionalis, jarmu tipus/modell</param>
        /// <param name="year">Opcionalis, jarmu gyartasi eve</param>
        /// <param name="settlement">Opcionalis, a telepules ahol jarmuvet keresunk</param>
        /// <param name="fuelType">Opcionalis, jarmu uzemanyaganak tipusa</param>
        /// <param name="transmission">Opcionalis, jarmu sebessegvaltojanak tipusa</param>
        /// <param name="minRate">Opcionalis, minimum oradij</param>
        /// <param name="maxRate">Opcionalis, maximum oradij</param>
        /// <param name="minPrice">Opcionalis, minimum teljes ar</param>
        /// <param name="maxPrice">Opcionalis, maximum teljes ar</param>
        /// <param name="showOwned">
        /// Visszaadja-e a bejelentkezett felhasznalo
        /// jarmuveit? Alapvetoen false/hamis
        /// </param>
        /// <returns>(Szurt) jarmuvek</returns>
        [HttpGet]
        public async Task<IActionResult> GetVehicles(
            [FromQuery] DateTime? rentalStart = null,
            [FromQuery] DateTime? rentalEnd = null,
            [FromQuery] string? manufacturer = null,
            [FromQuery] string? model = null,
            [FromQuery] int? year = null,
            [FromQuery] string? settlement = null,
            [FromQuery] string? fuelType = null,
            [FromQuery] string? transmission = null,
            [FromQuery] int? minRate = null,
            [FromQuery] int? maxRate = null,
            [FromQuery] int? minPrice = null,
            [FromQuery] int? maxPrice = null,
            [FromQuery] bool showOwned = false
        )
        {
            var authUser = await _authSrv.GetUser(User);

            var vehicles = (await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Availabilities)
                .Include(x => x.Owner)
                .Include(x => x.Rentals)
                .Include(x => x.Images.OrderBy(y => y.SortIndex))
                .Where(x => 
                    ((rentalStart != null && rentalEnd != null && rentalStart < rentalEnd) ?
                        !x.Rentals.Any(r => RentalStatus.OfferAccepted <= r.Status &&
                                            r.Status < RentalStatus.Finished &&
                                            !(r.End < rentalStart || rentalEnd < r.Start)) &&
                        x.Availabilities.Any(a => a.Start <= rentalStart && rentalStart <= a.End) &&
                        x.Availabilities.Any(a => a.Start <= rentalEnd && rentalEnd <= a.End) &&
                        // Tsodalatos megoldas, az intervallumon belul, az utolso elerhetoseget kiveve, vegigmegyunk,
                        // es megnezzuk, hogy van-e olyan elerhetoseg utana, ami akkor kezdodik, amikor az vegez.
                        x.Availabilities
                            .Where(a => rentalStart <= a.End && a.End < rentalEnd)
                            .All(a1 => x.Availabilities.Any(a2 => a1.End == a2.Start && a1.End < a2.End))
                    : true) &&
                    (manufacturer != null ? x.Manufacturer.ToLower().Contains(manufacturer.ToLower()) : true) &&
                    (model != null ? x.Model.ToLower().Contains(model.ToLower()) : true) &&
                    (year != null ? x.Year == year : true) &&
                    (settlement != null && x.Owner != null ? 
                        x.Owner.AddressSettlement.ToLower().Contains(settlement.ToLower()) 
                    : true) &&
                    (fuelType != null ? x.FuelType.ToLower().Contains(fuelType.ToLower()) : true) &&
                    (transmission != null ? x.Transmission.ToLower().Contains(transmission.ToLower()) : true) &&
                    (!showOwned && authUser != null ? x.OwnerId != authUser.Id : true) &&
                    (minRate != null ? 
                        x.Availabilities
                            .Where(a => !(a.End < rentalStart || rentalEnd < a.Start))
                            .All(a => minRate.Value <= a.HourlyRate) 
                    : true) &&
                    (maxRate != null ?
                        x.Availabilities
                            .Where(a => !(a.End < rentalStart || rentalEnd < a.Start))
                            .All(a => a.HourlyRate <= maxRate.Value)
                    : true)
                )
                .ToListAsync())
                .Select(x =>
                {
                    var quote = x.GetQuote(rentalStart, rentalEnd);
                    if (quote != null) x.ExtensionData.Add("quote", quote);
                    
                    return x;
                })
                .Where(x => x.ExtensionData.ContainsKey("quote") && x.ExtensionData["quote"] is VehicleQuote quote ? 
                                (minPrice != null ? minPrice.Value <= quote.FullPrice : true) &&
                                (maxPrice != null ? quote.FullPrice <= maxPrice : true) 
                            : true);
            
            return Ok(vehicles.FilterSerialize(authUser));
        }

        /// <summary>
        /// Visszaadja a megadott id-ju jarmu adatait
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rentalStart">Opcionalis, a berles kezdeti idopontja</param>
        /// <param name="rentalEnd">
        /// Opcionalis, a berles vegso idopontja,
        /// nem lehet elobb, mint a kezdeti idopont
        /// </param>
        /// <returns>A megadott jarmu adatai</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(
            int id, 
            [FromQuery] DateTime? rentalStart = null,
            [FromQuery] DateTime? rentalEnd = null
        )
        {
            var authUser = await _authSrv.GetUser(User);

            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Owner)
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Renter)
                .Include(x => x.Images.OrderBy(y => y.SortIndex))
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (vehicle == null) return NotFound();

            if (rentalStart != null && rentalEnd != null && rentalStart < rentalEnd)
                vehicle.ExtensionData.Add("quote", vehicle.GetQuote(rentalStart, rentalEnd));

            return Ok(vehicle.FilterSerialize(authUser));
        }

        /// <summary>
        /// Visszaadja a bejelentkezett felhasznalo jarmuveinek adatait.
        /// </summary>
        /// <returns>
        /// 401, ha a felhasznalo nincs bejelentkezve
        /// 200 + jarmuvek listaja, ha a felhasznalo be van jelentkezve
        /// </returns>
        [Authorize(Roles = "User")]
        [HttpGet("Owned")]
        public async Task<IActionResult> GetOwnedVehicles()
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();

            var vehicles = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AsSplitQuery()
                .Include(x => x.Owner)
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Renter)
                .Include(x => x.Images.OrderBy(y => y.SortIndex))
                .Where(x => x.OwnerId == authUser.Id)
                .ToListAsync();

            return Ok(vehicles.FilterSerialize(authUser));
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] VehicleDTO vehicleData)
        {
            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();
            
            if (!vehicleData.CheckValid())
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
                Horsepower = vehicleData.Horsepower,
                AvgFuelConsumption = vehicleData.AvgFuelConsumption,
                FuelType = vehicleData.FuelType,
                Transmission = vehicleData.Transmission,
                InsuranceNumber = vehicleData.InsuranceNumber,
            };
            
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();

            return Created($"{Request.GetDisplayUrl()}/{vehicle.Id}", vehicle.FilterSerialize(authUser));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] VehicleDTO vehicleData)
        {
            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();
            
            if (!vehicleData.CheckValid())
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

        [HttpGet("{id}/Quote")]
        public async Task<IActionResult> GetQuote(
            int id,
            [FromQuery] DateTime? rentalStart = null,
            [FromQuery] DateTime? rentalEnd = null
        )
        {
            if ((rentalStart == null || rentalEnd == null) ||
                rentalEnd <= rentalStart ||
                rentalStart.Value.AddMinutes(5) <= _timePrv.GetUtcNow()) // 5 perc arbitrary,
                                                                         // csak egy kiss buffer spacenek kell
                return BadRequest();
            
            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (vehicle == null) return NotFound();
            
            var quote = vehicle.GetQuote(rentalStart, rentalEnd);
            if (quote == null) return Conflict();
            
            return Ok(quote);
        }
        
        [HttpGet("{id}/Availability")]
        public async Task<IActionResult> GetAvailabilities(int id)
        {
            var authUser = await _authSrv.GetUser(User);
            
            return Ok(
                (await _context.VehicleAvailabilities
                    .AsNoTracking()
                    .IgnoreAutoIncludes()
                    .Where(x => x.VehicleId == id)
                    .ToListAsync())
                    .FilterSerialize(authUser)
            );
        }

        [HttpPost("{vehicleId}/Availability")]
        public async Task<IActionResult> AddAvailability(int vehicleId, [FromBody] VehicleAvailability availability)
        {
            if (availability.End <= availability.Start ||
                availability.HourlyRate < 0)
                return BadRequest(new
                    {
                        Error = "A bérelhetőség kezdete nem lehet később a végénél, illetve " +
                                "a beállított ár nem lehet 0 vagy kevesebb!"
                    });

            var authUser = await _authSrv.GetUser(User);
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
            var authUser = await _authSrv.GetUser(User);
            
            var availability = await _context.VehicleAvailabilities
                .AsNoTracking()
                .IgnoreAutoIncludes()
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
            if (replacement.End <= replacement.Start ||
                replacement.HourlyRate < 0)
                return BadRequest(new
                {
                    Error = "A bérelhetőség kezdete nem lehet később a végénél, illetve " +
                            "a beállított ár nem lehet 0 vagy kevesebb!"
                });

            var authUser = await _authSrv.GetUser(User);
            if (authUser == null) return Unauthorized();

            var availability = await _context.VehicleAvailabilities
                .Include(x => x.Vehicle)
                .ThenInclude(x => x.Availabilities)
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.AvailabilityId == availabilityId);
            
            if (availability == null || availability.Vehicle == null) return NotFound();
            if (availability.Vehicle.OwnerId != authUser.Id &&
                authUser.Role != UserRole.Administrator) return Forbid();
            
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
            var authUser = await _authSrv.GetUser(User);
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
        public async Task<IActionResult> GetImages(int vehicleId)
        {
            var authUser = await _authSrv.GetUser(User);
            
            return Ok(
                (await _context.VehicleImages
                    .Where(x => x.VehicleId == vehicleId)
                    .OrderBy(x => x.SortIndex)
                    .ToListAsync())
                    .FilterSerialize(authUser)
            );
        }
        
        [HttpGet("{vehicleId}/Image/{imageId}")]
        public async Task<IActionResult> GetImageById(int vehicleId, int imageId)
        {
            var authUser = await _authSrv.GetUser(User);

            var image = await _context.VehicleImages
                .FirstOrDefaultAsync(x =>
                    x.VehicleId == vehicleId &&
                    x.ImageId == imageId
                );
            
            return Ok(image.FilterSerialize(authUser));
        }
        
        [HttpPost("{vehicleId}/Image")]
        public async Task<IActionResult> AddImage(int vehicleId, IFormFile file, [FromQuery] int? sortIndex = null)
        {
            var authUser = await _authSrv.GetUser(User);

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
                Path = path,
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
            var authUser = await _authSrv.GetUser(User);

            var image = await _context.VehicleImages
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
            var authUser = await _authSrv.GetUser(User);

            var image = await _context.VehicleImages
                .Include(x => x.Vehicle)
                .FirstOrDefaultAsync(x => x.VehicleId == vehicleId && x.ImageId == imageId);
            
            if (image == null) return NotFound();
            
            if (authUser == null) return Unauthorized();
            if (authUser.Role != UserRole.Administrator &&
                image.Vehicle.OwnerId != authUser.Id) return Forbid();

            if (!_resSrv.Delete(image.Path)) return StatusCode(500);

            _context.VehicleImages.Remove(image);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
