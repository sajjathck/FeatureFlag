using FeatureFlags.Api.DTOs;
using FeatureFlags.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlags.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlagsController : ControllerBase
    {
        private readonly IFlagService _service;

        public FlagsController(IFlagService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _service.ListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFlagDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlagDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpPost("{id}/toggle")]
        public async Task<IActionResult> Toggle(int id)
        {
            var toggled = await _service.ToggleAsync(id);
            if (toggled == null) return NotFound();
            return Ok(toggled);
        }

        [HttpGet("evaluate")]
        public async Task<IActionResult> Evaluate([FromQuery] string flagName, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(flagName) || string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { error = "flagName and userId are required" });
            }
            var res = await _service.EvaluateAsync(flagName, userId);
            return Ok(res);
        }
    }
}
