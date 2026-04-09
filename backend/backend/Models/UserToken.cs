using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

public enum TokenType
{
    PasswordReset,
    EmailConfirm
}

public class UserToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    [MaxLength(8)]
    public required string Token { get; set; }
    public required TokenType Type { get; set; }
    public required DateTime TimeCreated { get; set; }
}