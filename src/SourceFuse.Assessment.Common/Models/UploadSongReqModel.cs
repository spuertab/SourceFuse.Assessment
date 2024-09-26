using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SourceFuse.Assessment.Common.Models
{
    public class UploadSongReqModel
    {
        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; }

        public SongReqModel SongData { get; set; }
    }
}