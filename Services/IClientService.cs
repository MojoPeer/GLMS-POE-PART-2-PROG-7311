using GLMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GLMS.Services
{
    public interface IClientService
    {
        Task<List<Client>> GetAllAsync();
        Task<Client> GetByIdAsync(int id);
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(int id);
    }
}
