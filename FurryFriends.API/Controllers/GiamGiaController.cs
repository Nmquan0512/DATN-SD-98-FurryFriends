using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiamGiaController : ControllerBase
    {
        private readonly IGiamGiaService _giamGiaService;

        public GiamGiaController(IGiamGiaService giamGiaService)
        {
            _giamGiaService = giamGiaService;
        }

        // GET: api/GiamGia
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var giamGias = await _giamGiaService.GetAllAsync();
            return Ok(giamGias);
        }

        // GET: api/GiamGia/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var giamGia = await _giamGiaService.GetByIdAsync(id);
            if (giamGia == null)
                return NotFound("Không tìm thấy mã giảm giá");

            return Ok(giamGia);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GiamGiaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                // Log lỗi ModelState
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key]?.Errors;
                    if (errors != null && errors.Count > 0)
                    {
                        foreach (var err in errors)
                        {
                            Console.WriteLine($"[ModelStateError] {key}: {err.ErrorMessage}");
                        }
                    }
                }
                return BadRequest(ModelState);
            }

            var created = await _giamGiaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.GiamGiaId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GiamGiaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.GiamGiaId)
                return BadRequest("ID không khớp với DTO");

            var updated = await _giamGiaService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound("Không tìm thấy mã giảm giá để cập nhật");

            return Ok(updated);
        }


        // POST: api/GiamGia/{id}/assign-sanphamchitiet
        [HttpPost("{id}/assign-sanphamchitiet")]
        public async Task<IActionResult> AssignSanPhamChiTiet(Guid id, [FromBody] List<Guid> sanPhamChiTietIds)
        {
            if (sanPhamChiTietIds == null || !sanPhamChiTietIds.Any())
                return BadRequest("Danh sách sản phẩm chi tiết không được rỗng.");

            var result = await _giamGiaService.AddSanPhamChiTietToGiamGiaAsync(id, sanPhamChiTietIds);
            if (!result)
                return BadRequest("Không thể gán sản phẩm chi tiết vào đợt giảm giá");

            return Ok("Gán sản phẩm chi tiết thành công");
        }
    }
}
