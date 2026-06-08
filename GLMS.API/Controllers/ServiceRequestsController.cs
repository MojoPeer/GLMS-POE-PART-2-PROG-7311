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
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _service;

        public ServiceRequestsController(IServiceRequestService service)
        {
            _service = service;
        }

        /// <summary>Returns all service requests.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        /// <summary>Returns a single service request by ID.</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Creates a new service request.
        /// Returns 422 if the contract is Draft, Expired, or On Hold.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.AddAsync(request);
            if (!created)
                return UnprocessableEntity(new { message = "Cannot create service request: contract is Draft, Expired, or On Hold." });
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
        }

        /// <summary>Updates an existing service request.</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceRequest request)
        {
            if (id != request.Id) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(request);
            if (!updated)
                return UnprocessableEntity(new { message = "Cannot update service request: contract is Draft, Expired, or On Hold." });
            return NoContent();
        }

        /// <summary>Deletes a service request by ID.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
