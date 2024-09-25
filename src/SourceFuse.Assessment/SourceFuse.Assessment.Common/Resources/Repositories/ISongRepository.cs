using SourceFuse.Assessment.Common.Resources.Entities;

namespace SourceFuse.Assessment.Common.Resources.Repositories
{
    public interface ISongRepository
    {
        Task<IEnumerable<Song>> GetSongsAsync();
        Task<Song> GetSongByIdAsync(Guid id);
        Task AddSongAsync(Song song);
        Task UpdateSongAsync(Song song);
        Task DeleteSongAsync(Song song);
        Task<bool> SongExistsAsync(Guid id);
    }
}
