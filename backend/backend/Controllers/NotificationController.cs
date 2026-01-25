using backend.Auth;
using backend.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthManager _authMgr;

        public NotificationController(Context context, AuthManager authMgr)
        {
            _context = context;
            _authMgr = authMgr;
        }
        
        // GET: api/<NotificationController>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var uid = _authMgr.GetUID(User);

            if (uid == null) return Unauthorized();

            return Ok(
                await _context.Notifications
                .Where(x => x.UserId == uid)
                .ToListAsync()
            );
        }

        // GET api/<NotificationController>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var uid = _authMgr.GetUID(User);
            if (uid == null) return Unauthorized();

            var message = await _context.Notifications.FirstOrDefaultAsync(x => x.UserId == uid && x.Id == id);
            if (message == null) return NotFound();

            return Ok(message);
        }

        // POST api/<NotificationController>
        [Authorize]
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id)
        {
            var uid = _authMgr.GetUID(User);
            if (uid == null) return Unauthorized();
            
            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.UserId == uid && x.Id == id);
            if (notification == null) return NotFound();

            notification.Read = true;
            await _context.SaveChangesAsync();

            return Ok(notification);
        }

        // DELETE api/<NotificationController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var uid = _authMgr.GetUID(User);
            if (uid == null) return Unauthorized();
            
            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.UserId == uid && x.Id == id);
            if (notification == null) return NotFound();

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            
            return Ok(notification);
        }
    }
}
