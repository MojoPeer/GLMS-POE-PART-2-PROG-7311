using GLMS.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.Services
{
    public interface IContractService
    {
        Task<List<Contract>> GetAllAsync();
        Task<Contract> GetByIdAsync(int id);
        Task AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);
        Task<List<Contract>> FilterAsync(ContractStatus? status, DateTime? start, DateTime? end);
    }
}
