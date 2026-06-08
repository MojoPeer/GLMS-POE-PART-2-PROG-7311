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
    public class ServiceRequestsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ServiceRequestsApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var testFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Jwt:Key"]      = "GLMS_JWT_SuperSecretKey_2026_PROG7311_POE_Part3",
                        ["Jwt:Issuer"]   = "GLMS.API",
                        ["Jwt:Audience"] = "GLMS.MVC",
                        ["ConnectionStrings:DefaultConnection"] = "DataSource=:memory:"
                    });
                });

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("SRTestDb_" + Guid.NewGuid()));
                });
            });

            _client = testFactory.CreateClient();
        }

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

        private async Task<int> CreateContractWithStatus(string status)
        {
            var cr = await _client.PostAsJsonAsync("/api/clients",
                new { name = $"Client-{status}-{Guid.NewGuid()}", contactDetails = "x@x.com", region = "Asia" });
            var cObj = await cr.Content.ReadFromJsonAsync<JsonElement>();
            var cId = cObj.GetProperty("id").GetInt32();

            var conResp = await _client.PostAsJsonAsync("/api/contracts", new
            {
                clientId = cId,
                startDate = DateTime.Today.ToString("yyyy-MM-dd"),
                endDate = DateTime.Today.AddMonths(12).ToString("yyyy-MM-dd"),
                status,
                serviceLevel = "Gold"
            });
            var conObj = await conResp.Content.ReadFromJsonAsync<JsonElement>();
            return conObj.GetProperty("id").GetInt32();
        }

        [Fact]
        public async Task GetServiceRequests_Returns200()
        {
            Authorize(await GetTokenAsync());
            var response = await _client.GetAsync("/api/servicerequests");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_ActiveContract_Returns201()
        {
            Authorize(await GetTokenAsync());
            var contractId = await CreateContractWithStatus("Active");

            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId,
                description = "Test request on Active contract",
                cost = 5000.00,
                status = "Pending"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_DraftContract_Returns422()
        {
            Authorize(await GetTokenAsync());
            var contractId = await CreateContractWithStatus("Draft");

            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId,
                description = "Blocked by business rule",
                cost = 1000.00,
                status = "Pending"
            });

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_ExpiredContract_Returns422()
        {
            Authorize(await GetTokenAsync());
            var contractId = await CreateContractWithStatus("Expired");

            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId,
                description = "Blocked by business rule",
                cost = 2000.00,
                status = "Pending"
            });

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task CreateServiceRequest_OnHoldContract_Returns422()
        {
            Authorize(await GetTokenAsync());
            var contractId = await CreateContractWithStatus("On Hold");

            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId,
                description = "Blocked by business rule",
                cost = 3000.00,
                status = "Pending"
            });

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequestById_NotFound_Returns404()
        {
            Authorize(await GetTokenAsync());
            var response = await _client.GetAsync("/api/servicerequests/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteServiceRequest_Returns204()
        {
            Authorize(await GetTokenAsync());
            var contractId = await CreateContractWithStatus("Active");

            var createResp = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                contractId,
                description = "To delete",
                cost = 1500.00,
                status = "Pending"
            });
            var created = await createResp.Content.ReadFromJsonAsync<JsonElement>();
            var id = created.GetProperty("id").GetInt32();

            var deleteResp = await _client.DeleteAsync($"/api/servicerequests/{id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
        }
    }
}
