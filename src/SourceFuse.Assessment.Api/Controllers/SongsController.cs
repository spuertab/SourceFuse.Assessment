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
        private readonly ILogger<SongsController> _logger;

        public SongsController(ISongService songService, ILogger<SongsController> logger)
        {
            _songService = songService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<SongModel>>> GetSongs()
        {
            _logger.LogInformation("Fetching all songs.");
            var songs = await _songService.GetSongsAsync();
            _logger.LogInformation("Fetched {Count} songs.", songs.Count());
            return Ok(songs);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<SongModel>> GetSong(Guid id)
        {
            _logger.LogInformation("Fetching song with ID: {Id}", id);
            var song = await _songService.GetSongByIdAsync(id);

            if (song == null)
            {
                _logger.LogWarning("Song with ID: {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Fetched song with ID: {Id}", id);
            return Ok(song);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SongModel>> PostSong([FromForm] IFormFile file, [FromForm] SongModel song)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding a new song with title: {Title}", song.Title);
            var createdSong = await _songService.AddSongAsync(file, song);
            _logger.LogInformation("Added new song with ID: {Id}", createdSong.SongId);
            return CreatedAtAction(nameof(GetSong), new { id = createdSong.SongId }, createdSong);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutSong(Guid id, SongModel song)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating song with ID: {Id}", id);
            await _songService.UpdateSongAsync(id, song);
            _logger.LogInformation("Updated song with ID: {Id}", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSong(Guid id)
        {
            _logger.LogInformation("Deleting song with ID: {Id}", id);
            await _songService.DeleteSongAsync(id);
            _logger.LogInformation("Deleted song with ID: {Id}", id);
            return NoContent();
        }
    }
}