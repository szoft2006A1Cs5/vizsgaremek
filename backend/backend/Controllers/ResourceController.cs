using backend.Auth;
using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthManager _authMgr;
        public ResourceController(Context context, AuthManager authMgr)
        {
            _context = context;
            _authMgr = authMgr;
        }

        [HttpPost]
        public async Task<IActionResult> AddResource(IFormFile file)
        {
            var authUser = await _authMgr.GetUser(User, _context);

            if (authUser == null) return Unauthorized();

            return Ok();
        }
    }
}
