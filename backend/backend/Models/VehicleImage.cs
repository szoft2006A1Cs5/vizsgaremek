using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    [PrimaryKey(nameof(VehicleId), nameof(ImagePath))]
    public class VehicleImage
    {
        public int VehicleId { get; set; }
        public required Vehicle Vehicle { get; set; }
        public required string ImagePath { get; set; }
    }
}
