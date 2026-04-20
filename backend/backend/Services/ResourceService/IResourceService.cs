using backend.Models;

namespace backend.Services.ResourceService;

public interface IResourceService
{ 
    /// <summary>
    /// Eltarolja a fajlt, es visszaadja az eleresi utvonalat
    /// </summary>
    /// <param name="file">A tarolando fajl</param>
    /// <returns>Az eleresi utvonal, amennyiben sikeres</returns>
    public Task<string?> Store(IFormFile file);
    
    /// <summary>
    /// Kitorli a megadott eleresi ut alatt talalhato
    /// fajlt.
    /// </summary>
    /// <param name="filename">Az eleresi ut, amelyen a fajl keresendo.</param>
    /// <returns>True, ha sikeresen torlodott, false, ha nem</returns>
    public bool Delete(string filename);
}