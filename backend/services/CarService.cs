using backend.Data;
using backend.Entities;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.services
{
    public class CarService : ICarService
    {
        private readonly AppDbContext _context;

        public CarService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetCarsAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        public async Task<Car?> GetCarByIdAsync(Guid carId)
        {
            return await _context.Cars.FirstOrDefaultAsync(c => c.Id == carId);
        }


        public async Task<Car> CreateCarAsync(CarAddDTO car, Guid userId, byte[]? imageData)
        {
            var newCar = new Car
            {
                Id = Guid.NewGuid(),
                SellerId = userId,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                Milleage = car.Milleage,
                Price = car.Price,
                FuelType = car.FuelType,
                Transmission = car.Transmission,
                Description = car.Description,
                Status = car.Status,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Image = imageData
            };

            _context.Cars.Add(newCar);
            await _context.SaveChangesAsync();
            return newCar;
        }

        public async Task<bool> UpdateCarAsync(Guid id, CarAddDTO updatedCar, byte[]? imageData)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return false;

            car.Make = updatedCar.Make;
            car.Model = updatedCar.Model;
            car.Year = updatedCar.Year;
            car.Milleage = updatedCar.Milleage;
            car.Price = updatedCar.Price;
            car.FuelType = updatedCar.FuelType;
            car.Status = updatedCar.Status;

            if (imageData != null)
                car.Image = imageData;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCarAsync(Guid carId)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == carId);
            if (car == null)
            {
                return false;
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
