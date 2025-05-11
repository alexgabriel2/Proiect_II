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

namespace backend.Controllers.Tests
{
    // ---------------- REGISTER TESTS ----------------

    [TestClass()]
    public class AuthControllerTests
    {
        [TestMethod]
        public async Task Register_UserAlreadyExists_ReturnsBadRequest()
        {
            // User deja existent => returns BadRequest
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto
            {
                Username = "string",
                Password = "password123"
            };

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null); // Simulate user already exists

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.IsNotNull(badRequestResult.Value);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(string));
        }

        [TestMethod]
        public async Task Register_ValidUser_ReturnsOk()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();

            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe",
                Email = "newuser@example.com"
            };

            var tokenResponse = new TokenResponseDto
            {
                AccessToken = "some-access-token",
                RefreshToken = "some-refresh-token"
            };

            // Setăm mock-ul astfel încât să nu existe user deja înregistrat
            mockAuthService
                .Setup(service => service.checkExisting(It.IsAny<RegisterDto>()))
                .ReturnsAsync(new List<string>()); // niciun user existent

            // Simulăm că serviciul creează cu succes userul
            mockAuthService
                .Setup(service => service.RegisterAsync(It.IsAny<RegisterDto>()))
                .ReturnsAsync(tokenResponse);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = (OkObjectResult)result.Result;
            var returnedTokenResponse = okResult.Value as TokenResponseDto;

            Assert.IsNotNull(returnedTokenResponse);
            Assert.AreEqual(tokenResponse.AccessToken, returnedTokenResponse?.AccessToken);
            Assert.AreEqual(tokenResponse.RefreshToken, returnedTokenResponse?.RefreshToken);
        }


        [TestMethod]
        public async Task Register_MissingUsername_ReturnsBadRequest()
        {
            //Register fara username => serviciu null si controller BadRequest
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "", Password = "password123" };

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequest = (BadRequestObjectResult)result.Result;
            Assert.IsNotNull(badRequest.Value); 
            Assert.IsInstanceOfType(badRequest.Value, typeof(string)); 
        }

        [TestMethod]
        public async Task Register_MissingPassword_ReturnsBadRequest()
        {
            //Register fara parola => serviciu null si controller BadRequest
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "newuser", Password = "" };

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_MissingUsernameAndPassword_ReturnsBadRequest()
        {
            //Register fara user & parola => serviciu null + BadRequest
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "", Password = "" };

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_ShortPassword_ReturnsBadRequest()
        {
            //Parola prea scurta => serviciu null pentru parola invalida
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "newuser", Password = "123" };

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null); // simulăm rejectarea parolei scurte

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_ServiceThrowsException_ReturnsBadRequest()
        {
            // Serviciul arunca o exceptie => controllerul trebuie sa returneze un BadRequest
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "erroruser", Password = "password123" };

            // Simulăm aruncarea unei excepții de tip Exception
            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ThrowsAsync(new Exception("Unexpected error"));

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult)); // Verificăm că este un BadRequestObjectResult
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("Please try again", badRequestResult.Value); // Verificăm mesajul specificat în controller
        }


       /* [TestMethod]
        public async Task Register_UsernameWithSpecialCharacters_ReturnsOk()
        {
            //User foloseste caractere speciale
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "user@domain.com", Password = "password123" };

            var tokenResponse = new TokenResponseDto
            {
                AccessToken = "some-access-token",
                RefreshToken = "some-refresh-token"
            };

            mockAuthService.Setup(service => service.RegisterAsync(registerDto))
                           .ReturnsAsync(tokenResponse);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            var returnedTokenResponse = okResult.Value as TokenResponseDto; // Converim `okResult.Value` la tipul `TokenResponseDto`

            // Verifici dacă token-urile returnate sunt corecte
            Assert.IsNotNull(returnedTokenResponse);
            Assert.AreEqual(tokenResponse.AccessToken, returnedTokenResponse?.AccessToken);
            Assert.AreEqual(tokenResponse.RefreshToken, returnedTokenResponse?.RefreshToken);
        }*/

        [TestMethod]
        public async Task Register_WeakPassword_ReturnsBadRequest()
        {
            //Parola slaba
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "user", Password = "password" };  // Fără numere și caractere speciale

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_InvalidEmail_ReturnsBadRequest()
        {
            // Invalid email format
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "user", Password = "password123", Email = "invalid-email" };

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null); // Simulează că emailul este invalid

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_UsernameTooShort_ReturnsBadRequest()
        {
            // Username prea scurt
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "a", Password = "password123" };  // Username prea scurt

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_UsernameTooLong_ReturnsBadRequest()
        {
            // Username prea lung
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = new string('a', 51), Password = "password123" };  // Username prea lung

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_UsernameWithInvalidCharacters_ReturnsBadRequest()
        {
            // Username conține caractere speciale nepermise
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "user!@#", Password = "password123" };

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null); // Simulează că username-ul nu este valid

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_UsernameWithWhitespace_ReturnsBadRequest()
        {
            // Username conține doar spații
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto { Username = "    ", Password = "password123" };

            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null); // Simulează că username-ul nu este valid

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_NullRequestBody_ReturnsBadRequest()
        {
            // Request cu body null
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            RegisterDto? registerDto = null;  // Request cu body null

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto
            {
                Username = "user123",
                Password = "password123",
                Email = "duplicate@example.com"
            };

            // Simulăm că emailul este deja înregistrat, deci serviciul returnează null
            mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequest = (BadRequestObjectResult)result.Result;
            Assert.IsNotNull(badRequest.Value);
            Assert.IsInstanceOfType(badRequest.Value, typeof(string)); 
        }

        [TestMethod]
        public async Task Register_SameUsernameDifferentCase_ReturnsBadRequest()
        {   // Sensibilitate case-uri
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto
            {
                Username = "User123", // presupunem că "user123" există deja
                Password = "Password123!",
                Email = "user123@example.com"
            };

            // Simulează că serviciul detectează existența userului (insensibil la case)
            mockAuthService
                .Setup(service => service.checkExisting(registerDto))
                .ReturnsAsync(new List<string> { "Username already exists" });

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.IsNotNull(badRequestResult.Value);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(string));
        }

        [TestMethod]
        public async Task Register_WithWhitespaceTrimmed_ReturnsBadRequest()
        {
            //Spatii goale in creditentials
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var registerDto = new RegisterDto
            {
                Username = "   newuser   ",
                Password = "   Password123!   ",
                FirstName = "   John   ",
                LastName = "   Doe   ",
                Email = "   newuser@example.com   "
            };

            // Simulează că serviciul de validare returnează erori din cauza spațiilor
            mockAuthService
                .Setup(service => service.checkExisting(It.IsAny<RegisterDto>()))
                .ReturnsAsync(new List<string>());

            mockAuthService
                .Setup(service => service.RegisterAsync(It.IsAny<RegisterDto>()))
                .ReturnsAsync((TokenResponseDto?)null);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        // ---------------- LOGIN TESTS ----------------

        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsOkWithTokens()
        {
            //Login cu date valide
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = "validUser", Password = "validPassword" };

            var expectedResponse = new TokenResponseDto
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            };

            mockAuthService.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(expectedResponse);
            var controller = new AuthController(mockAuthService.Object);

            var result = await controller.Login(loginDto);

            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            var tokenResponse = okResult.Value as TokenResponseDto;
            Assert.IsNotNull(tokenResponse);
            Assert.AreEqual(expectedResponse.AccessToken, tokenResponse?.AccessToken);
            Assert.AreEqual(expectedResponse.RefreshToken, tokenResponse?.RefreshToken);
        }

        [TestMethod]
        public async Task Login_InvalidCredentials_ReturnsBadRequest()
        {
            //Login cu date invalide
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = "invalid", Password = "wrongpass" };

            mockAuthService.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync((TokenResponseDto?)null);
            var controller = new AuthController(mockAuthService.Object);

            var result = await controller.Login(loginDto);

            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequest = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("Invalid credentials", badRequest.Value);
        }

        [TestMethod]
        public async Task Login_InvalidUsername_ReturnsBadRequest()
        {
            //Username gresit
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = "invaliduser", Password = "validpassword123" };

            mockAuthService.Setup(service => service.LoginAsync(loginDto)).ReturnsAsync((TokenResponseDto)null);
            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("Invalid credentials", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Login_InvalidPassword_ReturnsBadRequest()
        {
            //Parola gresita
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = "validuser", Password = "invalidpassword" };

            mockAuthService.Setup(service => service.LoginAsync(loginDto)).ReturnsAsync((TokenResponseDto)null);
            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("Invalid credentials", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Login_EmptyUsernameAndPassword_ReturnsBadRequest()
        {
            //Username si parola goale
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = "", Password = "" };

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("Invalid credentials", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Login_UsernameAndPasswordWithSpaces_ReturnsBadRequest()
        {
            //Username si parola cu spatii
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = "  ", Password = "   " };

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("Invalid credentials", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Login_AccountInactive_ReturnsBadRequest()
        {
            //Cretitentials pentru cont inactiv
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = "inactiveuser", Password = "validpassword123" };

            // Simulăm că utilizatorul este inactiv
            mockAuthService.Setup(service => service.LoginAsync(loginDto)).ReturnsAsync((TokenResponseDto)null);
            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("Invalid credentials", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Login_TooManyFailedAttempts_LocksAccount()
        {
            //Incercari multiple gresite
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = "bruteforceuser", Password = "wrongpassword" };

            // Simulăm mai multe încercări greșite
            for (int i = 0; i < 5; i++)
            {
                mockAuthService.Setup(service => service.LoginAsync(loginDto)).ReturnsAsync((TokenResponseDto)null);
            }

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task Login_NullUsername_ReturnsBadRequest()
        {
            //Username null
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = null!, Password = "somepassword" };

            var controller = new AuthController(mockAuthService.Object);

            var result = await controller.Login(loginDto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("Invalid credentials", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Login_NullPassword_ReturnsBadRequest()
        {
            //Parola nula
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = "validuser", Password = null! };

            var controller = new AuthController(mockAuthService.Object);

            var result = await controller.Login(loginDto);

            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result.Result;
            Assert.AreEqual("Invalid credentials", badRequestResult.Value);
        }

        [TestMethod]
        public async Task Login_AuthServiceThrowsException_ReturnsInternalServerError()
        {
            //Exceptie login
            var mockAuthService = new Mock<IAuthService>();
            var loginDto = new LoginDto { Username = "user", Password = "password" };

            mockAuthService
                .Setup(service => service.LoginAsync(loginDto))
                .ThrowsAsync(new Exception("Unexpected error"));

            var controller = new AuthController(mockAuthService.Object);

            var result = await controller.Login(loginDto);

            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result.Result;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        // ---------------- REFRESH TOKEN TESTS ----------------

        [TestMethod]
        public async Task RefreshToken_ValidToken_ReturnsOkResult()
        {
            //Token valid
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var refreshTokenRequestDto = new RefreshTokenRequestDto
            {
                UserId = Guid.NewGuid(),  // Generăm un UserId valid
                RefreshToken = "valid_refresh_token"
            };

            var tokenResponseDto = new TokenResponseDto
            {
                AccessToken = "new_access_token",
                RefreshToken = "new_refresh_token"
            };

            mockAuthService
                .Setup(service => service.RefreshTokenAsync(refreshTokenRequestDto))
                .ReturnsAsync(tokenResponseDto);

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.RefreshToken(refreshTokenRequestDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result.Result;
            Assert.AreEqual(200, okResult.StatusCode);
            var responseDto = (TokenResponseDto)okResult.Value;
            Assert.AreEqual("new_access_token", responseDto.AccessToken);
            Assert.AreEqual("new_refresh_token", responseDto.RefreshToken);
        }

        [TestMethod]
        public async Task RefreshToken_InvalidToken_ReturnsUnauthorized()
        {
            //Token invalid
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var refreshTokenRequestDto = new RefreshTokenRequestDto
            {
                UserId = Guid.NewGuid(),  // Generăm un UserId valid
                RefreshToken = "invalid_refresh_token"
            };

            mockAuthService
                .Setup(service => service.RefreshTokenAsync(refreshTokenRequestDto))
                .ReturnsAsync((TokenResponseDto)null);  // Serviciul returnează null pentru token invalid

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.RefreshToken(refreshTokenRequestDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
            var unauthorizedResult = (UnauthorizedObjectResult)result.Result;
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
            Assert.AreEqual("Invalid token", unauthorizedResult.Value);
        }

        [TestMethod]
        public async Task RefreshToken_ServiceThrowsException_ReturnsInternalServerError()
        {
            //Exceptie serviciu
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var refreshTokenRequestDto = new RefreshTokenRequestDto
            {
                UserId = Guid.NewGuid(),  // Generăm un UserId valid
                RefreshToken = "some_refresh_token"
            };

            mockAuthService
                .Setup(service => service.RefreshTokenAsync(refreshTokenRequestDto))
                .ThrowsAsync(new Exception("Unexpected error"));

            var controller = new AuthController(mockAuthService.Object);

            // Act
            var result = await controller.RefreshToken(refreshTokenRequestDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result.Result;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("Unexpected error", objectResult.Value);
        }

        // ---------------- PASSWORD CHANGE TESTS ----------------

        [TestMethod]
        public async Task ChangePassword_ValidData_ReturnsOk()
        {
            //Schimbare parola cu succes
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

            var controller = new AuthController(mockAuthService.Object);

            // Simulăm un utilizator autentificat cu username = "testuser"
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
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Password changed successfully.", okResult.Value);
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
                NewPassword = "pass" // prea scurtă, identică
            };

            var controller = new AuthController(mockAuthService.Object);

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
            //Parola gresita
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "WrongOldPassword!",
                NewPassword = "NewPassword1!"
            };

            mockAuthService
                .Setup(service => service.ChangePasswordAsync(changePasswordDto, "testuser"))
                .ReturnsAsync(false); // Simulăm eșec la schimbare

            var controller = new AuthController(mockAuthService.Object);

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


    }
}
