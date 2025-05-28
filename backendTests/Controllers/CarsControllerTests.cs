using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using backend.Controllers;
using backend.services;
using backend.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using backend.Models;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Routing;

namespace backendTests.Controllers
{
    [TestClass]
    public class CarsControllerTests
    {
        private Mock<ICarService> mockCarService;
        private CarsController controller;

        [TestInitialize]
        public void Setup()
        {
            mockCarService = new Mock<ICarService>();
            controller = new CarsController(mockCarService.Object);
        }

        [TestMethod] 
        public async Task GetCars_ReturnsOkWithList()
        {
            // Arrange
            var mockCarService = new Mock<ICarService>();
            var fakeCars = new List<Car>
            {
                new Car
                {
                    Id = Guid.NewGuid(),
                    Make = "Toyota",
                    Model = "Corolla",
                    Year = 2020,
                    Milleage = 15000,
                    Price = 15000,
                    FuelType = "Gasoline",
                    Status = "Available",
                    Image = null
                },
                new Car
                {
                    Id = Guid.NewGuid(),
                    Make = "Honda",
                    Model = "Civic",
                    Year = 2019,
                    Milleage = 20000,
                    Price = 14000,
                    FuelType = "Gasoline",
                    Status = "Sold",
                    Image = null
                }
            };

            mockCarService.Setup(service => service.GetCarsAsync())
                          .ReturnsAsync(fakeCars);

            var controller = new CarsController(mockCarService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper
                .Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://fake-url/image");

            controller.Url = mockUrlHelper.Object;

            // Act
            var result = await controller.GetCars();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            var carList = okResult.Value as List<CarCardDTO>;

            Assert.IsNotNull(carList);
            Assert.AreEqual(fakeCars.Count, carList.Count);
            Assert.AreEqual(fakeCars[0].Make, carList[0].Make);
        }

        [TestMethod]
        public async Task GetCars_EmptyList_ReturnsOkWithEmptyList()
        {
            // Arrange
            var mockCarService = new Mock<ICarService>();
            mockCarService.Setup(service => service.GetCarsAsync())
                          .ReturnsAsync(new List<Car>());

            var controller = new CarsController(mockCarService.Object);

            // Act
            var result = await controller.GetCars();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var carList = okResult.Value as List<CarCardDTO>;
            Assert.IsNotNull(carList);
            Assert.AreEqual(0, carList.Count);
        }

        [TestMethod]
        public async Task GetCarById_ExistingCar_ReturnsOk()
        {
            // Arrange
            var mockCarService = new Mock<ICarService>();
            var carId = Guid.NewGuid();
            var car = new Car
            {
                Id = carId,
                Make = "Toyota",
                Model = "Corolla",
                Year = 2020,
                Milleage = 15000,
                Price = 15000,
                FuelType = "Gasoline",
                Status = "Available"
            };

            mockCarService.Setup(service => service.GetCarByIdAsync(carId))
                          .ReturnsAsync(car);

            var controller = new CarsController(mockCarService.Object);

            // Act
            var result = await controller.GetCarById(carId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedCar = okResult.Value as Car;
            Assert.IsNotNull(returnedCar);
            Assert.AreEqual(carId, returnedCar.Id);
        }

        [TestMethod]
        public async Task GetCarById_NonExistingCar_ReturnsNotFound()
        {
            // Arrange
            var mockCarService = new Mock<ICarService>();
            var carId = Guid.NewGuid();

            mockCarService.Setup(service => service.GetCarByIdAsync(carId))
                          .ReturnsAsync((Car?)null);

            var controller = new CarsController(mockCarService.Object);

            // Act
            var result = await controller.GetCarById(carId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CreateCar_ValidData_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var carAddDto = new CarAddDTO
            {
                Make = "Toyota",
                Model = "Corolla",
                Year = 2020,
                Milleage = 10000,
                Price = 15000,
                FuelType = "Gasoline",
                Transmission = "Automatic",
                Description = "Nice car",
                Status = "Available",
                Image = null
            };

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            mockCarService.Setup(s => s.CreateCarAsync(carAddDto, userId, null))
                .ReturnsAsync(new Car { Id = Guid.NewGuid(), Make = carAddDto.Make });

            // Act
            var result = await controller.CreateCar(carAddDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Car added successfully", okResult.Value);
        }

        [TestMethod]
        public async Task CreateCar_MissingUserId_ReturnsUnauthorized()
        {
            // Arrange
            var carAddDto = new CarAddDTO { Make = "Toyota" };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.CreateCar(carAddDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.AreEqual("Invalid token", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task CreateCar_NullCarDto_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.CreateCar(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Car cannot be null", badRequestResult.Value);
        }

        [TestMethod]
        public async Task GetCarImage_ExistingImage_ReturnsFile()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var imageData = new byte[] { 1, 2, 3, 4 };
            var car = new Car { Id = carId, Image = imageData };

            var mockCarService = new Mock<ICarService>();
            mockCarService.Setup(s => s.GetCarByIdAsync(carId)).ReturnsAsync(car);

            var controller = new CarsController(mockCarService.Object);

            // Act
            var result = await controller.GetCarImage(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(FileContentResult));
            var fileResult = result as FileContentResult;
            CollectionAssert.AreEqual(imageData, fileResult.FileContents);
            Assert.AreEqual("image/jpeg", fileResult.ContentType);
        }

        [TestMethod]
        public async Task GetCarImage_CarWithoutImage_ReturnsNotFound()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var car = new Car { Id = carId, Image = null };

            var mockCarService = new Mock<ICarService>();
            mockCarService.Setup(s => s.GetCarByIdAsync(carId)).ReturnsAsync(car);

            var controller = new CarsController(mockCarService.Object);

            // Act
            var result = await controller.GetCarImage(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetCarImage_CarNotFound_ReturnsNotFound()
        {
            // Arrange
            var carId = Guid.NewGuid();

            var mockCarService = new Mock<ICarService>();
            mockCarService.Setup(s => s.GetCarByIdAsync(carId)).ReturnsAsync((Car?)null);

            var controller = new CarsController(mockCarService.Object);

            // Act
            var result = await controller.GetCarImage(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        [TestMethod]
        public async Task DeleteCar_ExistingCarAndAuthorizedUser_ReturnsOk()
        {
            // Arrange
            var mockCarService = new Mock<ICarService>();
            var carId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var car = new Car
            {
                Id = carId,
                SellerId = userId
            };

            mockCarService.Setup(s => s.GetCarByIdAsync(carId))
                          .ReturnsAsync(car);

            mockCarService.Setup(s => s.DeleteCarAsync(carId))
                          .ReturnsAsync(true);

            var controller = new CarsController(mockCarService.Object);

            // Setup user claims (authorized user is the seller)
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.DeleteCar(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Car deleted successfully.", okResult.Value);
        }

        [TestMethod]
        public async Task DeleteCar_NonExistingCar_ReturnsNotFound()
        {
            // Arrange
            var mockCarService = new Mock<ICarService>();
            var carId = Guid.NewGuid();

            mockCarService.Setup(s => s.GetCarByIdAsync(carId))
                          .ReturnsAsync((Car?)null);

            var controller = new CarsController(mockCarService.Object);

            // Setup user claims (any user)
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.DeleteCar(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Car not found.", notFoundResult.Value);
        }

        [TestMethod]
        public async Task DeleteCar_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var mockCarService = new Mock<ICarService>();
            var carId = Guid.NewGuid();

            var car = new Car
            {
                Id = carId,
                SellerId = Guid.NewGuid()
            };

            mockCarService.Setup(s => s.GetCarByIdAsync(carId))
                          .ReturnsAsync(car);

            var controller = new CarsController(mockCarService.Object);

            // No user claims setup - unauthorized
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // no user
            };

            // Act
            var result = await controller.DeleteCar(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.AreEqual("Invalid token.", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task DeleteCar_UserNotOwner_ReturnsForbid()
        {
            // Arrange
            var mockCarService = new Mock<ICarService>();
            var carId = Guid.NewGuid();

            var car = new Car
            {
                Id = carId,
                SellerId = Guid.NewGuid()
            };

            mockCarService.Setup(s => s.GetCarByIdAsync(carId))
                          .ReturnsAsync(car);

            var controller = new CarsController(mockCarService.Object);

            // Setup claims with user different from SellerId
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.DeleteCar(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task DeleteCar_DeleteFails_ReturnsBadRequest()
        {
            // Arrange
            var mockCarService = new Mock<ICarService>();
            var carId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var car = new Car
            {
                Id = carId,
                SellerId = userId
            };

            mockCarService.Setup(s => s.GetCarByIdAsync(carId))
                          .ReturnsAsync(car);

            mockCarService.Setup(s => s.DeleteCarAsync(carId))
                          .ReturnsAsync(false);

            var controller = new CarsController(mockCarService.Object);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.DeleteCar(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Failed to delete the car.", badRequestResult.Value);
        }
    }
}
