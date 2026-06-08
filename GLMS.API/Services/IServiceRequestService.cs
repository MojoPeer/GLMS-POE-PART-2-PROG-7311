using GLMS.API.Models;

namespace GLMS.API.Services
{
    public interface IServiceRequestService
    {
        Task<List<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<bool> AddAsync(ServiceRequest request);
        Task<bool> UpdateAsync(ServiceRequest request);
        Task DeleteAsync(int id);
    }
}
