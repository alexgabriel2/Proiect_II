using backend.Entities;
using backend.Migrations;
using backend.Models;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(IAuthService authService) : ControllerBase {
        public static User user = new User();

        [HttpGet("Validate")]
        public IActionResult AuthenticationOnlyEndpoint() {
            return Ok(true);
        }

        [HttpGet("UserContact")]
        public async Task<ActionResult<string>> GetUserContact(string userId) {
           
            if (userId == null) {
                return Unauthorized("Invalid token");
            }
            var user = await authService.GetUserByIdAsync(userId);
            if (user == null) {
                return NotFound("User not found");
            }
           
            return Ok(user.PhoneNumber);
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
            user.Username = userDto.Username;
            await authService.UpdateUserAsync(user, userId);
            return Ok(user);
        }
        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePassword)
        {
            var errorsValidation = changePassword.checkValidation();
            if (errorsValidation.Count > 0)
            {
                return BadRequest(errorsValidation); // <-- asta trimite o listă de string-uri
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
                return Ok(new { message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        ///////////////
        //FAVORITE
        ///////////////
        [HttpPost("Favorite/Add")]
        public async Task<IActionResult> SaveCar([FromBody] AddFavoriteDto dto) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                return Unauthorized("Invalid token");
            }
            var user = await authService.GetUserByIdAsync(userId);
            if (user == null) {
                return NotFound("User not found");
            }

            var favorite = await authService.AddToFavorite(dto.CarId, userId);
            if (favorite == null) {
                return BadRequest("Failed to add to favorites");
            }

            return Ok("Saved");
        }

        [HttpGet("Favorite/Get")]
        public async Task<ActionResult<List<CarCardDTO>>> GetFavorite() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                return Unauthorized("Invalid token");
            }
            var user = await authService.GetUserByIdAsync(userId);
            if (user == null) {
                return NotFound("User not found");
            }
            var favorites = await authService.GetFavorites(userId);
            if (favorites == null) {
                return BadRequest("Failed to get favorites");
            }
            return Ok(favorites);
        }
        [HttpDelete("Favorite/Delete")]
        public async Task<IActionResult> DeleteFavorite(string carId) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                return Unauthorized("Invalid token");
            }
            var user = await authService.GetUserByIdAsync(userId);
            if (user == null) {
                return NotFound("User not found");
            }
            var result = await authService.DeleteFavorite(carId, userId);
            if (!result) {
                return BadRequest("Failed to delete favorite");
            }
            return Ok("Deleted");
        }
    }
}
