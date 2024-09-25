namespace SourceFuse.Assessment.Common.Resources.Entities
{
    public class Song
    {
        public Guid SongId { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Singer { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string S3Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
