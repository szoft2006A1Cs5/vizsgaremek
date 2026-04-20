using backend.Models;

namespace backend.DTOs.User;

public class UserModificationDTO : UserDTO
{
    public required string PreviousPassword { get; set; }
    public UserRole? Role { get; set; }
}