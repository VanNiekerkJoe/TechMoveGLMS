using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ClientsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients.ToListAsync();
            return View(clients);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ContactDetails,Region")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Client created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,Name,ContactDetails,Region")] Client client)
        {
            if (id != client.ClientId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Client updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null) return NotFound();

            // Warn if client has linked contracts
            var contractCount = await _context.Contracts.CountAsync(c => c.ClientId == id);
            ViewBag.ContractCount = contractCount;

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                // Load and delete all linked contracts (and their PDF files) first
                var contracts = await _context.Contracts
                    .Where(c => c.ClientId == id)
                    .ToListAsync();

                foreach (var contract in contracts)
                {
                    // Delete associated PDF file if it exists
                    if (!string.IsNullOrEmpty(contract.SignedAgreementPath))
                    {
                        string filePath = Path.Combine(
                            _env.WebRootPath,
                            contract.SignedAgreementPath.TrimStart('/'));

                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);
                    }
                }

                // Remove contracts first, then client
                _context.Contracts.RemoveRange(contracts);
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Client and {contracts.Count} associated contract(s) deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }
    }
}