using backend.Models;
using FileTypeChecker;
using FileTypeChecker.Extensions;

namespace backend.Services.ResourceService;

public class LocalResourceService : IResourceService
{
    private readonly IWebHostEnvironment _webHostEnv;

    public LocalResourceService(IWebHostEnvironment webHostEnv)
    {
        _webHostEnv = webHostEnv;
    }
    
    public async Task<Resource?> Upload(IFormFile formFile, User authUser)
    {
        string filename;
        
        await using (var stream = formFile.OpenReadStream())
        {
            if (!await stream.IsImageAsync()) return null;

            filename = $"{Guid.NewGuid().ToString()}{Path.GetExtension(formFile.FileName)}";
            string path = Path.Combine(_webHostEnv.WebRootPath, filename);

            if (stream.CanSeek)
                stream.Position = 0;
            
            await using (var file = File.Create(path))
                await stream.CopyToAsync(file);
        }

        return new Resource
        {
            Path = filename,
            UserId = authUser.Id
        };
    }
}