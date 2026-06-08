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
    public class ContractsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ContractsApiIntegrationTests(WebApplicationFactory<Program> factory)
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
                        options.UseInMemoryDatabase("ContractsTestDb_" + Guid.NewGuid()));
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
                new { username = "wrong", password = "wrong" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_NoToken_Returns401()
        {
            _client.DefaultRequestHeaders.Authorization = null;
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
        public async Task CreateContract_ValidData_Returns201()
        {
            Authorize(await GetTokenAsync());

            var clientResp = await _client.PostAsJsonAsync("/api/clients",
                new { name = "Test Client", contactDetails = "t@t.com", region = "Africa" });
            Assert.Equal(HttpStatusCode.Created, clientResp.StatusCode);
            var clientObj = await clientResp.Content.ReadFromJsonAsync<JsonElement>();
            var clientId = clientObj.GetProperty("id").GetInt32();

            var contractResp = await _client.PostAsJsonAsync("/api/contracts", new
            {
                clientId,
                startDate = DateTime.Today.ToString("yyyy-MM-dd"),
                endDate = DateTime.Today.AddMonths(12).ToString("yyyy-MM-dd"),
                status = "Active",
                serviceLevel = "Gold"
            });

            Assert.Equal(HttpStatusCode.Created, contractResp.StatusCode);
        }

        [Fact]
        public async Task FilterContracts_ByStatus_ReturnsMatchingOnly()
        {
            Authorize(await GetTokenAsync());

            // Create a client and Active contract
            var cr = await _client.PostAsJsonAsync("/api/clients",
                new { name = "Filter Client", contactDetails = "f@f.com", region = "Asia" });
            var cObj = await cr.Content.ReadFromJsonAsync<JsonElement>();
            var cId = cObj.GetProperty("id").GetInt32();

            await _client.PostAsJsonAsync("/api/contracts", new
            {
                clientId = cId,
                startDate = DateTime.Today.ToString("yyyy-MM-dd"),
                endDate = DateTime.Today.AddMonths(6).ToString("yyyy-MM-dd"),
                status = "Active",
                serviceLevel = "Silver"
            });

            var response = await _client.GetAsync("/api/contracts?status=Active");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var contracts = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
            Assert.NotNull(contracts);
            Assert.All(contracts, c => Assert.Equal("Active", c.GetProperty("status").GetString()));
        }

        [Fact]
        public async Task PatchContractStatus_Returns204()
        {
            Authorize(await GetTokenAsync());

            var cr = await _client.PostAsJsonAsync("/api/clients",
                new { name = "Patch Client", contactDetails = "p@p.com", region = "Europe" });
            var cObj = await cr.Content.ReadFromJsonAsync<JsonElement>();
            var cId = cObj.GetProperty("id").GetInt32();

            var conResp = await _client.PostAsJsonAsync("/api/contracts", new
            {
                clientId = cId,
                startDate = DateTime.Today.ToString("yyyy-MM-dd"),
                endDate = DateTime.Today.AddMonths(6).ToString("yyyy-MM-dd"),
                status = "Draft",
                serviceLevel = "Bronze"
            });
            var conObj = await conResp.Content.ReadFromJsonAsync<JsonElement>();
            var conId = conObj.GetProperty("id").GetInt32();

            var patch = await _client.PatchAsJsonAsync($"/api/contracts/{conId}/status",
                new { status = "Active" });
            Assert.Equal(HttpStatusCode.NoContent, patch.StatusCode);
        }
    }
}
