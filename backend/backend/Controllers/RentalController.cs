using backend.DTOs.Message;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        // GET: api/<RentalController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            throw new NotImplementedException();
            return Ok();
        }

        // GET api/<RentalController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            throw new NotImplementedException();
            return Ok();
        }

        // POST api/<RentalController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Rental value)
        {
            throw new NotImplementedException();
            return Ok();
        }

        // PUT api/<RentalController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Rental value)
        {
            throw new NotImplementedException();
            return Ok();
        }

        // DELETE api/<RentalController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            throw new NotImplementedException();
            return Ok();
        }
        
        [HttpGet("{id}/Message")]
        public async Task<IActionResult> GetMessages([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            throw new NotImplementedException();
            return Ok();
        }
        
        [HttpPost("{id}/Message")]
        public async Task<IActionResult> SendMessage([FromBody] MessageSendDTO content)
        {
            throw new NotImplementedException();
            return Ok();
        }

        [HttpGet("Offer")]
        public async Task<IActionResult> GetOffers([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            throw new NotImplementedException();
            return Ok();
        }
    }
}
