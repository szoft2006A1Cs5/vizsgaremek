using System.Text.RegularExpressions;
using backend.Models;

namespace backend.DTOs.Vehicle;

public class VehicleDTO
{
    public required string VIN { get; set; }
    public required string LicensePlate { get; set; }
    public required string Manufacturer { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public required string Description { get; set; }
    public int OdometerReading { get; set; }
    public int Horsepower { get; set; }
    public double AvgFuelConsumption { get; set; }
    public required string FuelType { get; set; }
    public required string InsuranceNumber { get; set; }
    public required string Transmission { get; set; }

    public bool CheckValid()
    {
        return Regex.IsMatch(this.VIN, "^[A-Z0-9]{17}$") &&
               Regex.IsMatch(this.LicensePlate, "^([A-Z]{4}[0-9]{3})|([A-Z]{3}[0-9]{3})$") &&
               !string.IsNullOrWhiteSpace(this.InsuranceNumber);
    }
}