using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Services;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly IApiService _apiService;

        public ClientsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = await _apiService.GetAsync<List<Client>>("api/Clients");
            return View(clients ?? new List<Client>());
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                var result = await _apiService.PostAsync<Client>("api/Clients", client);
                if (result != null)
                {
                    TempData["Success"] = "Client created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create client.");
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var client = await _apiService.GetAsync<Client>($"api/Clients/{id}");
            if (client == null) return NotFound();
            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.ClientId) return NotFound();
            if (ModelState.IsValid)
            {
                var result = await _apiService.PutAsync<Client>($"api/Clients/{id}", client);
                if (result != null)
                {
                    TempData["Success"] = "Client updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to update client.");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var client = await _apiService.GetAsync<Client>($"api/Clients/{id}");
            if (client == null) return NotFound();
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _apiService.DeleteAsync($"api/Clients/{id}");
            if (success)
            {
                TempData["Success"] = "Client deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete client.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            var clients = _apiService.GetAsync<List<Client>>("api/Clients").Result;
            return clients?.Any(c => c.ClientId == id) ?? false;
        }
    }
}