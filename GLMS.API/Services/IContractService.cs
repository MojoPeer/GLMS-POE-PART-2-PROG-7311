using GLMS.API.Models;

namespace GLMS.API.Services
{
    public interface IContractService
    {
        Task<List<Contract>> GetAllAsync(string? status = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<Contract?> GetByIdAsync(int id);
        Task AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string newStatus);
    }
}
