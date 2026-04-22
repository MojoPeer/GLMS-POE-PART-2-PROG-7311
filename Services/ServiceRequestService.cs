using GLMS.Data;
using GLMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GLMS.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly AppDbContext _context;
        public ServiceRequestService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests.Include(sr => sr.Contract).ToListAsync();
        }
        public async Task<ServiceRequest> GetByIdAsync(int id)
        {
            return await _context.ServiceRequests.Include(sr => sr.Contract).FirstOrDefaultAsync(sr => sr.Id == id);
        }
        public async Task<bool> AddAsync(ServiceRequest request)
        {
            var contract = await _context.Contracts.FindAsync(request.ContractId);
            if (contract == null || contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
                return false;
            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task UpdateAsync(ServiceRequest request)
        {
            _context.ServiceRequests.Update(request);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var req = await _context.ServiceRequests.FindAsync(id);
            if (req != null)
            {
                _context.ServiceRequests.Remove(req);
                await _context.SaveChangesAsync();
            }
        }
    }
}
