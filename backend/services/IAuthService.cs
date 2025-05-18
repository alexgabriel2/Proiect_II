using backend.Entities;
using backend.Models;

namespace backend.services {
    public interface IAuthService {
        Task<TokenResponseDto?> RegisterAsync(RegisterDto request);
        Task<TokenResponseDto?> LoginAsync(LoginDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<List<string>> checkExisting(RegisterDto user);
        Task<UserDTO?> GetUserByIdAsync(string userId);
        Task UpdateUserAsync(UserDTO user,string userID);
        Task<Favorite?> AddToFavorite(string carId, string userId);
        Task<List<CarCardDTO>> GetFavorites(string userId);
        Task<bool> DeleteFavorite(string v, string userId);
    }
}
