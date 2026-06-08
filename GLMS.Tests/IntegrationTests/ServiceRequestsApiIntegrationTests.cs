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
    /// Integration tests for the ServiceRequests API endpoint.
    /// Verifies that the business rule (Active contracts only) is enforced at the API level.
    /// </summary>
    public class ServiceRequestsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ServiceRequestsApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var testFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("IntegrationTestDb_SR"));
                });
            });

            _client = testFactory.CreateClient();
        }

        private async Task<string> GetJwtTokenAsync()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new { username = "admin", password = "admin123" });
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            return result.GetProperty("token").GetString() ?? string.Empty;
        }

        private void SetAuthHeader(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<(int clientId, int contractId)> CreateClientAndContractAsync(string status)
        {
            var token = await GetJwtTokenAsync();
            SetAuthHeader(token);

            var clientResp = await _client.PostAsJsonAsync("/api/clients", new
            {
                name = $"SR Test Client - {status}",
                contactDetails = "sr@test.com",
                region = "Asia"
            });
            var clientObj = await clientResp.Content.ReadFromJsonAsync<JsonElement>();
            var clientId = clientObj.GetProperty("id").GetInt32();

            var contractResp = await _client.PostAsJsonAsync("/api/contracts", new
            {
                clientId,
                startDate = DateTime.Today.ToString("yyyy-MM-dd"),
                endDate = DateTime.Today.AddMonths(12).ToString("yyyy-MM-dd"),
                status,
                serviceLevel = "Gold"
            });
            var contractObj = await contractResp.Content.ReadFromJsonAsync<JsonElement>();
            var contractId = contractObj.GetProperty("id").GetInt32();

            return (clientId, contractId);
        }

        [Fact]
        public async Task GetAllServiceRequests_ReturnsOk()
        {
            var token = await GetJwtTokenAsync();
            SetAuthHeader(token);

            var response = await _client.GetAsync("/api/servicerequests");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            Assert.StartsWith("[", json.TrimStart());
        }

        [Fact]
        public async Task CreateServiceRequest_OnActiveContract_ReturnsCreated()
        {
            var (_, contractId) = await CreateClientAndContractAsync("Active");

            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId,
                description = "Test service request on Active contract",
                cost = 5000.00,
                status = "Pending"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_OnDraftContract_ReturnsUnprocessableEntity()
        {
            var (_, contractId) = await CreateClientAndContractAsync("Draft");

            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId,
                description = "Should be blocked by business rule",
                cost = 1000.00,
                status = "Pending"
            });

            // Business rule: Draft contracts cannot have service requests
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_OnExpiredContract_ReturnsUnprocessableEntity()
        {
            var (_, contractId) = await CreateClientAndContractAsync("Expired");

            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId,
                description = "Should be blocked by business rule",
                cost = 2000.00,
                status = "Pending"
            });

            // Business rule: Expired contracts cannot have service requests
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_OnOnHoldContract_ReturnsUnprocessableEntity()
        {
            var (_, contractId) = await CreateClientAndContractAsync("On Hold");

            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId,
                description = "Should be blocked by business rule",
                cost = 3000.00,
                status = "Pending"
            });

            // Business rule: On Hold contracts cannot have service requests
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequestById_NotFound_Returns404()
        {
            var token = await GetJwtTokenAsync();
            SetAuthHeader(token);

            var response = await _client.GetAsync("/api/servicerequests/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteServiceRequest_Existing_ReturnsNoContent()
        {
            var (_, contractId) = await CreateClientAndContractAsync("Active");

            // Create first
            var createResp = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId,
                description = "To be deleted",
                cost = 1500.00,
                status = "Pending"
            });
            var created = await createResp.Content.ReadFromJsonAsync<JsonElement>();
            var id = created.GetProperty("id").GetInt32();

            // Delete
            var deleteResp = await _client.DeleteAsync($"/api/servicerequests/{id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
        }
    }
}
