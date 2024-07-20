using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DTOs.LoginDTO;
using DTOs.UserDTO;
using Interface.IAuthService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApp.Data;
using MyApp.Models;
using MyApp.RefreshToken;

namespace Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            await _authService.RegisterAsync(userDTO);
            return Ok(new { message = "User registered successfully", StatusCode = 200 });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await _authService.LoginAsync(loginDTO);
            return Ok(response);
        }

        // Logout isn't tested, maybe it's not working
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            await _authService.RevokeTokenAsync(refreshToken ?? throw new ApplicationException("Refresh token not found"));
            return Ok(new { message = "Token revoked successfully", StatusCode = 200 });
        }

    }
}
