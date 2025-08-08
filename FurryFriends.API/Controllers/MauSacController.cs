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
    public class MauSacController : ControllerBase
    {
        private readonly IMauSacService _service;

        public MauSacController(IMauSacService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MauSacDTO dto)
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

            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            try
            {
                var created = await _service.CreateAsync(dto);
                if (created == null) return BadRequest("Không thể tạo màu sắc");
                return CreatedAtAction(nameof(GetById), new { id = created.MauSacId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MauSacDTO dto)
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

            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (!result) return NotFound("Không tìm thấy màu sắc!");
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
                return NotFound();

            return NoContent();
        }
    }
}