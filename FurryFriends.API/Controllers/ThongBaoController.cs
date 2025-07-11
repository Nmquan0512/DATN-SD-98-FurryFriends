using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongBaoController : ControllerBase
    {
        private readonly IThongBaoRepository _repo;
        public ThongBaoController(IThongBaoRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tb = await _repo.GetByIdAsync(id);
            if (tb == null) return NotFound();
            return Ok(tb);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ThongBao thongBao)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _repo.AddAsync(thongBao);
            return Ok(thongBao);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ThongBao thongBao)
        {
            if (id != thongBao.ThongBaoId) return BadRequest();
            await _repo.UpdateAsync(thongBao);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _repo.MarkAsReadAsync(id);
            return NoContent();
        }
    }
} 