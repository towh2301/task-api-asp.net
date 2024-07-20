using System.ComponentModel.DataAnnotations;
using MyApp.RefreshToken;

namespace MyApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string PasswordHash { get; set; } // This is the hashed password
        public ICollection<UserRefreshToken>? RefreshTokens { get; set; }
        public ICollection<TaskItem>? Tasks { get; set; }
    }
}
