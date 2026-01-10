using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    public enum AvailabilityRecurrence
    {
        None = 0,
        Weekly = 1,
        Biweekly = 2,
        Monthly = 3,
    }

    [PrimaryKey(nameof(VehicleId), nameof(Start), nameof(End))]
    public class VehicleAvailability
    {
        public int VehicleId { get; set; }
        public required Vehicle Vehicle { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public AvailabilityRecurrence Recurrence { get; set; }
        public int HourlyRate { get; set; }
    }
}
