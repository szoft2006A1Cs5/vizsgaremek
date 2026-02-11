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
        public async Task<IActionResult> Get()
        {
            //var user = await _authMgr.GetUIDAndRelations(User, _context);
            var authUser = await _authMgr.GetUser(User, _context);
            
            var vehicles = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Owner)
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Renter)
                .Include(x => x.Images)
                .FilterVisibility(_context, authUser)!
                .ToListAsync();

            //return ControllerVisibilityFilterer.VisibilityTo(vehicles, user, 200);
            return Ok(vehicles);
        }

        // GET api/<VehicleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var authUser = await _authMgr.GetUser(User, _context);
            //var user = await _authMgr.GetUIDAndRelations(User, _context);
            
            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Owner)
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Renter)
                .Include(x => x.Images)
                .Where(x => x.Id == id)
                .FilterVisibility(_context, authUser)!
                .FirstOrDefaultAsync();

            if (vehicle == null) return NotFound();

            return Ok(vehicle);
            //return ControllerVisibilityFilterer.VisibilityTo(vehicle, user, 200);
        }

        [Authorize(Roles = "User")]
        [HttpGet("owned")]
        public async Task<IActionResult> GetOwned()
        {
            //var user = await _authMgr.GetUIDAndRelations(User, _context);
            var user = await _authMgr.GetUser(User, _context);

            if (user == null) return Unauthorized();

            var vehicles = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .AsSplitQuery()
                .Include(x => x.Availabilities)
                .Include(x => x.Images)
                .Where(x => x.OwnerId == user.Id)
                .FilterVisibility(_context, user)!
                .ToListAsync();

            //return ControllerVisibilityFilterer.VisibilityTo(vehicles, user, 200);
            return Ok(vehicles);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleDTO vehicledata)
        {
            var authUser = await _authMgr.GetUIDAndRelations(User, _context);

            if (authUser == null) return Unauthorized();

            Vehicle vehicle = new Vehicle
            {
                Id = vehicledata.Id,
                OwnerId = authUser.Item1,
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

            return Created($"{Request.GetDisplayUrl()}/{vehicle.Id}", vehicle);
        }
    }
}
