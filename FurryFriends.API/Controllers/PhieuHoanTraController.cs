using FurryFriends.API.Data;
using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuHoanTraController : ControllerBase
    {
        private readonly IPhieuHoanTraService _service;

        public PhieuHoanTraController(IPhieuHoanTraService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var phieu = await _service.GetByIdAsync(id);
            if (phieu == null) return NotFound();
            return Ok(phieu);
        }

        [HttpGet("khachhang/{khachHangId}")]
        public async Task<IActionResult> GetByKhachHang(Guid khachHangId)
        {
            var list = await _service.GetByKhachHangAsync(khachHangId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhieuHoanTraCreateRequest request)
        {
            var result = await _service.CreateAsync(request);
            if (result) return Ok();
            return BadRequest("Tạo phiếu hoàn trả thất bại");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PhieuHoanTraUpdateRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            if (result) return Ok();
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (result) return Ok();
            return NotFound();
        }
    }
}
