using backend.Data;
using backend.Entities;
using backend.Models;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
//todo modify controller to use services
namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController( ICarService _carService) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<List<CarDTO>>> GetCars() {
            var cars = await _carService.GetCarsAsync();
            var carDtos = cars.Select(car => new CarDTO {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                Milleage = car.Milleage,
                Price = car.Price,
                FuelType = car.FuelType,
                Status = car.Status,
            }).ToList();
            return Ok(carDtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Car>> GetCarById(Guid id) {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
                return NotFound();
            return Ok(car);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Car>> CreateCar([FromBody] Car car) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                return Unauthorized("Invalid token");
            }
            if (car == null) {
                return BadRequest("Car cannot be null");
            }
            var createdCar = await _carService.CreateCarAsync(car, Guid.Parse(userId));
            return CreatedAtAction(nameof(GetCarById), new { id = createdCar.Id }, createdCar);
        }
    }
}
