using backend.Entities;
using backend.Models;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(IAuthService authService) : Controller {
        public static User user = new User();

        [HttpGet("Validate")]
        public IActionResult AuthenticationOnlyEndpoint() {
            return Ok(true);
        }

        [HttpGet("GetInfo")]
        public async Task<ActionResult<UserDTO>> GetInfo() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                return Unauthorized("Invalid token");
            }
            var user = await authService.GetUserByIdAsync(userId);
            if (user == null) {
                return NotFound("User not found");
            }
            return Ok(user);
        }
       
        [HttpPut("UpdateInfo")]
        public async Task<ActionResult<UserDTO>> UpdateInfo(UserDTO userDto) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                return Unauthorized("Invalid token");
            }
            var user = await authService.GetUserByIdAsync(userId);
            if (user == null) {
                return NotFound("User not found");
            }
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;
            await authService.UpdateUserAsync(user, userId);
            return Ok(user);
        }

        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePassword)
        {
            var errorsValidation = changePassword.checkValidation();
            if (errorsValidation.Count > 0)
            {
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
