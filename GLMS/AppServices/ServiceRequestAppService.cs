// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.Data;
using GLMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.AppServices
{
    public class ServiceRequestAppService : IServiceRequestAppService
    {
        private readonly ApplicationDbContext _db;

        public ServiceRequestAppService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            return await _db.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            return await _db.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<bool> AddAsync(ServiceRequest request)
        {
            var contract = await _db.Contracts.FindAsync(request.ContractId);
            if (contract == null)
                return false;

            var status = contract.Status?.Trim().ToLowerInvariant();
            if (status == "draft" || status == "expired" || status == "on hold")
                return false;

            _db.ServiceRequests.Add(request);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(ServiceRequest request)
        {
            var contract = await _db.Contracts.FindAsync(request.ContractId);
            if (contract == null)
                return false;

            var status = contract.Status?.Trim().ToLowerInvariant();
            if (status == "draft" || status == "expired" || status == "on hold")
                return false;

            _db.ServiceRequests.Update(request);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.ServiceRequests.FindAsync(id);
            if (entity != null)
            {
                _db.ServiceRequests.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}