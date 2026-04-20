using System.Text.RegularExpressions;
using backend.Contexts;
using backend.Models;

namespace backend.DTOs.User
{
    public class UserRegistrationDTO : UserDTO
    {
        public new required string Password { get; set; }

        public new bool CheckValid(TimeProvider timePrv)
        {
            return base.CheckValid(timePrv) &&
                   Regex.IsMatch(this.Password, @"^(?=.*[a-z])(?=.*\d)(?=.*[A-Z]).{8,}$");
        }
    }
}
