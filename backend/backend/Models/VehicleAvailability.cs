using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using backend.Common;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace backend.Models
{

    [PrimaryKey(nameof(Id))]
    [Index(nameof(VehicleId), nameof(AvailabilityId))]
    public class VehicleAvailability
    {
        [JsonIgnore]
        public int Id { get; set; }
        [SwaggerIgnore]
        public int VehicleId { get; set; }
        [SwaggerIgnore]
        [JsonIgnore]
        public Vehicle? Vehicle { get; set; }
        [SwaggerIgnore]
        public int AvailabilityId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        [NotMapped]
        [JsonIgnore]
        public DateInterval DateInterval
        {
            get => new DateInterval(Start, End);
        }
        
        public int HourlyRate { get; set; }
    }
}
