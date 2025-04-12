using backend.Entities;
using backend.Models;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase {
        public static User user = new User();

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(UserDto request) {

            var user = await authService.RegisterAsync(request);
            if (user == null) {
                return BadRequest("User already exists");
            }

            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginDto request) {
            var response = await authService.LoginAsync(request);
            if (response == null) {
                return BadRequest("Invalid credentials");
            }
            return Ok(response);
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request) {
            var result = await authService.RefreshTokenAsync(request);
            if(result is null || result.AccessToken is null || result.RefreshToken is null) {
                return Unauthorized("Invalid token");
            } 
            return Ok(result);
        }

        [Authorize]
        [HttpGet("AuthenticationOnlyEndpoint")]
        public IActionResult AuthenticationOnlyEndpoint() {
            return Ok("You are authenticated");
        }

        [Authorize (Roles ="Admin")]
        [HttpGet]
        public IActionResult AdminOnlyEndpoint() {
            return Ok("You are authenticated");
        }
    }
}
