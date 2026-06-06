using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;

namespace TechMoveGLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IFileService _fileService;

        public ContractsController(AppDbContext context, IWebHostEnvironment env, IFileService fileService)
        {
            _context = context;
            _env = env;
            _fileService = fileService;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var contracts = _context.Contracts.Include(c => c.Client).AsQueryable();

            if (startDate.HasValue)
                contracts = contracts.Where(c => c.StartDate >= startDate.Value);
            if (endDate.HasValue)
                contracts = contracts.Where(c => c.EndDate <= endDate.Value);
            if (status.HasValue)
                contracts = contracts.Where(c => c.Status == status.Value);

            return View(await contracts.ToListAsync());
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name");
            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile signedAgreement)
        {
            ModelState.Remove("SignedAgreementPath");
            ModelState.Remove("ServiceRequests");
            ModelState.Remove("Client");

            if (ModelState.IsValid)
            {
                try
                {
                    contract.SignedAgreementPath = string.Empty;
                    _context.Contracts.Add(contract);
                    await _context.SaveChangesAsync();

                    if (signedAgreement != null && signedAgreement.Length > 0)
                    {
                        var extension = Path.GetExtension(signedAgreement.FileName).ToLower();
                        if (extension != ".pdf")
                        {
                            ModelState.AddModelError("signedAgreement", "Only PDF files are allowed.");
                            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
                            return View(contract);
                        }

                        if (signedAgreement.Length > 10 * 1024 * 1024)
                        {
                            ModelState.AddModelError("signedAgreement", "File size cannot exceed 10MB.");
                            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
                            return View(contract);
                        }

                        string uploadsFolder = Path.Combine(_env.WebRootPath, "contracts");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = $"{contract.ContractId}_{Guid.NewGuid()}.pdf";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await signedAgreement.CopyToAsync(stream);
                        }

                        contract.SignedAgreementPath = $"/contracts/{uniqueFileName}";
                        _context.Contracts.Update(contract);
                        await _context.SaveChangesAsync();
                    }

                    TempData["Success"] = "Contract created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating contract: {ex.Message}");
                }
            }

            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile signedAgreement)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            ModelState.Remove("Client");
            ModelState.Remove("ServiceRequests");
            ModelState.Remove("SignedAgreementPath");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingContract = await _context.Contracts.FindAsync(id);
                    if (existingContract == null)
                    {
                        return NotFound();
                    }

                    // Update properties
                    existingContract.ClientId = contract.ClientId;
                    existingContract.StartDate = contract.StartDate;
                    existingContract.EndDate = contract.EndDate;
                    existingContract.Status = contract.Status;
                    existingContract.ServiceLevel = contract.ServiceLevel;

                    // Handle new file upload
                    if (signedAgreement != null && signedAgreement.Length > 0)
                    {
                        var extension = Path.GetExtension(signedAgreement.FileName).ToLower();
                        if (extension != ".pdf")
                        {
                            ModelState.AddModelError("signedAgreement", "Only PDF files are allowed.");
                            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
                            return View(contract);
                        }

                        // Delete old file
                        if (!string.IsNullOrEmpty(existingContract.SignedAgreementPath))
                        {
                            string oldFilePath = Path.Combine(_env.WebRootPath, existingContract.SignedAgreementPath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Save new file
                        string uploadsFolder = Path.Combine(_env.WebRootPath, "contracts");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = $"{existingContract.ContractId}_{Guid.NewGuid()}.pdf";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await signedAgreement.CopyToAsync(stream);
                        }

                        existingContract.SignedAgreementPath = $"/contracts/{uniqueFileName}";
                    }

                    _context.Update(existingContract);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Contract updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ContractId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                // Delete associated PDF file
                if (!string.IsNullOrEmpty(contract.SignedAgreementPath))
                {
                    string filePath = Path.Combine(_env.WebRootPath, contract.SignedAgreementPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Contract deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/Download
        public IActionResult Download(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return NotFound();
            }

            string fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/pdf", Path.GetFileName(fullPath));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}