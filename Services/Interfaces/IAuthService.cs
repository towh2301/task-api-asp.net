// src/YourProject.API/Services/Interfaces/IAuthService.cs
using DTOs.AuthenticationResponse;
using DTOs.LoginDTO;
using DTOs.UserDTO;
using MyApp.Models;

namespace Interface.IAuthService
{
    public interface IAuthService
    {
        Task<AuthenticationResponse> RegisterAsync(UserDTO userDto);
        Task<AuthenticationResponse> LoginAsync(LoginDTO loginDto);
        Task<AuthenticationResponse> RefreshTokenAsync(string token);
        Task RevokeTokenAsync(string token);
        // Get user by Id
        Task<User> GetByIdAsync(int id);
    }
}