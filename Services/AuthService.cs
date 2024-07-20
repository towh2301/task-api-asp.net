// src/YourProject.API/Services/AuthService.cs
using DTOs.AuthenticationResponse;
using DTOs.LoginDTO;
using DTOs.UserDTO;
using Interface.IAuthService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApp.Data;
using MyApp.Models;
using MyApp.RefreshToken;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthenticationResponse> RegisterAsync(UserDTO userDTO)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userDTO.Username))
                throw new ApplicationException("Username is already taken");

            if (await _context.Users.AnyAsync(u => u.Email == userDTO.Email))
                throw new ApplicationException("Email is already taken");

            var user = new User
            {
                Username = userDTO.Username,
                Password = userDTO.Password,
                Email = userDTO.Email,
                RefreshTokens = new List<UserRefreshToken>(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password)
            };

            _context.Users.Add(user);
            var token = GenerateJwtToken(user);
            await _context.SaveChangesAsync();

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResponse(user, token, refreshToken.Token);
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginDTO loginDto)
        {
            User? user = null;
            try
            {
                user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);
            }
            catch (System.Exception)
            {
                Console.WriteLine("");
                throw new ApplicationException("User not found with this username: " + loginDto.Username);
            }

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new ApplicationException("Username or password is incorrect");

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens ??= new List<UserRefreshToken>();
            user.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync();

            return new AuthenticationResponse(user, token, user.RefreshTokens.Last().Token);
        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshTokens != null && u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                throw new ApplicationException("Invalid token");

            var refreshToken = user.RefreshTokens?.Single(x => x.Token == token);

            if (refreshToken != null && !refreshToken.IsActive)
                throw new ApplicationException("Invalid token");

            var newRefreshToken = GenerateRefreshToken();
            if (refreshToken != null)
            {
                if (refreshToken != null)
                {
                    refreshToken.Revoked = DateTime.UtcNow;
                }
            }

            var jwtToken = GenerateJwtToken(user);
            await _context.SaveChangesAsync();

            user.RefreshTokens ??= new List<UserRefreshToken>();
            user.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResponse(user, jwtToken, newRefreshToken.Token);
        }

        public async Task RevokeTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ApplicationException("Token is required");

            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshTokens != null && u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                throw new ApplicationException("Invalid token");

            var refreshToken = user.RefreshTokens?.Single(x => x.Token == token);

            if (refreshToken != null && refreshToken.IsActive)
                throw new ApplicationException("Invalid token");

            if (refreshToken != null)
            {
                refreshToken.Revoked = DateTime.UtcNow;
            }



            await _context.SaveChangesAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id) ?? throw new ApplicationException("User not found");
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserRefreshToken GenerateRefreshToken()
        {
            using var rngCryptoServiceProvider = RandomNumberGenerator.Create();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            var currentUser = _context.Users.Find(1) ?? null;
            return new UserRefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                User = currentUser
            };
        }
    }
}