using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LichSuThaoTacController : ControllerBase
    {
        private readonly ILichSuThaoTacRepository _repo;

        public LichSuThaoTacController(ILichSuThaoTacRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int take = 5)
        {
            try
            {
                var result = await _repo.GetRecentAsync(take);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] LichSuThaoTac log)
        {
            await _repo.AddAsync(log);
            return Ok();
        }
    }
}
