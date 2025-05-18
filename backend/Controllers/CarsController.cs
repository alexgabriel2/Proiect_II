using backend.Data;
using backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Car>>> GetCars()
        {
            var cars = await _context.Cars.ToListAsync();
            return Ok(cars);
        }
        [HttpPost]
        public async Task<ActionResult<List<Car>>> CreateCar([FromBody] Car car)
        {
            if (car == null)
            {
                return BadRequest("Car cannot be null");
            }
            car.Id = Guid.NewGuid();
            car.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd");
            _context.Cars.Add(car);                 
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCars), new { id = car.Id }, car);
        }
    }
}
