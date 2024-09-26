using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using SourceFuse.Assessment.Common.Models;
using SourceFuse.Assessment.Common.Resources.Entities;
using SourceFuse.Assessment.Common.Resources.Repositories;
using SourceFuse.Assessment.Common.Services;

namespace SourceFuse.Assessment.Tests.Common.Services
{
    [TestFixture]
    public class SongServiceTests
    {
        private Mock<ISongRepository> _songRepositoryMock;
        private Mock<IAmazonS3> _s3ClientMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IConfiguration> _configurationMock;
        private SongService _songService;

        [SetUp]
        public void Setup()
        {
            _songRepositoryMock = new Mock<ISongRepository>();
            _s3ClientMock = new Mock<IAmazonS3>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();

            var bucketNameSectionMock = new Mock<IConfigurationSection>();
            bucketNameSectionMock.Setup(x => x.Value).Returns("test-bucket");
            _configurationMock.Setup(x => x["AWS:BucketName"]).Returns("test-bucket");

            _songService = new SongService(
                _songRepositoryMock.Object,
                _s3ClientMock.Object,
                _configurationMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task GetSongsAsync_ReturnsSongs()
        {
            // Arrange
            var songs = new List<Song> { new Song { SongId = Guid.NewGuid(), Title = "Test Song" } };
            _songRepositoryMock.Setup(repo => repo.GetSongsAsync()).ReturnsAsync(songs);
            _mapperMock.Setup(m => m.Map<IEnumerable<SongModel>>(It.IsAny<IEnumerable<Song>>()))
                       .Returns(new List<SongModel> { new SongModel { SongId = Guid.NewGuid(), Title = "Test Song" } });

            // Act
            var result = await _songService.GetSongsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task GetSongByIdAsync_ReturnsSong()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var song = new Song { SongId = songId, Title = "Test Song" };
            _songRepositoryMock.Setup(repo => repo.GetSongByIdAsync(songId)).ReturnsAsync(song);
            _mapperMock.Setup(m => m.Map<SongModel>(It.IsAny<Song>())).Returns(new SongModel { SongId = songId, Title = "Test Song" });

            // Act
            var result = await _songService.GetSongByIdAsync(songId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(songId, result.SongId);
        }

        [Test]
        public void AddSongAsync_InvalidFileFormat_ThrowsArgumentException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("audio/wav");
            var songModel = new SongModel { Title = "Test Song" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _songService.AddSongAsync(fileMock.Object, songModel));
            Assert.AreEqual("The file must be in MP3 format.", ex.Message);
        }

        [Test]
        public async Task AddSongAsync_ValidFile_AddsSong()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("audio/mpeg");
            fileMock.Setup(f => f.FileName).Returns("test.mp3");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var songModel = new SongModel { Title = "Test Song" };
            var song = new Song { SongId = Guid.NewGuid(), Title = "Test Song" };

            _mapperMock.Setup(m => m.Map<Song>(It.IsAny<SongModel>())).Returns(song);
            _s3ClientMock.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                         .ReturnsAsync(new PutObjectResponse());
            _songRepositoryMock.Setup(repo => repo.AddSongAsync(It.IsAny<Song>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<SongModel>(It.IsAny<Song>())).Returns(songModel);

            // Act
            var result = await _songService.AddSongAsync(fileMock.Object, songModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(songModel.Title, result.Title);
        }

        [Test]
        public void UpdateSongAsync_SongIdMismatch_ThrowsArgumentException()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var songModel = new SongModel { SongId = Guid.NewGuid(), Title = "Test Song" };
            var song = new Song { SongId = songModel.SongId, Title = songModel.Title };

            _mapperMock.Setup(m => m.Map<Song>(It.IsAny<SongModel>())).Returns(song);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _songService.UpdateSongAsync(songId, songModel));
            Assert.AreEqual("Song ID mismatch", ex.Message);
        }

        [Test]
        public async Task UpdateSongAsync_ValidSong_UpdatesSong()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var songModel = new SongModel { SongId = songId, Title = "Test Song" };
            var song = new Song { SongId = songId, Title = "Test Song" };

            _mapperMock.Setup(m => m.Map<Song>(It.IsAny<SongModel>())).Returns(song);
            _songRepositoryMock.Setup(repo => repo.UpdateSongAsync(It.IsAny<Song>())).Returns(Task.CompletedTask);

            // Act
            await _songService.UpdateSongAsync(songId, songModel);

            // Assert
            _songRepositoryMock.Verify(repo => repo.UpdateSongAsync(It.IsAny<Song>()), Times.Once);
        }

        [Test]
        public void DeleteSongAsync_SongNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var songId = Guid.NewGuid();
            _songRepositoryMock.Setup(repo => repo.GetSongByIdAsync(songId)).ReturnsAsync((Song)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _songService.DeleteSongAsync(songId));
            Assert.AreEqual("Song not found", ex.Message);
        }
    }
}