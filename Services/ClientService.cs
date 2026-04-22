using GLMS.Data;
using GLMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;
        public ClientService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Client>> GetAllAsync()
        {
            return await _context.Clients.Include(c => c.Contracts).ToListAsync();
        }
        public async Task<Client> GetByIdAsync(int id)
        {
            return await _context.Clients.Include(c => c.Contracts).FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task AddAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
        }
    }
}
