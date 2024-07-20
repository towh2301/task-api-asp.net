using MyApp.Models;
using MyApp.RefreshToken;

namespace DTOs.UserDTO
{
    public class UserDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public string? PasswordHash { get; set; }

    }
}