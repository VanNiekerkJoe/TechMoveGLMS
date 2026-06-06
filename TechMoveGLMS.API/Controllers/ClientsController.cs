using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.MVC.Services;

namespace TechMoveGLMS.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IApiService _apiService;

        public ClientsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _apiService.GetAsync<List<Client>>("api/clients");
            return View(clients ?? new List<Client>());
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                var result = await _apiService.PostAsync<Client>("api/clients", client);
                if (result != null)
                    TempData["Success"] = "Client created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = await _apiService.GetAsync<Client>($"api/clients/{id}");
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.ClientId) return NotFound();
            if (ModelState.IsValid)
            {
                var result = await _apiService.PutAsync<Client>($"api/clients/{id}", client);
                if (result != null)
                    TempData["Success"] = "Client updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = await _apiService.GetAsync<Client>($"api/clients/{id}");
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _apiService.DeleteAsync($"api/clients/{id}");
            if (success)
                TempData["Success"] = "Client deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }

    public class Client
    {
        public int ClientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactDetails { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }
}