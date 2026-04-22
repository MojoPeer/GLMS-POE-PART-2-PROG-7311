using GLMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.Services
{
    public interface IServiceRequestService
    {
        Task<List<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest> GetByIdAsync(int id);
        Task<bool> AddAsync(ServiceRequest request);
        Task UpdateAsync(ServiceRequest request);
        Task DeleteAsync(int id);
    }
}
