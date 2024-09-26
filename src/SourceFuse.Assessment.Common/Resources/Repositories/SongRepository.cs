using Microsoft.EntityFrameworkCore;
using SourceFuse.Assessment.Common.Resources.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SourceFuse.Assessment.Common.Resources.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly MusicContext _context;

        public SongRepository(MusicContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Song>> GetSongsAsync()
        {
            return await _context.Songs.ToListAsync();
        }

        public async Task<Song> GetSongByIdAsync(Guid id)
        {
            return await _context.Songs.FindAsync(id);
        }

        public async Task AddSongAsync(Song song)
        {
            song.CreatedAt = DateTime.UtcNow;
            song.UpdatedAt = DateTime.UtcNow;

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSongAsync(Song song)
        {
            song.UpdatedAt = DateTime.UtcNow;

            _context.Entry(song).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSongAsync(Song song)
        {
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SongExistsAsync(Guid id)
        {
            return await _context.Songs.AnyAsync(e => e.SongId == id);
        }
    }
}