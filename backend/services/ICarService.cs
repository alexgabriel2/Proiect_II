using backend.Entities;
using backend.Models;

namespace backend.services {
    public interface ICarService {
        Task<List<Car>> GetCarsAsync();
        Task<Car> CreateCarAsync(CarAddDTO car, Guid userId, byte[]? imageData);
        Task<Car?> GetCarByIdAsync(Guid carId);
        Task<bool> UpdateCarAsync(Guid id, CarAddDTO updatedCar, byte[]? imageData);
        Task<bool> DeleteCarAsync(Guid carId);

    }
}
