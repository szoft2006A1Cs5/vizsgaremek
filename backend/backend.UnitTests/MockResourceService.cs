using backend.Services.ResourceService;
using Microsoft.AspNetCore.Http;

namespace backend.UnitTests;

public class MockResourceService : IResourceService
{
    public async Task<string?> Store(IFormFile formFile)
    {
        return $"{Guid.NewGuid().ToString()}{Path.GetExtension(formFile.FileName)}";
    }

    public bool Delete(string filename) => true;
}