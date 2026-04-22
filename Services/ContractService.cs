using GLMS.Data;
using GLMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GLMS.Services
{
    public class ContractService : IContractService
    {
        private readonly AppDbContext _context;
        public ContractService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Contract>> GetAllAsync()
        {
            return await _context.Contracts.Include(c => c.Client).Include(c => c.ServiceRequests).ToListAsync();
        }
        public async Task<Contract> GetByIdAsync(int id)
        {
            return await _context.Contracts.Include(c => c.Client).Include(c => c.ServiceRequests).FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task AddAsync(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Contract>> FilterAsync(ContractStatus? status, DateTime? start, DateTime? end)
        {
            var query = _context.Contracts.AsQueryable();
            if (status.HasValue)
                query = query.Where(c => c.Status == status);
            if (start.HasValue)
                query = query.Where(c => c.StartDate >= start);
            if (end.HasValue)
                query = query.Where(c => c.EndDate <= end);
            return await query.Include(c => c.Client).Include(c => c.ServiceRequests).ToListAsync();
        }
    }
}
