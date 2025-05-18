using backend.Entities;
using backend.Models;

namespace backend.services {
    public interface ICarService {
        Task<List<Car>> GetCarsAsync();
        Task<Car> CreateCarAsync(CarAddDTO car, Guid userId, byte[]? imageData);
        Task<Car?> GetCarByIdAsync(Guid carId);

    }
}
