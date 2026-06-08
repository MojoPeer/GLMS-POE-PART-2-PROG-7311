// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.API.Data;
using GLMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS.API.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _db;
        public ClientService(ApplicationDbContext db) { _db = db; }

        public async Task<List<Client>> GetAllAsync()
            => await _db.Clients.ToListAsync();

        public async Task<Client?> GetByIdAsync(int id)
            => await _db.Clients.FindAsync(id);

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
