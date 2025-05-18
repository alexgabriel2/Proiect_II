using backend.Data;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.services {
    public class CarService : ICarService {
        private readonly AppDbContext _context;

        public CarService(AppDbContext context) {
            _context = context;
        }

        public async Task<List<Car>> GetCarsAsync() {
            return await _context.Cars.ToListAsync();
        }
        public async Task<Car?> GetCarByIdAsync(Guid carId) {
            return await _context.Cars.FirstOrDefaultAsync(c => c.Id == carId);
        }


        public async Task<Car> CreateCarAsync(Car car, Guid userId) {
            car.Id = Guid.NewGuid();
            car.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd");
            car.SellerId = userId; 

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }
    }
}
