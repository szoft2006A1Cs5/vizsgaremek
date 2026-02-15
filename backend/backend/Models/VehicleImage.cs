using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    [PrimaryKey(nameof(Id), nameof(VehicleId), nameof(ImagePath))]
    public class VehicleImage
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public required Vehicle Vehicle { get; set; }
        [MaxLength(512)]
        public required string ImagePath { get; set; }
    }
}
