using backend.Services;
using backend.Contexts;
using backend.Models;
using backend.Services.ResourceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Http.Extensions;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthService _authSrv;
        private readonly IResourceService _resSrv;
        
        public ResourceController(Context context, AuthService authSrv, IResourceService resSrv)
        {
            _context = context;
            _authSrv = authSrv;
            _resSrv = resSrv;
        }

        [HttpPost]
        public async Task<IActionResult> AddResources([FromForm(Name = "file")] List<IFormFile> files)
        {
            var authUser = await _authSrv.GetUser(User, _context);

            if (authUser == null) return Unauthorized();

            List<object> addedResources = new();
            
            foreach (var file in files)
            {
                var res = await _resSrv.Store(file);
                if (res == null) continue;

                addedResources.Add(new
                {
                    Path = res
                });
            }

            return Created($"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}/res/", addedResources);
        }
    }
}
