using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;
using System.Globalization;

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

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .OrderByDescending(sr => sr.CreatedDate)
                .ToListAsync();
            return View(serviceRequests);
        }

        // GET: ServiceRequests/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Only show Active and Draft contracts (not expired or on hold)
            var availableContracts = await _context.Contracts
                .Where(c => c.Status == ContractStatus.Active || c.Status == ContractStatus.Draft)
                .Include(c => c.Client)
                .ToListAsync();

            ViewBag.ContractId = new SelectList(availableContracts, "ContractId", "ServiceLevel");

            // Get exchange rate
            try
            {
                var rate = await _currencyService.GetExchangeRateAsync("USD", "ZAR");
                ViewBag.ExchangeRate = rate;
            }
            catch
            {
                ViewBag.ExchangeRate = 19.00m;
            }

            return View();
        }

        // POST: ServiceRequests/Create - FIXED DECIMAL PARSING
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            // ===== MANUAL DECIMAL PARSING - FIXES CULTURE ISSUE PERMANENTLY =====
            // Get CostUSD from form directly
            var costUSDFromForm = Request.Form["CostUSD"].ToString();
            decimal parsedCostUSD = 0;
            if (!string.IsNullOrEmpty(costUSDFromForm))
            {
                costUSDFromForm = costUSDFromForm.Replace(",", ".");
                decimal.TryParse(costUSDFromForm, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedCostUSD);
            }
            serviceRequest.CostUSD = parsedCostUSD;

            // Get CostZAR from form directly
            var costZARFromForm = Request.Form["CostZAR"].ToString();
            decimal parsedCostZAR = 0;
            if (!string.IsNullOrEmpty(costZARFromForm))
            {
                costZARFromForm = costZARFromForm.Replace(",", ".");
                decimal.TryParse(costZARFromForm, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedCostZAR);
            }
            serviceRequest.CostZAR = parsedCostZAR;

            // Debug logging
            System.Diagnostics.Debug.WriteLine($"=== CREATE REQUEST ===");
            System.Diagnostics.Debug.WriteLine($"Raw CostUSD: '{costUSDFromForm}' -> Parsed: {parsedCostUSD}");
            System.Diagnostics.Debug.WriteLine($"Raw CostZAR: '{costZARFromForm}' -> Parsed: {parsedCostZAR}");
            System.Diagnostics.Debug.WriteLine($"ContractId: {serviceRequest.ContractId}");
            System.Diagnostics.Debug.WriteLine($"Description: {serviceRequest.Description}");

            // Remove validation for properties we don't bind
            ModelState.Remove("Contract");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("Status");
            ModelState.Remove("CostUSD");
            ModelState.Remove("CostZAR");

            // Manual validation
            if (serviceRequest.ContractId <= 0)
            {
                ModelState.AddModelError("ContractId", "Please select a valid contract.");
            }

            if (string.IsNullOrWhiteSpace(serviceRequest.Description))
            {
                ModelState.AddModelError("Description", "Description is required.");
            }

            if (serviceRequest.CostUSD <= 0)
            {
                ModelState.AddModelError("CostUSD", "Please enter a valid USD amount greater than 0.");
            }

            // Check contract status
            var contract = await _context.Contracts.FindAsync(serviceRequest.ContractId);
            if (contract != null && (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold))
            {
                ModelState.AddModelError("", "Service Requests cannot be created for contracts that are Expired or On Hold.");
            }

            if (!ModelState.IsValid)
            {
                var availableContracts = await _context.Contracts
                    .Where(c => c.Status == ContractStatus.Active || c.Status == ContractStatus.Draft)
                    .ToListAsync();
                ViewBag.ContractId = new SelectList(availableContracts, "ContractId", "ServiceLevel", serviceRequest.ContractId);

                try
                {
                    var rate = await _currencyService.GetExchangeRateAsync("USD", "ZAR");
                    ViewBag.ExchangeRate = rate;
                }
                catch
                {
                    ViewBag.ExchangeRate = 19.00m;
                }
                return View(serviceRequest);
            }

            // Fallback: Calculate ZAR if not set properly
            if (serviceRequest.CostZAR == 0 && serviceRequest.CostUSD > 0)
            {
                try
                {
                    var rate = await _currencyService.GetExchangeRateAsync("USD", "ZAR");
                    serviceRequest.CostZAR = serviceRequest.CostUSD * rate;
                    System.Diagnostics.Debug.WriteLine($"ZAR calculated via API: {serviceRequest.CostZAR}");
                }
                catch
                {
                    serviceRequest.CostZAR = serviceRequest.CostUSD * 19.00m;
                    System.Diagnostics.Debug.WriteLine($"ZAR calculated via fallback: {serviceRequest.CostZAR}");
                }
            }

            // Set remaining properties
            serviceRequest.CreatedDate = DateTime.Now;
            serviceRequest.Status = ServiceRequestStatus.Pending;

            // Save to database
            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            System.Diagnostics.Debug.WriteLine($"SAVED - CostUSD: {serviceRequest.CostUSD}, CostZAR: {serviceRequest.CostZAR}");

            TempData["Success"] = "Service Request created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: ServiceRequests/UpdateStatus/5 (AJAX inline update)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            try
            {
                var serviceRequest = await _context.ServiceRequests.FindAsync(id);
                if (serviceRequest == null)
                {
                    return Json(new { success = false, message = "Service Request not found." });
                }

                // Validate status value
                if (status < 0 || status > 3)
                {
                    return Json(new { success = false, message = "Invalid status value." });
                }

                // Update status
                serviceRequest.Status = (ServiceRequestStatus)status;
                _context.Update(serviceRequest);
                await _context.SaveChangesAsync();

                string statusName = Enum.GetName(typeof(ServiceRequestStatus), status) ?? "Unknown";
                return Json(new { success = true, message = "Status updated successfully.", statusName = statusName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .FirstOrDefaultAsync(m => m.ServiceRequestId == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Service Request deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.ServiceRequestId == id);
        }
    }
}