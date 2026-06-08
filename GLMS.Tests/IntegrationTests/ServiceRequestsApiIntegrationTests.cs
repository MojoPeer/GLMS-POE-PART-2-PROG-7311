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
    public class ServiceRequestsApiIntegrationTests : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ServiceRequestsApiIntegrationTests()
        {
            var dbName = "SRDb_" + Guid.NewGuid();

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

        // ── Seeded contract IDs (from DbInitializer) ──────────────────────────
        // Contract 1:  Active   (ClientId 1, Gold)
        // Contract 2:  Expired  (ClientId 2, Silver)
        // Contract 4:  On Hold  (ClientId 4, Bronze)
        // Contract 5:  Draft    (ClientId 5, Gold)
        // Service Requests 1-10 are seeded on contracts 1, 3, 6, 9

        [Fact]
        public async Task GetServiceRequests_Returns200()
        {
            Authorize(await GetTokenAsync());
            var response = await _client.GetAsync("/api/servicerequests");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequestById_NotFound_Returns404()
        {
            Authorize(await GetTokenAsync());
            var response = await _client.GetAsync("/api/servicerequests/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequestById_SeededRequest_Returns200()
        {
            Authorize(await GetTokenAsync());
            // Service request ID 1 is always seeded
            var response = await _client.GetAsync("/api/servicerequests/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_ActiveContract_Returns201()
        {
            Authorize(await GetTokenAsync());
            // Contract 1 is seeded with status "Active"
            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId = 1,
                description = "Test service request on Active contract",
                cost = 5000.00,
                status = "Pending"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_DraftContract_Returns422()
        {
            Authorize(await GetTokenAsync());
            // Contract 5 is seeded with status "Draft"
            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId = 5,
                description = "Blocked by Draft business rule",
                cost = 1000.00,
                status = "Pending"
            });

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_ExpiredContract_Returns422()
        {
            Authorize(await GetTokenAsync());
            // Contract 2 is seeded with status "Expired"
            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId = 2,
                description = "Blocked by Expired business rule",
                cost = 2000.00,
                status = "Pending"
            });

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_OnHoldContract_Returns422()
        {
            Authorize(await GetTokenAsync());
            // Contract 4 is seeded with status "On Hold"
            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId = 4,
                description = "Blocked by On Hold business rule",
                cost = 3000.00,
                status = "Pending"
            });

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task DeleteServiceRequest_Returns204()
        {
            Authorize(await GetTokenAsync());

            // Create a service request on seeded Active contract 1
            var createResp = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId = 1,
                description = "To be deleted",
                cost = 1500.00,
                status = "Pending"
            });
            Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);

            var created = await createResp.Content.ReadFromJsonAsync<JsonElement>();
            var id = created.GetProperty("id").GetInt32();

            var deleteResp = await _client.DeleteAsync($"/api/servicerequests/{id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
        }
    }
}
