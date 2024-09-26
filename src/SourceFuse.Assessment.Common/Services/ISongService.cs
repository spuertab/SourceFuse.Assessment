using Microsoft.AspNetCore.Http;
using SourceFuse.Assessment.Common.Models;

namespace SourceFuse.Assessment.Common.Services
{
    public interface ISongService
    {
        Task<IEnumerable<SongRespModel>> GetSongsAsync();
        Task<SongRespModel> GetSongByIdAsync(Guid id);
        Task<SongRespModel> AddSongAsync(IFormFile file, SongReqModel song);
        Task UpdateSongAsync(Guid id, SongReqModel song);
        Task DeleteSongAsync(Guid id);
    }
}
