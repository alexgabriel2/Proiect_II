using Microsoft.VisualStudio.TestTools.UnitTesting;
using backend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using backend.services;
using backend.Models;
using backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace backendTests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        // ---------------- PASSWORD CHANGE TESTS ----------------

        [TestMethod]
        public async Task ChangePassword_ValidData_ReturnsOk()
        {
            // Schimbare parola cu succes
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "OldPassword1!",
                NewPassword = "NewPassword1!"
            };

            mockAuthService
                .Setup(service => service.ChangePasswordAsync(changePasswordDto, "testuser"))
                .ReturnsAsync(true);

            var controller = new UserController(mockAuthService.Object);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.ChangePassword(changePasswordDto);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var dict = okResult?.Value as IDictionary<string, object>;
            if (dict != null) {
                Assert.AreEqual("Password changed successfully.", dict["message"]);
            } else {
                // fallback for anonymous type (reflection)
                var messageProp = okResult?.Value?.GetType().GetProperty("message");
                Assert.IsNotNull(messageProp, "message property not found");
                var messageValue = messageProp.GetValue(okResult.Value);
                Assert.AreEqual("Password changed successfully.", messageValue);
            }
        }

        [TestMethod]
        public async Task ChangePassword_InvalidDto_ReturnsBadRequest()
        {
            // Parola identica sau prea scurta
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "pass",
                NewPassword = "pass"
            };

            var controller = new UserController(mockAuthService.Object);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.ChangePassword(changePasswordDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequest = result as BadRequestObjectResult;
            Assert.IsInstanceOfType(badRequest.Value, typeof(List<string>));
            var errors = badRequest.Value as List<string>;
            Assert.IsTrue(errors.Count > 0);
        }

        [TestMethod]
        public async Task ChangePassword_ChangeFails_ReturnsBadRequest()
        {
            // Parola greșită sau altă eroare
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "WrongOldPassword!",
                NewPassword = "NewPassword1!"
            };

            mockAuthService
                .Setup(service => service.ChangePasswordAsync(changePasswordDto, "testuser"))
                .ReturnsAsync(false);

            var controller = new UserController(mockAuthService.Object);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.ChangePassword(changePasswordDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequest = result as BadRequestObjectResult;
            Assert.AreEqual("Password could not be changed.", badRequest.Value);
        }

        //---------------- GET INFO TESTS -----------------

        [TestMethod]
        public async Task GetInfo_NoUserId_ReturnsUnauthorized()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var controller = new UserController(mockAuthService.Object);

            // Setăm user fără claim de NameIdentifier
            var claims = new List<Claim>();
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.GetInfo();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.AreEqual("Invalid token", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task GetInfo_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var userId = "123";

            mockAuthService
                .Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync((UserDTO)null);

            var controller = new UserController(mockAuthService.Object);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.GetInfo();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.AreEqual("User not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task GetInfo_ValidUser_ReturnsOkWithUser()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var userId = "123";
            var userDto = new UserDTO
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            mockAuthService
                .Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(userDto);

            var controller = new UserController(mockAuthService.Object);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.GetInfo();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(userDto, okResult.Value);
        }

        //------------------- UPDATE INFO TESTS ---------

        [TestMethod]
        public async Task UpdateInfo_NoUserId_ReturnsUnauthorized()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var controller = new UserController(mockAuthService.Object);

            var userDto = new UserDTO { FirstName = "New", LastName = "Name", Email = "new@example.com" };

            var claims = new List<Claim>(); // fără NameIdentifier
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.UpdateInfo(userDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.AreEqual("Invalid token", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task UpdateInfo_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var userId = "123";

            // Mock GetUserByIdAsync să întoarcă null pentru UserDTO
            mockAuthService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((UserDTO?)null);

            var controller = new UserController(mockAuthService.Object);

            var userDto = new UserDTO { FirstName = "New", LastName = "Name", Email = "new@example.com" };

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.UpdateInfo(userDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.AreEqual("User not found", notFoundResult.Value);
        }

        [TestMethod]
        public async Task UpdateInfo_ValidUser_UpdatesAndReturnsOk()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var userId = "123";

            var existingUserDto = new UserDTO
            {
                Id = userId,
                FirstName = "Old",
                LastName = "Name",
                Email = "old@example.com"
            };

            mockAuthService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(existingUserDto);
            mockAuthService.Setup(s => s.UpdateUserAsync(It.IsAny<UserDTO>(), userId)).Returns(Task.CompletedTask);

            var controller = new UserController(mockAuthService.Object);

            var userDto = new UserDTO
            {
                FirstName = "New",
                LastName = "Name",
                Email = "new@example.com"
            };

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.UpdateInfo(userDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;

            var updatedUser = okResult.Value as UserDTO;
            Assert.AreEqual("New", updatedUser.FirstName);
            Assert.AreEqual("Name", updatedUser.LastName);
            Assert.AreEqual("new@example.com", updatedUser.Email);
        }

        //----------------- FAVORITE TESTS ---------------

        //--- ADD ---
        [TestMethod]
        public async Task SaveCar_WithValidUserAndCarId_ReturnsOk()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();

            var mockAuthService = new Mock<IAuthService>();

            // Setup pentru un user valid
            var userDto = new UserDTO { Id = userId };
            mockAuthService.Setup(s => s.GetUserByIdAsync(userId))
                           .ReturnsAsync(userDto);

            // Setup pentru adăugarea la favorite
            mockAuthService.Setup(s => s.AddToFavorite(carId.ToString(), userId))
                           .ReturnsAsync(new Favorite
                           {
                               CarId = carId,
                               UserId = Guid.Parse(userId)
                           });

            var controller = new UserController(mockAuthService.Object);

            // Simulăm autentificarea cu token (ClaimsPrincipal)
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            // Act
            var result = await controller.SaveCar(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Saved", okResult?.Value);
        }

        [TestMethod]
        public async Task SaveCar_AddFavoriteFails_ReturnsBadRequest()
        {
            // Arrange
            var carId = Guid.NewGuid();
            var userId = Guid.NewGuid().ToString();
            var mockAuthService = new Mock<IAuthService>();

            mockAuthService.Setup(s => s.GetUserByIdAsync(userId))
                           .ReturnsAsync(new UserDTO { Id = userId });

            mockAuthService.Setup(s => s.AddToFavorite(carId.ToString(), userId))
                           .ReturnsAsync((Favorite)null); // simulăm eșecul

            var controller = new UserController(mockAuthService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
                }
            };

            // Act
            var result = await controller.SaveCar(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task SaveCar_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((UserDTO)null);

            var controller = new UserController(mockAuthService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
                }
            };

            // Act
            var result = await controller.SaveCar(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task SaveCar_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new UserController(new Mock<IAuthService>().Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // fără user
            };

            // Act
            var result = await controller.SaveCar(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        //--- GET ---
        [TestMethod]
        public async Task GetFavorite_WithValidUser_ReturnsFavorites()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            var mockAuthService = new Mock<IAuthService>();
            var userDto = new UserDTO { Id = userId };

            mockAuthService.Setup(s => s.GetUserByIdAsync(userId))
                           .ReturnsAsync(userDto);

            var favoriteList = new List<CarCardDTO>
    {
        new CarCardDTO { Id = Guid.NewGuid(), Make = "Dacia", Model = "Logan", Year = 2020, Price = 5000 },
        new CarCardDTO { Id = Guid.NewGuid(), Make = "BMW", Model = "X5", Year = 2022, Price = 35000 }
    };

            mockAuthService.Setup(s => s.GetFavorites(userId))
                           .ReturnsAsync(favoriteList);

            var controller = new UserController(mockAuthService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
                }
            };

            // Act
            var result = await controller.GetFavorite();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var favorites = okResult?.Value as List<CarCardDTO>;
            Assert.IsNotNull(favorites);
            Assert.AreEqual(2, favorites.Count);
        }

        [TestMethod]
        public async Task GetFavorite_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new UserController(new Mock<IAuthService>().Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // fără token
            };

            // Act
            var result = await controller.GetFavorite();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public async Task GetFavorite_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var mockAuthService = new Mock<IAuthService>();

            mockAuthService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((UserDTO)null);

            var controller = new UserController(mockAuthService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
                }
            };

            // Act
            var result = await controller.GetFavorite();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetFavorite_FavoritesReturnNull_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var mockAuthService = new Mock<IAuthService>();

            mockAuthService.Setup(s => s.GetUserByIdAsync(userId))
                           .ReturnsAsync(new UserDTO { Id = userId });

            mockAuthService.Setup(s => s.GetFavorites(userId))
                           .ReturnsAsync((List<CarCardDTO>)null); // simulăm eroare

            var controller = new UserController(mockAuthService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
                }
            };

            // Act
            var result = await controller.GetFavorite();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        //--- DELETE ---

        [TestMethod]
        public async Task DeleteFavorite_WithValidUserAndCarId_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var carId = Guid.NewGuid();

            var mockAuthService = new Mock<IAuthService>();

            mockAuthService.Setup(s => s.GetUserByIdAsync(userId))
                           .ReturnsAsync(new UserDTO { Id = userId });

            mockAuthService.Setup(s => s.DeleteFavorite(carId.ToString(), userId))
                           .ReturnsAsync(true);

            var controller = new UserController(mockAuthService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
                }
            };

            // Act
            var result = await controller.DeleteFavorite(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Deleted", okResult?.Value);
        }

        [TestMethod]
        public async Task DeleteFavorite_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            var controller = new UserController(new Mock<IAuthService>().Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // fără token
            };

            // Act
            var result = await controller.DeleteFavorite(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public async Task DeleteFavorite_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var carId = Guid.NewGuid();

            var mockAuthService = new Mock<IAuthService>();

            mockAuthService.Setup(s => s.GetUserByIdAsync(userId))
                           .ReturnsAsync((UserDTO)null);

            var controller = new UserController(mockAuthService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
                }
            };

            // Act
            var result = await controller.DeleteFavorite(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task DeleteFavorite_DeleteFails_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var carId = Guid.NewGuid();

            var mockAuthService = new Mock<IAuthService>();

            mockAuthService.Setup(s => s.GetUserByIdAsync(userId))
                           .ReturnsAsync(new UserDTO { Id = userId });

            mockAuthService.Setup(s => s.DeleteFavorite(carId.ToString(), userId))
                           .ReturnsAsync(false);

            var controller = new UserController(mockAuthService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
                }
            };

            // Act
            var result = await controller.DeleteFavorite(carId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }


    }
}
