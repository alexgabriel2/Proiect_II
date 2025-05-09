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

namespace backend.Controllers.Tests {
    [TestClass()]
    public class AuthControllerTests {
        [TestMethod]
        public async Task Register_ReturnsOk_WhenUserAlreadyExists() {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var userDto = new RegisterDto {
                Username = "existingUser",
                Password = "password123"
            };

            mockAuthService
                .Setup(service => service.RegisterAsync(userDto))
                .ReturnsAsync((TokenResponseDto?)null); // Simulate user already exists

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(userDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult)); // This expectation is incorrect
        }

    }
}
