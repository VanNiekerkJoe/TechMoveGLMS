using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.API.Data;
using TechMoveGLMS.API.DTOs;
using TechMoveGLMS.API.Models;

namespace TechMoveGLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceRequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetServiceRequests()
        {
            var requests = await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .OrderByDescending(sr => sr.CreatedDate)
                .Select(sr => new ServiceRequestDTO
                {
                    ServiceRequestId = sr.ServiceRequestId,
                    ContractId = sr.ContractId,
                    ContractName = sr.Contract.ServiceLevel,
                    Description = sr.Description,
                    CostUSD = sr.CostUSD,
                    CostZAR = sr.CostZAR,
                    Status = sr.Status.ToString(),
                    CreatedDate = sr.CreatedDate
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetServiceRequest(int id)
        {
            var request = await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id);

            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest([FromBody] CreateServiceRequestDTO dto)
        {
            var contract = await _context.Contracts.FindAsync(dto.ContractId);
            if (contract == null)
                return BadRequest(new { message = "Invalid contract" });

            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
                return BadRequest(new { message = "Cannot create request for Expired or On Hold contracts" });

            var request = new ServiceRequest
            {
                ContractId = dto.ContractId,
                Description = dto.Description,
                CostUSD = dto.CostUSD,
                CostZAR = dto.CostZAR,
                Status = ServiceRequestStatus.Pending,
                CreatedDate = DateTime.Now
            };

            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServiceRequest), new { id = request.ServiceRequestId }, request);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int status)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            request.Status = (ServiceRequestStatus)status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Status updated successfully", status = request.Status.ToString() });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Service request deleted successfully" });
        }
    }
}