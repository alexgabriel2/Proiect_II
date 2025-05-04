using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Cars : ControllerBase
    {
        [HttpGet]
        public ActionResult <List<Car>> GetCars()
        {
            var cars = new List<Car>
            {
                new Car
                {
                    Id = Guid.NewGuid(),
                    SellerId = Guid.NewGuid(),
                    Make = "Toyota",
                    Model = "Corolla",
                    Year = 2020,
                    Milleage = 15000,
                    Price = 20000,
                    FuelType = "Petrol",
                    Transmission = "Automatic",
                    Description = "A reliable car with great fuel efficiency.",
                    Status = "Available",
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd")
                }
            };
            return Ok(cars);
        }
        [HttpPost]
        public ActionResult<Car> CreateCar([FromBody] Car car)
        {
            if (car == null)
            {
                return BadRequest("Car cannot be null");
            }
            car.Id = Guid.NewGuid();
            car.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd");
            // Save the car to the database (not implemented here)
            return CreatedAtAction(nameof(GetCars), new { id = car.Id }, car);
        }
    }
}
