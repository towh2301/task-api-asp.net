using MyApp.Models;
namespace MyApp.RefreshToken
{

    // Refresh Token Model is used to store refresh tokens in the database
    // This is used to authenticate users and provide them with a new access token
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        // public required string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        // public required string RevokedByIp { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}