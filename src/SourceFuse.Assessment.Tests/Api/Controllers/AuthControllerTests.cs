using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SourceFuse.Assessment.Api.Controllers;
using SourceFuse.Assessment.Common.Models;
using SourceFuse.Assessment.Common.Services;

namespace SourceFuse.Assessment.Tests.Api.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authServiceMock;
        private Mock<ILogger<AuthController>> _loggerMock;
        private AuthController _authController;

        [SetUp]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _authController = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public void Login_ReturnsOkResult_WithToken_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "testuser", Password = "password" };
            var token = "testtoken";
            _authServiceMock.Setup(service => service.Login(loginModel)).Returns(token);

            // Act
            var result = _authController.Login(loginModel);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<string>(okResult.Value.GetType().GetProperty("Token").GetValue(okResult.Value, null));
            Assert.AreEqual(token, okResult.Value.GetType().GetProperty("Token").GetValue(okResult.Value, null));
        }

        [Test]
        public void Login_ReturnsUnauthorized_WhenLoginFails()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "testuser", Password = "wrongpassword" };
            _authServiceMock.Setup(service => service.Login(loginModel)).Returns((string)null);

            // Act
            var result = _authController.Login(loginModel);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }
    }
}