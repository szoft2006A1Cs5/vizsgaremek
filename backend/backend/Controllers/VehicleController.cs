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
            var user = await _authMgr.GetUIDAndRelations(User, _context);

            var vehicles = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Owner)
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Renter)
                .Include(x => x.Images)
                .ToListAsync();

            return ControllerVisibilityFilterer.VisibilityTo(vehicles, user);
        }

        // GET api/<VehicleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _authMgr.GetUIDAndRelations(User, _context);

            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Owner)
                .Include(x => x.Availabilities)
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Renter)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (vehicle == null) return NotFound();

            return ControllerVisibilityFilterer.VisibilityTo(vehicle, user);
        }
    }
}
