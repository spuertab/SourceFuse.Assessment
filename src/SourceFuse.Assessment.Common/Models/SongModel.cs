using System.ComponentModel.DataAnnotations;

namespace SourceFuse.Assessment.Common.Models
{
    public class SongModel
    {
        [Required]
        public Guid SongId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Album { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Singer { get; set; }

        [StringLength(50)]
        public string Genre { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        public string S3Url { get; set; }
    }
}