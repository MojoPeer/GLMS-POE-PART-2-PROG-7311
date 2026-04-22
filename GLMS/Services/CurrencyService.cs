// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GLMS.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> ConvertUsdToZarAsync(decimal usdAmount)
        {
            // Example API: https://api.exchangerate-api.com/v4/latest/USD
            var response = await _httpClient.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var zarRate = root.GetProperty("rates").GetProperty("ZAR").GetDecimal();
            return usdAmount * zarRate;
        }
    }
}
