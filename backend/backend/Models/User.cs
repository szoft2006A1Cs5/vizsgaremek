using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using backend.VisibilityFiltering;

namespace backend.Models
{
    public enum UserRole { 
        User = 0,
        Administrator = 1
    };

    public class User : IFilterable
    {
        public User() {}
        
        public int Id { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly), MaxLength(8)]
        public required string IdCardNumber { get; set; }

        [MaxLength(64)]
        public required string Name { get; set; }

        [VisibleTo(VisibilityLevel.InRelation), MaxLength(11)]
        public required string Phone { get; set; }

        [VisibleTo(VisibilityLevel.InRelation)]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(512)]
        public string? ProfilePicPath { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly), MaxLength(64)]
        public required string Email { get; set; }

        [VisibleTo(VisibilityLevel.AdminOnly)]
        public required byte[] Password { get; set; } // HASH

        [VisibleTo(VisibilityLevel.AdminOnly)]
        public required byte[] Salt { get; set; }

        [VisibleTo(VisibilityLevel.AdminOnly)]
        public UserRole Role { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly), MaxLength(10)]
        public required string DriversLicenseNumber { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public DateTime DriversLicenseDate { get; set; }

        [VisibleTo(VisibilityLevel.InRelation), MaxLength(4)]
        public required string AddressZipcode { get; set; }

        [MaxLength(64)]
        public required string AddressSettlement { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly), MaxLength(64)]
        public required string AddressStreetHouse { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public int Balance { get; set; }

        [VisibleTo(VisibilityLevel.OwnerOnly)] 
        public ICollection<Rental> Rentals { get; set; } = [];
        public ICollection<Vehicle> Vehicles { get; set; } = [];
        
        [VisibleTo(VisibilityLevel.OwnerOnly)]
        public ICollection<Notification> Notifications { get; set; } = [];


        public static Func<object?, User?, bool> GetVisibilityConditionLambda(VisibilityLevel visLevel)
        {
            switch (visLevel)
            {
                case VisibilityLevel.Public:
                    return (obj, _) => true;
                case VisibilityLevel.AdminOnly:
                    return (_, auth) => auth != null && auth.Role == UserRole.Administrator;
                case VisibilityLevel.InRelation:
                    return (obj, auth) => obj is User model && 
                                          auth != null && (model.Rentals.Any(x => x.RenterId == auth.Id || 
                                              x.Vehicle.OwnerId == auth.Id) || model.Id == auth.Id);
                case VisibilityLevel.OwnerOnly:
                    return (obj, auth) => obj is User model &&
                                          auth != null &&
                                          model.Id == auth.Id;
                default:
                    return (_, _) => false;
            }
        }
    }
}
