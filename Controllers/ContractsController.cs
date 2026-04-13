using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;
using TechMoveGLMS.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TechMoveGLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _env;

        public ContractsController(AppDbContext context, IFileService fileService, IWebHostEnvironment env)
        {
            _context = context;
            _fileService = fileService;
            _env = env;
        }

        // GET: Contracts – with search/filter by date range and status
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

        public IActionResult Create()
        {
            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile signedAgreement)
        {
            if (ModelState.IsValid)
            {
                if (signedAgreement != null && _fileService.IsValidPdfFile(signedAgreement))
                {
                    _context.Add(contract);
                    await _context.SaveChangesAsync(); // get ContractId

                    var filePath = await _fileService.SavePdfFileAsync(signedAgreement, contract.ContractId.ToString());
                    contract.SignedAgreementPath = filePath;
                    _context.Update(contract);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("signedAgreement", "Only PDF files are allowed.");
            }
            ViewBag.ClientId = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        public async Task<IActionResult> Download(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return NotFound();
            string fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
            if (!System.IO.File.Exists(fullPath)) return NotFound();
            byte[] bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(bytes, "application/pdf", Path.GetFileName(fullPath));
        }
    }
}