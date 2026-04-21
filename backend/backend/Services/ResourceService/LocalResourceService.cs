using backend.Models;
using FileTypeChecker;
using FileTypeChecker.Extensions;
using System.IO;
using System.Security.Cryptography;
using backend.VisibilityFiltering;

namespace backend.Services.ResourceService;

public class LocalResourceService : IResourceService
{
    private readonly string _basePath;

    public LocalResourceService(IConfiguration config, IWebHostEnvironment webHostEnv)
    {
        _basePath = config["Resources:Local:BasePath"] ?? webHostEnv.WebRootPath;
    }

    public async Task<string?> Store(IFormFile formFile)
    {
        string filename;

        await using (var stream = formFile.OpenReadStream())
        {
            if (!await stream.IsImageAsync()) return null;

            if (stream.CanSeek) stream.Position = 0;

            string extension;
            try
            {
                extension = (await FileTypeValidator.GetFileTypeAsync(stream)).Extension;
            }
            catch
            {
                return null;
            }

            if (stream.CanSeek) stream.Position = 0;
            
            // Volt szebb... na
            string path;
            do
            {
                filename = $"{Guid.NewGuid().ToString()}.{extension}";
                path = Path.Combine(_basePath, filename);
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
        string path = Path.Combine(_basePath, filename);
        if (!Path.Exists(path)) return false;

        File.Delete(path);
        return true;
    }
}