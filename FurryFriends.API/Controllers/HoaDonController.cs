using FurryFriends.API.Models;
using FurryFriends.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FurryFriends.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonRepository _hoaDonRepository;

        public HoaDonController(IHoaDonRepository hoaDonRepository)
        {
            _hoaDonRepository = hoaDonRepository;
        }

        // GET: api/HoaDon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDon>>> GetHoaDons()
        {
            try
            {
                var hoaDons = await _hoaDonRepository.GetHoaDonListAsync();
                return Ok(hoaDons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/HoaDon/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HoaDon>> GetHoaDon(Guid id)
        {
            try
            {
                var hoaDon = await _hoaDonRepository.GetHoaDonByIdAsync(id);
                if (hoaDon == null)
                {
                    return NotFound($"Không tìm thấy hóa đơn với ID: {id}");
                }
                return Ok(hoaDon);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/HoaDon/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<HoaDon>>> SearchHoaDons([FromQuery] string keyword)
        {
            try
            {
                var hoaDons = await _hoaDonRepository.SearchHoaDonAsync(h => 
                    h.TenCuaKhachHang.Contains(keyword) || 
                    h.SdtCuaKhachHang.Contains(keyword) ||
                    h.EmailCuaKhachHang.Contains(keyword));
                return Ok(hoaDons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/HoaDon/{id}/pdf
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> ExportHoaDonToPdf(Guid id)
        {
            try
            {
                var pdfBytes = await _hoaDonRepository.ExportHoaDonToPdfAsync(id);
                if (pdfBytes == null)
                {
                    return NotFound($"Không tìm thấy hóa đơn với ID: {id}");
                }

                return File(pdfBytes, "application/pdf", $"HoaDon_{id}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 