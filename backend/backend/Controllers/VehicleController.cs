using backend.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/<JarmuController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Vehicles.ToListAsync());
        }

        // GET api/<JarmuController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var jarmu = await _context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);

            return jarmu != null ? Ok(jarmu) : NotFound();
        }
    }
}
