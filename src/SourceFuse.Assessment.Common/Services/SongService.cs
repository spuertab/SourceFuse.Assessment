using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SourceFuse.Assessment.Common.Resources.Entities;
using SourceFuse.Assessment.Common.Resources.Repositories;
using SourceFuse.Assessment.Common.Models;

namespace SourceFuse.Assessment.Common.Services
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;
        private readonly IAmazonS3 _s3Client;
        private readonly IMapper _mapper;
        private readonly string _bucketName;

        public SongService(ISongRepository songRepository, IAmazonS3 s3Client, IConfiguration configuration, IMapper mapper)
        {
            _songRepository = songRepository;
            _s3Client = s3Client;
            _mapper = mapper;
            _bucketName = configuration["AWS:BucketName"];
        }

        public async Task<IEnumerable<SongModel>> GetSongsAsync()
        {
            var songs = await _songRepository.GetSongsAsync();
            return _mapper.Map<IEnumerable<SongModel>>(songs);
        }

        public async Task<SongModel> GetSongByIdAsync(Guid id)
        {
            var song = await _songRepository.GetSongByIdAsync(id);
            return _mapper.Map<SongModel>(song);
        }

        public async Task<SongModel> AddSongAsync(IFormFile file, SongModel songModel)
        {
            if (!IsMusicFile(file))
            {
                throw new ArgumentException("The file must be a valid music format.");
            }

            var song = _mapper.Map<Song>(songModel);

            var key = $"{Guid.NewGuid()}_{file.FileName}";
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(putRequest);

            song.S3Url = $"https://{_bucketName}.s3.amazonaws.com/{key}";
            await _songRepository.AddSongAsync(song);

            return _mapper.Map<SongModel>(song);
        }

        public async Task UpdateSongAsync(Guid id, SongModel songModel)
        {
            var song = _mapper.Map<Song>(songModel);

            if (id != song.SongId)
            {
                throw new ArgumentException("Song ID mismatch");
            }

            await _songRepository.UpdateSongAsync(song);
        }

        public async Task DeleteSongAsync(Guid id)
        {
            var song = await _songRepository.GetSongByIdAsync(id);
            if (song == null)
            {
                throw new KeyNotFoundException("Song not found");
            }

            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = song.S3Url.Split('/').Last()
            };

            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
            await _songRepository.DeleteSongAsync(song);
        }

        private bool IsMusicFile(IFormFile file)
        {
            if (file == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(file.ContentType) || string.IsNullOrEmpty(file.FileName))
            {
                return false;
            }

            var allowedMimeTypes = new List<string>
            {
                "audio/mpeg", // .mp3
                "audio/wav",  // .wav
                "audio/x-wav",
                "audio/x-ms-wma", // .wma
                "audio/aac",  // .aac
                "audio/ogg",  // .ogg
                "audio/flac", // .flac
                "audio/mp4",  // .m4a
                "audio/x-m4a"
            };

            var allowedExtensions = new List<string>
            {
                ".mp3",
                ".wav",
                ".wma",
                ".aac",
                ".ogg",
                ".flac",
                ".m4a"
            };

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()) &&
                   allowedExtensions.Contains(fileExtension);
        }
    }
}