using backend.Data;
using backend.Entities;
using backend.Models;
using backend.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
//TODO
//Add delete and update methods
namespace backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController(ICarService _carService) : Controller {

        [HttpGet]
        public async Task<ActionResult<List<CarCardDTO>>> GetCars() {
            var cars = await _carService.GetCarsAsync();
            var carDtos = cars.Select(car => new CarCardDTO {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                Milleage = car.Milleage,
                Price = car.Price,
                FuelType = car.FuelType,
                Status = car.Status,
                Image = Url.Action(nameof(GetCarImage), new { id = car.Id })

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
        [HttpPost("AddCar")]
        public async Task<ActionResult> CreateCar([FromForm] CarAddDTO car) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Invalid token");
            if (car == null)
                return BadRequest("Car cannot be null");

            byte[]? imageData = null;
            if (car.Image != null && car.Image.Length > 0) {
                using (var ms = new MemoryStream()) {
                    await car.Image.CopyToAsync(ms);
                    imageData = ms.ToArray();
                }
            }

            var createdCar = await _carService.CreateCarAsync(car, Guid.Parse(userId), imageData);
            return Ok("Car added successfully");
        }
        [HttpGet("{id:guid}/image")]
        public async Task<IActionResult> GetCarImage(Guid id) {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null || car.Image == null)
                return NotFound();

            return File(car.Image, "image/jpeg"); // Adjust content type if needed
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateCar(Guid id, [FromForm] CarAddDTO updatedCar)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) 
                return Unauthorized("Invalid token");

            var existingCar = await _carService.GetCarByIdAsync(id);
            if (existingCar == null)
                return NotFound("Car not found.");

            if (existingCar.SellerId.ToString() != userId)
                return Forbid("You are not allowed to update this car.");

            byte[]? imageData = null;
            if (updatedCar.Image != null && updatedCar.Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await updatedCar.Image.CopyToAsync(ms);
                imageData = ms.ToArray();
            }

            var result = await _carService.UpdateCarAsync(id, updatedCar, imageData);
            if (!result)
                return BadRequest("Failed to update car.");

            return Ok("Car updated successfully.");
        }


        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteCar(Guid id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
                return NotFound("Car not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Invalid token.");

            if (car.SellerId.ToString() != userId)
                return Forbid("You are not allowed to delete this car!");

            var result = await _carService.DeleteCarAsync(id);
            if (!result)
                return BadRequest("Failed to delete the car.");

            return Ok("Car deleted successfully.");
        }

    }
}
