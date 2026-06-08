// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using GLMS.API.Data;

namespace GLMS.Tests.IntegrationTests
{
    // Fresh factory + isolated InMemory DB per test (no shared state issues)
    public class ContractsApiIntegrationTests : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ContractsApiIntegrationTests()
        {
            var dbName = "ContractsDb_" + Guid.NewGuid();

            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Jwt:Key"]      = "GLMS_JWT_SuperSecretKey_2026_PROG7311_POE_Part3",
                        ["Jwt:Issuer"]   = "GLMS.API",
                        ["Jwt:Audience"] = "GLMS.MVC"
                    });
                });

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(dbName));
                });
            });

            _client = _factory.CreateClient();
        }

        public void Dispose() => _factory.Dispose();

        private async Task<string> GetTokenAsync()
        {
            var resp = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "admin", password = "admin123" });
            var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("token").GetString() ?? "";
        }

        private void Authorize(string token) =>
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // ── Auth tests ────────────────────────────────────────────────────────

        [Fact]
        public async Task Login_ValidCredentials_Returns200WithToken()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "admin", password = "admin123" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", body);
        }

        [Fact]
        public async Task Login_InvalidCredentials_Returns401()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "wrong", password = "bad" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ── Contract tests (use seeded data — IDs 1-10 always present) ────────

        [Fact]
        public async Task GetContracts_NoToken_Returns401()
        {
            // No Authorization header — must be unauthenticated
            var response = await _client.GetAsync("/api/contracts");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_WithToken_Returns200()
        {
            Authorize(await GetTokenAsync());
            var response = await _client.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.StartsWith("[", body.TrimStart());
        }

        [Fact]
        public async Task GetContractById_NotFound_Returns404()
        {
            Authorize(await GetTokenAsync());
            var response = await _client.GetAsync("/api/contracts/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetContractById_SeededContract_Returns200()
        {
            Authorize(await GetTokenAsync());
            // Contract ID 1 is always seeded (Active, Gold)
            var response = await _client.GetAsync("/api/contracts/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task FilterContracts_ByStatus_ReturnsMatchingOnly()
        {
            Authorize(await GetTokenAsync());
            var response = await _client.GetAsync("/api/contracts?status=Active");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var contracts = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
            Assert.NotNull(contracts);
            Assert.NotEmpty(contracts);
            Assert.All(contracts, c =>
                Assert.Equal("Active", c.GetProperty("status").GetString()));
        }

        [Fact]
        public async Task FilterContracts_ByExpiredStatus_ReturnsMatchingOnly()
        {
            Authorize(await GetTokenAsync());
            var response = await _client.GetAsync("/api/contracts?status=Expired");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var contracts = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
            Assert.NotNull(contracts);
            Assert.All(contracts, c =>
                Assert.Equal("Expired", c.GetProperty("status").GetString()));
        }

        [Fact]
        public async Task PatchContractStatus_SeededContract_Returns204()
        {
            Authorize(await GetTokenAsync());
            // Contract 5 is seeded as "Draft" — patch it to "Active"
            var patch = await _client.PatchAsJsonAsync("/api/contracts/5/status",
                new { status = "Active" });

            Assert.Equal(HttpStatusCode.NoContent, patch.StatusCode);
        }

        [Fact]
        public async Task PatchContractStatus_NonExistent_Returns404()
        {
            Authorize(await GetTokenAsync());
            var patch = await _client.PatchAsJsonAsync("/api/contracts/99999/status",
                new { status = "Active" });

            Assert.Equal(HttpStatusCode.NotFound, patch.StatusCode);
        }
    }
}
