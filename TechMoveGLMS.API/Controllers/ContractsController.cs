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
    public class ContractsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/contracts
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetContracts(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? status)
        {
            var query = _context.Contracts.Include(c => c.Client).AsQueryable();

            if (startDate.HasValue)
                query = query.Where(c => c.StartDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(c => c.EndDate <= endDate.Value);
            if (status.HasValue)
                query = query.Where(c => (int)c.Status == status.Value);

            var contracts = await query.Select(c => new ContractDTO
            {
                ContractId = c.ContractId,
                ClientId = c.ClientId,
                ClientName = c.Client.Name,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Status = c.Status.ToString(),
                ServiceLevel = c.ServiceLevel,
                SignedAgreementPath = c.SignedAgreementPath
            }).ToListAsync();

            return Ok(contracts);
        }

        // GET: api/contracts/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetContract(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null)
                return NotFound();

            return Ok(new ContractDTO
            {
                ContractId = contract.ContractId,
                ClientId = contract.ClientId,
                ClientName = contract.Client.Name,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status.ToString(),
                ServiceLevel = contract.ServiceLevel,
                SignedAgreementPath = contract.SignedAgreementPath
            });
        }

        // POST: api/contracts
        [HttpPost]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractDTO dto)
        {
            var contract = new Contract
            {
                ClientId = dto.ClientId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = (ContractStatus)dto.Status,
                ServiceLevel = dto.ServiceLevel,
                SignedAgreementPath = string.Empty
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContract), new { id = contract.ContractId }, contract);
        }

        // PATCH: api/contracts/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(int id, [FromBody] UpdateContractStatusDTO dto)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            contract.Status = (ContractStatus)dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Status updated successfully", status = contract.Status.ToString() });
        }

        // DELETE: api/contracts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Contract deleted successfully" });
        }
    }
}