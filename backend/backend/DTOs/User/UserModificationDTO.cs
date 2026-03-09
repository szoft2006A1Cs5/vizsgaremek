using Humanizer;
using System.Text.RegularExpressions;

namespace backend.DTOs.User;

public class UserModificationDTO : UserDTO
{
    public required string PreviousPassword { get; set; }
}