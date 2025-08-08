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
    public class KichCoController : ControllerBase
    {
        private readonly IKichCoService _service;

        public KichCoController(IKichCoService service)
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
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound("Không tìm thấy kích cỡ!");
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KichCoDTO dto)
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
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.KichCoId }, created);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("TenKichCo", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("TenKichCo", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo kích cỡ: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] KichCoDTO dto)
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
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (!result)
                    return NotFound("Không tìm thấy kích cỡ!");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("TenKichCo", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("TenKichCo", ex.Message);
                return new BadRequestObjectResult(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật kích cỡ: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound("Không tìm thấy kích cỡ!");

            return NoContent();
        }
    }
}