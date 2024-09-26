using Microsoft.Extensions.Configuration;
using Moq;
using SourceFuse.Assessment.Common.Models;
using SourceFuse.Assessment.Common.Services;

namespace SourceFuse.Assessment.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IConfiguration> _configurationMock;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();

            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["Key"]).Returns("sdfwewew3rr32rwed323434gxg5454wer");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("testIssuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("testAudience");
            jwtSectionMock.Setup(x => x["ExpirationMinutes"]).Returns("60");

            _configurationMock.Setup(x => x.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            _authService = new AuthService(_configurationMock.Object);
        }

        [Test]
        public void Login_ValidUser_ReturnsToken()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "spuertab1", Password = "123" };

            // Act
            var token = _authService.Login(loginModel);

            // Assert
            Assert.NotNull(token);
        }

        [Test]
        public void Login_InvalidUser_ReturnsNull()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "invalidUser", Password = "wrongPassword" };

            // Act
            var token = _authService.Login(loginModel);

            // Assert
            Assert.Null(token);
        }

        [Test]
        public void Login_ValidUserWithDifferentRoles_ReturnsToken()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "spuertab2", Password = "123" };

            // Act
            var token = _authService.Login(loginModel);

            // Assert
            Assert.NotNull(token);
        }
    }
}