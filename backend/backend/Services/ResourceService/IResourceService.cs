using backend.Models;

namespace backend.Services.ResourceService;

public interface IResourceService
{ 
    public Task<string?> Store(IFormFile file);
    public bool Delete(string filename);
}