using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace backend.Models
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(VehicleId), nameof(ImageId))]
    public class VehicleImage
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public required Vehicle Vehicle { get; set; }
        public int ImageId { get; set; }
        public int SortIndex { get; set; }
        [MaxLength(2048)]
        public required string Path { get; set; }
    }
}
