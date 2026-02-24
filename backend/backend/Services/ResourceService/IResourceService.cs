using backend.Models;

namespace backend.Services.ResourceService;

public interface IResourceService
{ 
    public Task<Resource?> Upload(IFormFile file, User authUser);
}