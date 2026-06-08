// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.AppServices;
using GLMS.Models;
using System.Net.Http.Json;

namespace GLMS.HttpServices
{
    /// <summary>
    /// HTTP-based implementation of IClientAppService.
    /// Calls the GLMS Web API instead of directly accessing the database.
    /// </summary>
    public class HttpClientAppService : IClientAppService
    {
        private readonly HttpClient _http;

        public HttpClientAppService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Client>> GetAllAsync()
            => await _http.GetFromJsonAsync<List<Client>>("api/clients") ?? new();

        public async Task<Client?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/clients/{id}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<Client>();
        }

        public async Task AddAsync(Client client)
        {
            var response = await _http.PostAsJsonAsync("api/clients", client);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(Client client)
        {
            var response = await _http.PutAsJsonAsync($"api/clients/{client.Id}", client);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/clients/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
