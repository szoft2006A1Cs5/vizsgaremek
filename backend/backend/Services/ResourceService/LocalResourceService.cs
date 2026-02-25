using backend.Models;
using FileTypeChecker;
using FileTypeChecker.Extensions;
using System.Security.Cryptography;

namespace backend.Services.ResourceService;

public class LocalResourceService : IResourceService
{
    private readonly IWebHostEnvironment _webHostEnv;

    public LocalResourceService(IWebHostEnvironment webHostEnv)
    {
        _webHostEnv = webHostEnv;
    }
    
    public async Task<string?> Store(IFormFile formFile)
    {
        string filename;

        await using (var stream = formFile.OpenReadStream())
        {
            if (!await stream.IsImageAsync()) return null;

            if (stream.CanSeek)
                stream.Position = 0;

            filename = $"{Convert.ToHexString(await SHA512.Create().ComputeHashAsync(stream))}{Path.GetExtension(formFile.FileName)}";
            string path = Path.Combine(_webHostEnv.WebRootPath, filename);

            if (File.Exists(path)) return filename;

            if (stream.CanSeek)
                stream.Position = 0;

            await using (var file = File.Create(path))
                await stream.CopyToAsync(file);
        }

        return filename;
    }
}