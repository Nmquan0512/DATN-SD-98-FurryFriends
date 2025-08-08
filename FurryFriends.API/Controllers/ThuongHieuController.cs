using FurryFriends.API.Models.DTO;
using FurryFriends.API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FurryFriends.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThuongHieuController : ControllerBase
    {
        private readonly IThuongHieuService _service;

        public ThuongHieuController(IThuongHieuService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound("Không tìm thấy thương hiệu!");
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThuongHieuDTO dto)
        {
            // Explicitly validate the model
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "");
                    }
                }
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                if (created == null) return BadRequest("Không thể tạo thương hiệu");
                return CreatedAtAction(nameof(GetById), new { id = created.ThuongHieuId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ThuongHieuDTO dto)
        {
            // Explicitly validate the model
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "");
                    }
                }
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (!result) return NotFound("Không tìm thấy thương hiệu!");
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound("Không tìm thấy thương hiệu!");

            return NoContent();
        }
    }
}