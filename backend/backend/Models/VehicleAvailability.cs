using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using backend.Common;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace backend.Models
{
    public enum AvailabilityRecurrence
    {
        None = 0,
        Weekly = 1,
        Biweekly = 2,
        Monthly = 3,
    }

    [PrimaryKey(nameof(VehicleId), nameof(Id))]
    public class VehicleAvailability
    {
        [SwaggerIgnore]
        public int Id { get; set; }
        [SwaggerIgnore]
        public int VehicleId { get; set; }
        [SwaggerIgnore]
        public Vehicle? Vehicle { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        [NotMapped]
        [JsonIgnore]
        public DateInterval DateInterval
        {
            get => new DateInterval(Start, End);
        }
        
        public AvailabilityRecurrence Recurrence { get; set; }
        public int HourlyRate { get; set; }
    }
}
