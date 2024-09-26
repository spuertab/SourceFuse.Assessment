using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SourceFuse.Assessment.Common.Models
{
    public class UploadSongModel
    {
        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; }

        public SongModel SongData { get; set; }
    }
}