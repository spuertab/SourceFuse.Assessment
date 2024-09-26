using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SourceFuse.Assessment.Api.Controllers;
using SourceFuse.Assessment.Common.Models;
using SourceFuse.Assessment.Common.Services;

namespace SourceFuse.Assessment.Tests.Api.Controllers
{
    [TestFixture]
    public class SongsControllerTests
    {
        private Mock<ISongService> _songServiceMock;
        private Mock<ILogger<SongsController>> _loggerMock;
        private SongsController _songsController;

        [SetUp]
        public void Setup()
        {
            _songServiceMock = new Mock<ISongService>();
            _loggerMock = new Mock<ILogger<SongsController>>();
            _songsController = new SongsController(_songServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetSongs_ReturnsOkResult_WithListOfSongs()
        {
            // Arrange
            var songs = new List<SongRespModel>
            {
                new SongRespModel { SongId = Guid.NewGuid(), Title = "Song 1" },
                new SongRespModel { SongId = Guid.NewGuid(), Title = "Song 2" }
            };
            _songServiceMock.Setup(service => service.GetSongsAsync()).ReturnsAsync(songs);

            // Act
            var result = await _songsController.GetSongs();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<IEnumerable<SongRespModel>>(okResult.Value);
            Assert.AreEqual(2, ((IEnumerable<SongRespModel>)okResult.Value).Count());
        }

        [Test]
        public async Task GetSong_ReturnsOkResult_WithSong()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var songRespModel = new SongRespModel { SongId = songId, Title = "Test Song" };
            _songServiceMock.Setup(service => service.GetSongByIdAsync(songId)).ReturnsAsync(songRespModel);

            // Act
            var result = await _songsController.GetSong(songId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<SongRespModel>(okResult.Value);
            Assert.AreEqual(songId, ((SongRespModel)okResult.Value).SongId);
        }

        [Test]
        public async Task GetSong_ReturnsNotFound_WhenSongDoesNotExist()
        {
            // Arrange
            var songId = Guid.NewGuid();
            _songServiceMock.Setup(service => service.GetSongByIdAsync(songId)).ReturnsAsync((SongRespModel)null);

            // Act
            var result = await _songsController.GetSong(songId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task PostSong_ReturnsCreatedAtActionResult_WithSong()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var song = new SongReqModel { SongId = songId, Title = "New Song" };
            var songResp = new SongRespModel { SongId = songId, Title = "New Song" };
            var fileMock = new Mock<IFormFile>();
            _songServiceMock.Setup(service => service.AddSongAsync(fileMock.Object, song)).ReturnsAsync(songResp);

            // Act
            var uploadSong = new UploadSongReqModel
            {
                SongData = song,
                File = fileMock.Object
            };
            var result = await _songsController.PostSong(uploadSong);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.IsInstanceOf<SongRespModel>(createdAtActionResult.Value);
            Assert.AreEqual(song.SongId, ((SongRespModel)createdAtActionResult.Value).SongId);
        }

        [Test]
        public async Task PutSong_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var song = new SongReqModel { SongId = songId, Title = "Updated Song" };

            // Act
            var result = await _songsController.PutSong(songId, song);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteSong_ReturnsNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var song = new SongRespModel { SongId = songId, Title = "Song to Delete" };
            _songServiceMock.Setup(service => service.GetSongByIdAsync(songId)).ReturnsAsync(song);

            // Act
            var result = await _songsController.DeleteSong(songId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}