using backend.Entities;
using backend.Models;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase {
        public static User user = new User();

        [HttpPost("Register")]
        public async Task<ActionResult<TokenResponseDto>> Register(RegisterDto request) {
            if (request.checkValidation().Count > 0) {
                return BadRequest("Please try again");
            }
            var existingUser = await authService.checkExisting(request);
            if (existingUser.Count > 0) {
                return BadRequest(existingUser);
            }
            var token = await authService.RegisterAsync(request);
            return Ok(token);
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
            if (result is null || result.AccessToken is null || result.RefreshToken is null) {
                return Unauthorized("Invalid token");
            }
            return Ok(result);
        }

        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword( ChangePasswordDto changePassword)
        {
            var errorsValidation = changePassword.checkValidation();
            if (errorsValidation.Count > 0) { 
                return BadRequest(errorsValidation); 
            }
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Invalid token.");
            }
            try
            {
                var result = await authService.ChangePasswordAsync(changePassword, username);
                if (!result)
                {
                    return BadRequest("Password could not be changed.");
                }
                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
      
    }
}
