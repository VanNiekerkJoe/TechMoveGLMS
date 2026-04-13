using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;
using System.Threading.Tasks;

namespace TechMoveGLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(AppDbContext context, ICurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index() => View(await _context.ServiceRequests.Include(sr => sr.Contract).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.ContractId = new SelectList(_context.Contracts, "ContractId", "ServiceLevel");
            var rate = await _currencyService.GetExchangeRateAsync("USD", "ZAR");
            ViewBag.ExchangeRate = rate;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,Description,CostUSD,CostZAR")] ServiceRequest serviceRequest)
        {
            var contract = await _context.Contracts.FindAsync(serviceRequest.ContractId);
            if (contract == null) return NotFound();

            // Business rule: cannot create service request for expired or on-hold contracts
            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            {
                ModelState.AddModelError("", "Service Requests cannot be created for Expired or On Hold contracts.");
                ViewBag.ContractId = new SelectList(_context.Contracts, "ContractId", "ServiceLevel", serviceRequest.ContractId);
                var rate = await _currencyService.GetExchangeRateAsync("USD", "ZAR");
                ViewBag.ExchangeRate = rate;
                return View(serviceRequest);
            }

            if (ModelState.IsValid)
            {
                serviceRequest.CreatedDate = System.DateTime.Now;
                serviceRequest.Status = ServiceRequestStatus.Pending;
                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ContractId = new SelectList(_context.Contracts, "ContractId", "ServiceLevel", serviceRequest.ContractId);
            var fallbackRate = await _currencyService.GetExchangeRateAsync("USD", "ZAR");
            ViewBag.ExchangeRate = fallbackRate;
            return View(serviceRequest);
        }
    }
}