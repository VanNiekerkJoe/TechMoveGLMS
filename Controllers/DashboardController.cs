using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalClients = await _context.Clients.CountAsync();
            var totalContracts = await _context.Contracts.CountAsync();
            var totalRequests = await _context.ServiceRequests.CountAsync();
            var activeContracts = await _context.Contracts.CountAsync(c => c.Status == ContractStatus.Active);

            // Monthly contract data (from actual DB or fallback)
            var monthlyData = await _context.Contracts
                .GroupBy(c => c.StartDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .OrderBy(g => g.Month)
                .ToListAsync();

            string[] monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var labels = monthlyData.Select(m => monthNames[m.Month - 1]).ToArray();
            var values = monthlyData.Select(m => m.Count).ToArray();

            if (labels.Length == 0)
            {
                labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
                values = new[] { 0, 0, 0, 0, 0, 0 };
            }

            var statusDistribution = new
            {
                labels = new[] { "Active", "Draft", "Expired", "On Hold" },
                values = new[]
                {
                    await _context.Contracts.CountAsync(c => c.Status == ContractStatus.Active),
                    await _context.Contracts.CountAsync(c => c.Status == ContractStatus.Draft),
                    await _context.Contracts.CountAsync(c => c.Status == ContractStatus.Expired),
                    await _context.Contracts.CountAsync(c => c.Status == ContractStatus.OnHold)
                }
            };

            return Ok(new
            {
                totalClients,
                totalContracts,
                totalRequests,
                activeContracts,
                contractTrend = 12,
                monthlyContracts = new { labels, values },
                statusDistribution
            });
        }

        [HttpGet("recent-requests")]
        public async Task<IActionResult> GetRecentRequests()
        {
            var recentRequests = await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .OrderByDescending(sr => sr.CreatedDate)
                .Take(5)
                .Select(sr => new
                {
                    sr.Description,
                    sr.Status,
                    sr.CreatedDate,
                    ContractName = sr.Contract.ServiceLevel,
                    Date = sr.CreatedDate
                })
                .ToListAsync();

            return Ok(recentRequests);
        }
    }
}