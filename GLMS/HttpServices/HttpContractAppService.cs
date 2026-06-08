// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.AppServices;
using GLMS.Models;
using System.Net.Http.Json;

namespace GLMS.HttpServices
{
    /// <summary>
    /// HTTP-based implementation of IContractAppService.
    /// Calls the GLMS Web API instead of directly accessing the database.
    /// </summary>
    public class HttpContractAppService : IContractAppService
    {
        private readonly HttpClient _http;

        public HttpContractAppService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Contract>> GetAllAsync()
            => await _http.GetFromJsonAsync<List<Contract>>("api/contracts") ?? new();

        public async Task<Contract?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/contracts/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<Contract>();
        }

        public async Task AddAsync(Contract contract)
        {
            var response = await _http.PostAsJsonAsync("api/contracts", contract);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(Contract contract)
        {
            var response = await _http.PutAsJsonAsync($"api/contracts/{contract.Id}", contract);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/contracts/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Contract>> FilterAsync(string? status, DateTime? start, DateTime? end)
        {
            var query = "api/contracts?";
            if (!string.IsNullOrEmpty(status))
                query += $"status={Uri.EscapeDataString(status)}&";
            if (start.HasValue)
                query += $"startDate={start.Value:yyyy-MM-dd}&";
            if (end.HasValue)
                query += $"endDate={end.Value:yyyy-MM-dd}&";

            return await _http.GetFromJsonAsync<List<Contract>>(query.TrimEnd('&', '?')) ?? new();
        }
    }
}
