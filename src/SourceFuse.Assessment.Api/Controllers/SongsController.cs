using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SourceFuse.Assessment.Common.Models;
using SourceFuse.Assessment.Common.Services;

namespace SourceFuse.Assessment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongsController(ISongService songService)
        {
            _songService = songService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<SongModel>>> GetSongs()
        {
            var songs = await _songService.GetSongsAsync();
            return Ok(songs);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<SongModel>> GetSong(Guid id)
        {
            var song = await _songService.GetSongByIdAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            return Ok(song);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SongModel>> PostSong([FromForm] IFormFile file, [FromForm] SongModel song)
        {
            var createdSong = await _songService.AddSongAsync(file, song);
            return CreatedAtAction(nameof(GetSong), new { id = createdSong.SongId }, createdSong);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutSong(Guid id, SongModel song)
        {
            try
            {
                await _songService.UpdateSongAsync(id, song);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSong(Guid id)
        {
            try
            {
                await _songService.DeleteSongAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
