using backend.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JarmuController : ControllerBase
    {
        private readonly Context _context;

        public JarmuController(Context ctx)
        {
            _context = ctx;
        }

        // GET: api/<JarmuController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Jarmuvek.ToListAsync());
        }

        // GET api/<JarmuController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var jarmu = await _context.Jarmuvek.FirstOrDefaultAsync(x => x.Id == id);

            return jarmu != null ? Ok(jarmu) : NotFound();
        }
    }
}
