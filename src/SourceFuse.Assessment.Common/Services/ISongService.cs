using Microsoft.AspNetCore.Http;
using SourceFuse.Assessment.Common.Models;

namespace SourceFuse.Assessment.Common.Services
{
    public interface ISongService
    {
        Task<IEnumerable<SongModel>> GetSongsAsync();
        Task<SongModel> GetSongByIdAsync(Guid id);
        Task<SongModel> AddSongAsync(IFormFile file, SongModel song);
        Task UpdateSongAsync(Guid id, SongModel song);
        Task DeleteSongAsync(Guid id);
    }
}
