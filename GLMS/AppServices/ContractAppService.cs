// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.Data;
using GLMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GLMS.AppServices
{
    // Service responsible for contract CRUD and filtering logic.
    public class ContractAppService : IContractAppService
    {
        private readonly ApplicationDbContext _db;
        public ContractAppService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Contract>> GetAllAsync()
        {
            return await _db.Contracts.Include(c => c.Client).Include(c => c.ServiceRequests).ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _db.Contracts.Include(c => c.Client).Include(c => c.ServiceRequests).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Contract contract)
        {
            _db.Contracts.Add(contract);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Contract contract)
        {
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

        public async Task<List<Contract>> FilterAsync(string? status, DateTime? start, DateTime? end)
        {
            var query = _db.Contracts.AsQueryable();
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status == status);
            }
            if (start.HasValue)
            {
                query = query.Where(c => c.StartDate >= start.Value);
            }
            if (end.HasValue)
            {
                query = query.Where(c => c.EndDate <= end.Value);
            }
            return await query.Include(c => c.Client).Include(c => c.ServiceRequests).ToListAsync();
        }
    }
}
