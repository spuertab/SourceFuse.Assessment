using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SourceFuse.Assessment.Api.Middlewares;
using System.Net;

namespace SourceFuse.Assessment.Tests.Api.Middlewares
{
    [TestFixture]
    public class ExceptionHandlingMiddlewareTests
    {
        private Mock<RequestDelegate> _nextMock;
        private Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
        private ExceptionHandlingMiddleware _middleware;

        [SetUp]
        public void Setup()
        {
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task InvokeAsync_LogsUnauthorizedAccess()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unauthorized access attempt to")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task InvokeAsync_LogsBadRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Bad request made to")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task InvokeAsync_LogsForbiddenRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Forbidden request made to")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}