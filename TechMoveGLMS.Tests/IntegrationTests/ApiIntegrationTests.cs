using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace TechMoveGLMS.Tests.IntegrationTests
{
    public class ApiIntegrationTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;
        private string? _authToken;

        public ApiIntegrationTests(TestFixture fixture)
        {
            _client = fixture.Client;
        }

        private async Task<string> GetAuthToken()
        {
            var login = new { Username = "admin", Password = "admin123" };
            var content = new StringContent(
                JsonSerializer.Serialize(login),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/Auth/login", content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);

            return result.GetProperty("token").GetString() ?? "";
        }

        private void SetAuthToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task GetContracts_ReturnsOkStatus()
        {
            var token = await GetAuthToken();
            SetAuthToken(token);

            var response = await _client.GetAsync("/api/Contracts");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_ReturnsJsonData()
        {
            var token = await GetAuthToken();
            SetAuthToken(token);

            var response = await _client.GetAsync("/api/Contracts");
            var json = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(json));
            Assert.StartsWith("[", json.Trim());
        }

        [Fact]
        public async Task GetClients_ReturnsOkStatus()
        {
            var token = await GetAuthToken();
            SetAuthToken(token);

            var response = await _client.GetAsync("/api/Clients");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequests_ReturnsOkStatus()
        {
            var token = await GetAuthToken();
            SetAuthToken(token);

            var response = await _client.GetAsync("/api/ServiceRequests");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostInvalidServiceRequest_ReturnsBadRequest()
        {
            // First, login to get a token
            var token = await GetAuthToken();
            SetAuthToken(token);

            var invalidRequest = new { ContractId = 99999, Description = "Test", CostUSD = 100 };
            var content = new StringContent(
                JsonSerializer.Serialize(invalidRequest),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/ServiceRequests", content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            var login = new { Username = "admin", Password = "admin123" };
            var content = new StringContent(
                JsonSerializer.Serialize(login),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/Auth/login", content);
            var json = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("token", json);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var login = new { Username = "wrong", Password = "wrong" };
            var content = new StringContent(
                JsonSerializer.Serialize(login),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/Auth/login", content);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}