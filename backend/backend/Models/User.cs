using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public enum UserRole { 
        User = 0,
        Administrator = 1
    };

    public class User
    {
        public int Id { get; set; }
        public required string IdCardNumber { get; set; }
        public required string Name { get; set; }
        public required string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePicPath { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; } // HASH
        public UserRole Role { get; set; }
        public required string DriversLicenseNumber { get; set; }
        public DateTime DriversLicenseDate { get; set; }
        public required string AddressZipcode { get; set; }
        public required string AddressSettlement { get; set; }
        public required string AddressStreetHouse { get; set; }
        public int Balance { get; set; }
    }
}
