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
    
    public async Task<Resource?> Upload(IFormFile formFile)
    {
        string filename;
        ResourceType? fileType = null;
        
        await using (var stream = formFile.OpenReadStream())
        {
            var extension = Path.GetExtension(formFile.FileName);
            
            if (await stream.IsImageAsync()) fileType = ResourceType.Image;
            if (await stream.IsDocumentAsync()) fileType = ResourceType.PDF;

            filename = $"{Guid.NewGuid().ToString()}{extension}";
            string path = Path.Combine(_webHostEnv.WebRootPath, filename);

            if (stream.CanSeek)
                stream.Position = 0;
            
            await using (var file = File.Create(path))
            {
                await stream.CopyToAsync(file);
            }
        }

        if (fileType == null) return null;

        return new Resource
        {
            Path = filename,
            Type = fileType.Value
        };
    }
}