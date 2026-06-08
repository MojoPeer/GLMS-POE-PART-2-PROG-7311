// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.API.Data;
using GLMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS.API.Services
{
    public class ContractService : IContractService
    {
        private readonly ApplicationDbContext _db;
        public ContractService(ApplicationDbContext db) { _db = db; }

        public async Task<List<Contract>> GetAllAsync(string? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _db.Contracts.AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(c => c.Status == status);
            if (startDate.HasValue)
                query = query.Where(c => c.StartDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(c => c.EndDate <= endDate.Value);
            return await query.ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
            => await _db.Contracts.FindAsync(id);

        public async Task AddAsync(Contract contract)
        {
            var client = await _db.Clients.FindAsync(contract.ClientId);
            contract.ClientName = client?.Name ?? string.Empty;
            _db.Contracts.Add(contract);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contract contract)
        {
            var client = await _db.Clients.FindAsync(contract.ClientId);
            contract.ClientName = client?.Name ?? string.Empty;
            _db.Contracts.Update(contract);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Contracts.FindAsync(id);
            if (entity != null)
            {
                _db.Contracts.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateStatusAsync(int id, string newStatus)
        {
            var entity = await _db.Contracts.FindAsync(id);
            if (entity == null) return false;
            entity.Status = newStatus;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
