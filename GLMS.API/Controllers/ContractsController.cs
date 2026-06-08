// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.API.Models;
using GLMS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _service;

        public ContractsController(IContractService service)
        {
            _service = service;
        }

        /// <summary>Returns all contracts, optionally filtered by status, startDate, and endDate.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var contracts = await _service.GetAllAsync(status, startDate, endDate);
            return Ok(contracts);
        }

        /// <summary>Returns a single contract by ID.</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contract = await _service.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return Ok(contract);
        }

        /// <summary>Creates a new contract.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contract contract)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.AddAsync(contract);
            return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
        }

        /// <summary>Updates an existing contract.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contract contract)
        {
            if (id != contract.Id) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _service.UpdateAsync(contract);
            return NoContent();
        }

        /// <summary>Deletes a contract by ID.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _service.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>Updates only the status of a contract (PATCH).</summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateRequest request)
        {
            var updated = await _service.UpdateStatusAsync(id, request.Status);
            if (!updated) return NotFound();
            return NoContent();
        }
    }

    public record StatusUpdateRequest(string Status);
}
