// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using GLMS.API.Data;

namespace GLMS.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests that spin up the GLMS API in-process using WebApplicationFactory.
    /// These tests call live API endpoints and assert the HTTP responses.
    /// This is critical in a CI/CD pipeline to prevent breaking changes from reaching production.
    /// </summary>
    public class ContractsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public ContractsApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Override the database with an in-memory database for testing
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real SQL Server registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Use in-memory database for fast, isolated tests
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("IntegrationTestDb_Contracts"));
                });
            });

            _client = _factory.CreateClient();
        }

        private async Task<string> GetJwtTokenAsync()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new
            {
                username = "admin",
                password = "admin123"
            });
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            return result.GetProperty("token").GetString() ?? string.Empty;
        }

        private void SetAuthHeader(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new
            {
                username = "admin",
                password = "admin123"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", json);
            Assert.NotEmpty(json);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new
            {
                username = "wrong",
                password = "wrongpassword"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllContracts_WithoutToken_ReturnsUnauthorized()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var response = await _client.GetAsync("/api/contracts");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllContracts_WithValidToken_ReturnsOkWithJsonArray()
        {
            var token = await GetJwtTokenAsync();
            SetAuthHeader(token);

            var response = await _client.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.NotNull(json);
            // Response should be a JSON array
            Assert.StartsWith("[", json.TrimStart());
        }

        [Fact]
        public async Task CreateContract_WithValidData_ReturnsCreated()
        {
            var token = await GetJwtTokenAsync();
            SetAuthHeader(token);

            // First create a client to satisfy the FK
            var clientResponse = await _client.PostAsJsonAsync("/api/clients", new
            {
                name = "Test Client",
                contactDetails = "test@test.com",
                region = "Africa"
            });
            Assert.Equal(HttpStatusCode.Created, clientResponse.StatusCode);
            var client = await clientResponse.Content.ReadFromJsonAsync<JsonElement>();
            var clientId = client.GetProperty("id").GetInt32();

            var contractResponse = await _client.PostAsJsonAsync("/api/contracts", new
            {
                clientId = clientId,
                startDate = DateTime.Today.ToString("yyyy-MM-dd"),
                endDate = DateTime.Today.AddMonths(12).ToString("yyyy-MM-dd"),
                status = "Active",
                serviceLevel = "Gold"
            });

            Assert.Equal(HttpStatusCode.Created, contractResponse.StatusCode);
        }

        [Fact]
        public async Task GetContractById_WithNonExistentId_ReturnsNotFound()
        {
            var token = await GetJwtTokenAsync();
            SetAuthHeader(token);

            var response = await _client.GetAsync("/api/contracts/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task FilterContracts_ByStatus_ReturnsFilteredList()
        {
            var token = await GetJwtTokenAsync();
            SetAuthHeader(token);

            var response = await _client.GetAsync("/api/contracts?status=Active");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contracts = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
            Assert.NotNull(contracts);
            // All returned contracts should have status Active
            foreach (var contract in contracts)
            {
                var status = contract.GetProperty("status").GetString();
                Assert.Equal("Active", status);
            }
        }

        [Fact]
        public async Task UpdateContractStatus_WithPatch_ReturnsNoContent()
        {
            var token = await GetJwtTokenAsync();
            SetAuthHeader(token);

            // Create client and contract first
            var clientResp = await _client.PostAsJsonAsync("/api/clients", new
            {
                name = "Patch Test Client",
                contactDetails = "patch@test.com",
                region = "Europe"
            });
            var clientObj = await clientResp.Content.ReadFromJsonAsync<JsonElement>();
            var clientId = clientObj.GetProperty("id").GetInt32();

            var contractResp = await _client.PostAsJsonAsync("/api/contracts", new
            {
                clientId,
                startDate = DateTime.Today.ToString("yyyy-MM-dd"),
                endDate = DateTime.Today.AddMonths(6).ToString("yyyy-MM-dd"),
                status = "Draft",
                serviceLevel = "Silver"
            });
            var contract = await contractResp.Content.ReadFromJsonAsync<JsonElement>();
            var contractId = contract.GetProperty("id").GetInt32();

            // Patch the status
            var patchResponse = await _client.PatchAsJsonAsync($"/api/contracts/{contractId}/status",
                new { status = "Active" });

            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);
        }
    }
}
