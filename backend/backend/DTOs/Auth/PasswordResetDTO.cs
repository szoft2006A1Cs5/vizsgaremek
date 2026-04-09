namespace backend.DTOs.Auth;

public class PasswordResetDTO
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string Password  { get; set; }
}