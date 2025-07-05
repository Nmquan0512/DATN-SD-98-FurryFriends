using FurryFriends.API.Models;
using FurryFriends.API.Models.DTO;
using FurryFriends.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SanPhamController : ControllerBase
    {
        private readonly ISanPhamRepository _repository;

        public SanPhamController(ISanPhamRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SanPhamDTO>>> GetAll()
        {
            var list = await _repository.GetAllAsync();
            var result = new List<SanPhamDTO>();
            foreach (var item in list)
            {
                result.Add(new SanPhamDTO
                {
                    SanPhamId = item.SanPhamId,
                    TenSanPham = item.TenSanPham,
                    TaiKhoanId = item.TaiKhoanId,
                    ThuongHieuId = item.ThuongHieuId,
                    TrangThai = item.TrangThai
                });
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SanPhamDTO>> GetById(Guid id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return NotFound("Không tìm thấy sản phẩm!");

            var dto = new SanPhamDTO
            {
                SanPhamId = item.SanPhamId,
                TenSanPham = item.TenSanPham,
                TaiKhoanId = item.TaiKhoanId,
                ThuongHieuId = item.ThuongHieuId,
                TrangThai = item.TrangThai
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] SanPhamDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var entity = new SanPham
                {
                    SanPhamId = Guid.NewGuid(),
                    TenSanPham = dto.TenSanPham,
                    TaiKhoanId = dto.TaiKhoanId,
                    ThuongHieuId = dto.ThuongHieuId,
                    TrangThai = dto.TrangThai
                };
                await _repository.AddAsync(entity);
                dto.SanPhamId = entity.SanPhamId;
                return CreatedAtAction(nameof(GetById), new { id = entity.SanPhamId }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] SanPhamDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _repository.ExistsAsync(id))
                return NotFound("Không tìm thấy sản phẩm!");

            try
            {
                var entity = new SanPham
                {
                    SanPhamId = id,
                    TenSanPham = dto.TenSanPham,
                    TaiKhoanId = dto.TaiKhoanId,
                    ThuongHieuId = dto.ThuongHieuId,
                    TrangThai = dto.TrangThai
                };
                await _repository.UpdateAsync(entity);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            if (!await _repository.ExistsAsync(id))
                return NotFound("Không tìm thấy sản phẩm!");

            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }
    }
}