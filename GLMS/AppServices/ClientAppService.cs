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
    // Service responsible for client CRUD and data access.
    public class ClientAppService : IClientAppService
    {
        private readonly ApplicationDbContext _db;
        public ClientAppService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Client>> GetAllAsync()
        {
            return await _db.Clients.Include(c => c.Contracts).ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _db.Clients.Include(c => c.Contracts).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Client client)
        {
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            _db.Clients.Update(client);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Clients.FindAsync(id);
            if (entity != null)
            {
                _db.Clients.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
