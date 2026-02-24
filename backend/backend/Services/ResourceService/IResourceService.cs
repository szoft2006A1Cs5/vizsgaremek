using backend.Models;

namespace backend.Services.ResourceService;

public interface IResourceService
{ 
    public Task<string?> Upload(IFormFile file);
}