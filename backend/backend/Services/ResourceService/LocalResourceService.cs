using backend.Models;
using FileTypeChecker;
using FileTypeChecker.Extensions;
using System.IO;
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

            // Volt szebb... na
            string path;
            do
            {
                filename = $"{Guid.NewGuid().ToString()}{Path.GetExtension(formFile.FileName)}";
                path = Path.Combine(_webHostEnv.WebRootPath, filename);
            } while (File.Exists(path));

            if (stream.CanSeek)
                stream.Position = 0;

            await using (var file = File.Create(path))
                await stream.CopyToAsync(file);
        }

        return filename;
    }

    public bool Delete(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename)) return false;
        string path = Path.Combine(_webHostEnv.WebRootPath, filename);
        if (!Path.Exists(path)) return false;

        File.Delete(path);
        return true;
    }
}