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
    public class ChatLieuController : ControllerBase
    {
        private readonly IChatLieuService _service;

        public ChatLieuController(IChatLieuService service)
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
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound("Không tìm thấy chất liệu!");
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChatLieuDTO dto)
        {
            // Explicit validation
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(dto);

            if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? string.Empty);
                    }
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.ChatLieuId }, created);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("TenChatLieu", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("TenChatLieu", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo chất liệu: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ChatLieuDTO dto)
        {
            // Explicit validation
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(dto);

            if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    foreach (var memberName in validationResult.MemberNames)
                    {
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? string.Empty);
                    }
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (!result)
                    return NotFound("Không tìm thấy chất liệu!");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("TenChatLieu", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("TenChatLieu", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật chất liệu: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result)
                    return NotFound("Không tìm thấy chất liệu!");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa chất liệu: {ex.Message}");
            }
        }
    }
}