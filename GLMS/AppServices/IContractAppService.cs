// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.AppServices
{
    public interface IContractAppService
    {
        Task<List<Contract>> GetAllAsync();
        Task<Contract?> GetByIdAsync(int id);
        Task AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);
        Task<List<Contract>> FilterAsync(string? status, DateTime? start, DateTime? end);
    }
}
