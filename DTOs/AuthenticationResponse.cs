// src/YourProject.API/DTOs/AuthenticationResponse.cs
using MyApp.Models;

namespace DTOs.AuthenticationResponse
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public AuthenticationResponse(User user, string token, string refreshToken)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}