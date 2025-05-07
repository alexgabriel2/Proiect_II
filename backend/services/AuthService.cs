using Azure.Core;
using backend.Data;
using backend.Entities;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace backend.services {
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService {

        public async Task<TokenResponseDto?> LoginAsync(LoginDto request) {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
            if (user == null) {
                return null;
            }

            if (new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, request.Password)
                    == PasswordVerificationResult.Failed) {
                return null;
            }
            
            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user) {
            return new TokenResponseDto() {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };
        }

        public async Task<TokenResponseDto?> RegisterAsync(RegisterDto request) {

            
            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                 .HashPassword(user, request.Password);
            user.Username = request.Username;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;

            user.PasswordHash = hashedPassword;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return await CreateTokenResponse(user);
        }

        private string GenerateRefreshToken() {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);

        }

        private async Task<string> GenerateAndSaveRefreshToken(User user) {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private string CreateToken(User user) {
            var Claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    configuration.GetValue<string>("AppSettings:Token")!
                    )
                );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                   issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                   audience: configuration.GetValue<string>("AppSettings:Audience"),
                   claims: Claims,
                   expires: DateTime.Now.AddMinutes(15),
                   signingCredentials: creds
               );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken) {
            var user = await context.Users.FindAsync(userId);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.Now) {
                return null;
            }
            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request) {
            var user=await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null) {
                return null;
            }
            return await CreateTokenResponse(user);
        }
        public async Task<List<string>> checkExisting(RegisterDto user) {
            var errors = new List<string>();
            if (await context.Users.AnyAsync(x => x.Username == user.Username)) {
                errors.Add("Username already exists.");
            }
            if (await context.Users.AnyAsync(x => x.Email == user.Email)) {
                errors.Add("Email already exists.");
            }
            return errors;
        }

        public async Task<UserDTO?> GetUserByIdAsync(string userId) {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id.ToString() == userId);
            if (user == null) {
                return null;
            }
            return new UserDTO() {
                Id = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task UpdateUserAsync(UserDTO user, string userId) {
            var existingUser = await context.Users.FirstOrDefaultAsync(x => x.Id.ToString() == userId);
            if (existingUser == null) {
                throw new Exception("User not found");
            }
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            context.Users.Update(existingUser);
            await context.SaveChangesAsync();

        }
    }
}
