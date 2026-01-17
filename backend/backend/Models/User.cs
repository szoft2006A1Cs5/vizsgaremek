using System.ComponentModel.DataAnnotations.Schema;
using backend.Serialization;

namespace backend.Models
{
    public enum UserRole { 
        User = 0,
        Administrator = 1
    };

    public class User
    {
        [VisibilityKey]
        public int Id { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public required string IdCardNumber { get; set; }

        public required string Name { get; set; }

        [VisibleTo(VisibilityLevel.InRelation)]
        public required string Phone { get; set; }

        [VisibleTo(VisibilityLevel.InRelation)]
        public DateTime DateOfBirth { get; set; }

        public string? ProfilePicPath { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public required string Email { get; set; }

        [VisibleTo(VisibilityLevel.AdminOnly)]
        public required byte[] Password { get; set; } // HASH

        [VisibleTo(VisibilityLevel.AdminOnly)]
        public required byte[] Salt { get; set; }

        [VisibleTo(VisibilityLevel.AdminOnly)]
        public UserRole Role { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public required string DriversLicenseNumber { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public DateTime DriversLicenseDate { get; set; }

        [VisibleTo(VisibilityLevel.InRelation)]
        public required string AddressZipcode { get; set; }

        public required string AddressSettlement { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public required string AddressStreetHouse { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public int Balance { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public ICollection<Rental> Rentals { get; set; } = [];
        public ICollection<Vehicle> Vehicles { get; set; } = [];
    }
}
