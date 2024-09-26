using Microsoft.EntityFrameworkCore;
using SourceFuse.Assessment.Common.Resources.Entities;
using SourceFuse.Assessment.Common.Resources.Extensions;

namespace SourceFuse.Assessment.Common.Resources
{
    public class MusicContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }

        public MusicContext(DbContextOptions<MusicContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyUtcDateTimeConverter();
        }
    }
}
