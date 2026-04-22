// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using System.Threading.Tasks;

namespace GLMS.Services
{
    public interface ICurrencyService
    {
        Task<decimal> ConvertUsdToZarAsync(decimal usdAmount);
    }
}
