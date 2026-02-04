using backend.Auth;
using backend.Contexts;
using backend.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using backend.Models;
using backend.VisibilityFiltering;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthManager _authMgr;

        public UserController(Context ctx, AuthManager authMgr)
        {
            _context = ctx;
            _authMgr = authMgr;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var authUser = await _authMgr.GetUser(User, _context);

            var user = await _context.Users
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Vehicle)
                .ThenInclude(x => x.Owner)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Images)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Availabilities)
                .AsSplitQuery()
                .Where(x => x.Id == id)
                .FilterVisibility(_context, authUser)!
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();

            /*   return new ContentResult
           {
               StatusCode = 404,
               ContentType = "application/json"
           };*/

            //return ControllerVisibilityFilterer.VisibilityTo(user, authUser, 200);
            return Ok(user);
        }
    }
}
