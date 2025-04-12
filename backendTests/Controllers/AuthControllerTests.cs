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
        public async Task Register_UserAlreadyExists_ReturnsBadRequest() {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var userDto = new UserDto {
                Username = "string",
                Password = "password123"
            };

            mockAuthService
                .Setup(service => service.RegisterAsync(userDto))
                .ReturnsAsync((User?)null); // Simulate user already exists

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(userDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("User already exists", badRequestResult.Value);
        }
    }
}
