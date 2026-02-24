using backend.Services;
using backend.Contexts;
using backend.Models;
using backend.Services.ResourceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.VisibilityFiltering;

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

            List<Resource> addedResources = new();
            
            foreach (var file in files)
            {
                var res = await _resSrv.Upload(file, authUser);
                if (res == null) continue;
                
                await _context.Resources.AddAsync(res);
                await _context.SaveChangesAsync();
                
                addedResources.Add(res);
            }

            return Ok(addedResources.FilterSerialize(authUser));
        }
    }
}
