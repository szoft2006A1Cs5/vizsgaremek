namespace backend.DTOs.Auth;

public class RegistrationDTO
{
    public required string IdCardNumber { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string DriversLicenseNumber { get; set; }
    public DateOnly DriversLicenseDate { get; set; }
    public required string AddressZipcode { get; set; }
    public required string AddressSettlement { get; set; }
    public required string AddressStreetHouse { get; set; }
}