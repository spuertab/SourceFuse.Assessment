using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SourceFuse.Assessment.Common.Models;
using SourceFuse.Assessment.Common.Resources.Entities;
using SourceFuse.Assessment.Common.Resources.Repositories;
using SourceFuse.Assessment.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SourceFuse.Assessment.Tests.Common.Services
{
    [TestFixture]
    public class SongServiceTests
    {
        private Mock<ISongRepository> _songRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IAmazonS3> _s3ClientMock;
        private SongService _songService;

        [SetUp]
        public void Setup()
        {
            _songRepositoryMock = new Mock<ISongRepository>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();
            _s3ClientMock = new Mock<IAmazonS3>();

            _configurationMock.Setup(x => x["AWS:BucketName"]).Returns("test-bucket");
            _configurationMock.Setup(x => x["AWS:Region"]).Returns("us-west-2");
            _configurationMock.Setup(x => x["AWS:AccessKey"]).Returns("test-access-key");
            _configurationMock.Setup(x => x["AWS:SecretKey"]).Returns("test-secret-key");

            _songService = new SongService(
                _songRepositoryMock.Object,
                _configurationMock.Object,
                _mapperMock.Object
            );

            // Reemplazar el cliente S3 real con el mock
            var awsOptions = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("us-west-2")
            };

            var awsCredentials = new BasicAWSCredentials("test-access-key", "test-secret-key");

            var s3ClientField = typeof(SongService).GetField("_s3Client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            s3ClientField.SetValue(_songService, _s3ClientMock.Object);
        }

        [Test]
        public async Task GetSongsAsync_ReturnsSongs()
        {
            // Arrange
            var songs = new List<Song> { new Song { SongId = Guid.NewGuid(), Title = "Test Song", S3Url = "test-key" } };
            var songModels = new List<SongRespModel> { new SongRespModel { SongId = Guid.NewGuid(), Title = "Test Song", S3Url = "test-key" } };

            _songRepositoryMock.Setup(repo => repo.GetSongsAsync()).ReturnsAsync(songs);
            _mapperMock.Setup(m => m.Map<IEnumerable<SongRespModel>>(It.IsAny<IEnumerable<Song>>())).Returns(songModels);
            _s3ClientMock.Setup(s3 => s3.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns("https://signed-url");

            // Act
            var result = await _songService.GetSongsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual("https://signed-url", result.First().S3Url);
        }

        [Test]
        public async Task GetSongByIdAsync_ReturnsSong()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var song = new Song { SongId = songId, Title = "Test Song", S3Url = "test-key" };
            var songModel = new SongRespModel { SongId = songId, Title = "Test Song", S3Url = "test-key" };

            _songRepositoryMock.Setup(repo => repo.GetSongByIdAsync(songId)).ReturnsAsync(song);
            _mapperMock.Setup(m => m.Map<SongRespModel>(It.IsAny<Song>())).Returns(songModel);
            _s3ClientMock.Setup(s3 => s3.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns("https://signed-url");

            // Act
            var result = await _songService.GetSongByIdAsync(songId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(songId, result.SongId);
            Assert.AreEqual("https://signed-url", result.S3Url);
        }

        [Test]
        public void AddSongAsync_InvalidFileFormat_ThrowsArgumentException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("audio/wav");
            var songModel = new SongReqModel { Title = "Test Song" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _songService.AddSongAsync(fileMock.Object, songModel));
            Assert.AreEqual("The file must be a valid music format.", ex.Message);
        }

        [Test]
        public async Task AddSongAsync_ValidFile_AddsSong()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("audio/mpeg");
            fileMock.Setup(f => f.FileName).Returns("test.mp3");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var songModel = new SongReqModel { Title = "Test Song" };
            var song = new Song { SongId = Guid.NewGuid(), Title = "Test Song", S3Url = "test-key" };

            _mapperMock.Setup(m => m.Map<Song>(It.IsAny<SongReqModel>())).Returns(song);
            _s3ClientMock.Setup(s3 => s3.PutObjectAsync(It.IsAny<PutObjectRequest>(), default))
                         .ReturnsAsync(new PutObjectResponse());
            _songRepositoryMock.Setup(repo => repo.AddSongAsync(It.IsAny<Song>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<SongRespModel>(It.IsAny<Song>())).Returns(new SongRespModel { Title = "Test Song", S3Url = "https://signed-url" });
            _s3ClientMock.Setup(s3 => s3.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns("https://signed-url");

            // Act
            var result = await _songService.AddSongAsync(fileMock.Object, songModel);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(songModel.Title, result.Title);
            Assert.AreEqual("https://signed-url", result.S3Url);
        }

        [Test]
        public void AddSongAsync_SongIdAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("audio/mpeg");
            fileMock.Setup(f => f.FileName).Returns("test.mp3");
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var songModel = new SongReqModel { SongId = songId, Title = "Test Song" };
            var existingSong = new Song { SongId = songId, Title = "Existing Song" };

            _songRepositoryMock.Setup(repo => repo.GetSongByIdAsync(songId)).ReturnsAsync(existingSong);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _songService.AddSongAsync(fileMock.Object, songModel));
            Assert.AreEqual("A song with the same ID already exists.", ex.Message);
        }

        [Test]
        public void UpdateSongAsync_SongIdMismatch_ThrowsArgumentException()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var songModel = new SongReqModel { SongId = Guid.NewGuid(), Title = "Test Song" };
            var song = new Song { SongId = songModel.SongId, Title = songModel.Title };

            _mapperMock.Setup(m => m.Map<Song>(It.IsAny<SongReqModel>())).Returns(song);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _songService.UpdateSongAsync(songId, songModel));
            Assert.AreEqual("Song ID mismatch", ex.Message);
        }

        [Test]
        public async Task UpdateSongAsync_ValidSong_UpdatesSong()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var songModel = new SongReqModel { SongId = songId, Title = "Test Song" };
            var song = new Song { SongId = songId, Title = "Test Song" };

            _mapperMock.Setup(m => m.Map<Song>(It.IsAny<SongReqModel>())).Returns(song);
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

        [Test]
        public async Task DeleteSongAsync_ValidSong_DeletesSong()
        {
            // Arrange
            var songId = Guid.NewGuid();
            var song = new Song { SongId = songId, S3Url = "test-key" };

            _songRepositoryMock.Setup(repo => repo.GetSongByIdAsync(songId)).ReturnsAsync(song);
            _songRepositoryMock.Setup(repo => repo.DeleteSongAsync(It.IsAny<Song>())).Returns(Task.CompletedTask);
            _s3ClientMock.Setup(s3 => s3.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), default))
                         .ReturnsAsync(new DeleteObjectResponse());

            // Act
            await _songService.DeleteSongAsync(songId);

            // Assert
            _songRepositoryMock.Verify(repo => repo.DeleteSongAsync(It.IsAny<Song>()), Times.Once);
            _s3ClientMock.Verify(s3 => s3.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), default), Times.Once);
        }
    }
}