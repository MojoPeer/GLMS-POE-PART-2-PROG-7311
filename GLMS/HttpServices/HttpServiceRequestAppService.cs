// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.AppServices;
using GLMS.Models;
using System.Net.Http.Json;

namespace GLMS.HttpServices
{
    /// <summary>
    /// HTTP-based implementation of IServiceRequestAppService.
    /// Calls the GLMS Web API instead of directly accessing the database.
    /// Returns false when the API returns 422 (business rule blocked).
    /// </summary>
    public class HttpServiceRequestAppService : IServiceRequestAppService
    {
        private readonly HttpClient _http;

        public HttpServiceRequestAppService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ServiceRequest>> GetAllAsync()
            => await _http.GetFromJsonAsync<List<ServiceRequest>>("api/servicerequests") ?? new();

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/servicerequests/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ServiceRequest>();
        }

        public async Task<bool> AddAsync(ServiceRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/servicerequests", request);
            // 422 = business rule blocked (Draft/Expired/On Hold contract)
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(ServiceRequest request)
        {
            var response = await _http.PutAsJsonAsync($"api/servicerequests/{request.Id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/servicerequests/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
