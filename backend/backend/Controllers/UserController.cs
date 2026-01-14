using backend.Auth;
using backend.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var uid = _authMgr.GetUID(User);

            var user = await _context.Users.Include(x => x.Rentals).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) return NotFound();

            if (uid == id)
                return Ok(user);

            return Ok(new
            {
                user.Id,
                user.Name,
                user.ProfilePicPath,
                City = user.AddressSettlement
            });
        }
    }
}
