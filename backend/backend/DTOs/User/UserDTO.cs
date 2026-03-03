using System.Text.RegularExpressions;

namespace backend.DTOs.User
{
    public class UserDTO
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

        public bool CheckRegex()
        {
            return Regex.IsMatch(this.Name, @"^[A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+( [A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+)+$") &&
                   Regex.IsMatch(this.IdCardNumber, @"^\d{6}[A-Z]{2}$") &&
                   Regex.IsMatch(this.DriversLicenseNumber, @"^[A-Z]{2}\d{6}$") &&
                   Regex.IsMatch(this.Email, @"^[A-z0-9.-]+@([A-z0-9-]+\.)+(com|hu)$") &&
                   Regex.IsMatch(this.Phone, @"^(36|06)(94|70|30|20)\d{7}$") &&
                   Regex.IsMatch(this.AddressZipcode, @"^\d{4}$") &&
                   (this.DateOfBirth.ToDateTime(new TimeOnly(0)).AddYears(18) <= DateTime.Now);
        }
    }
}
