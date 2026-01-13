using backend.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly Context _context;

        public VehicleController(Context ctx)
        {
            _context = ctx;
        }

        // GET: api/<VehicleController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Vehicles.ToListAsync());
        }

        // GET api/<VehicleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier);

            var vehicle = await _context.Vehicles.Include(x => x.Owner).FirstOrDefaultAsync(x => x.Id == id);

            if (vehicle == null)
                return NotFound();

            if (uid != null)
            {
                if (!int.TryParse(uid.Value, out var userId))
                    return BadRequest();

                if (vehicle.OwnerId == userId)
                    return Ok(vehicle);
            }

            return Ok(new
            {
                vehicle.Id,
                vehicle.OwnerId,
                vehicle.Manufacturer,
                vehicle.Model,
                vehicle.AvgFuelConsumption,
                vehicle.Description,
                vehicle.OdometerReading,
                vehicle.Year,
            });
        }
    }
}
