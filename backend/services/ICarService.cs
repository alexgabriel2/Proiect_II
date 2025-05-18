using backend.Entities;

namespace backend.services {
    public interface ICarService {
        Task<List<Car>> GetCarsAsync();
        Task<Car> CreateCarAsync(Car car, Guid userId);
        Task<Car?> GetCarByIdAsync(Guid carId);

    }
}
