// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.AppServices
{
    public interface IServiceRequestAppService
    {
        Task<List<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);

        // Returns true if created successfully, false when business rule blocks creation
        Task<bool> AddAsync(ServiceRequest request);

        // Returns true if updated successfully, false when business rule blocks update
        Task<bool> UpdateAsync(ServiceRequest request);

        Task DeleteAsync(int id);
    }
}