using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(TaskManagementDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Client>> GetClientsByCompanyAsync(Guid companyId)
        {
            return await _context.Clients
                .Include(c => c.Projects)
                .Include(c => c.Tasks)
                .Where(c => c.CompanyId == companyId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Client?> GetClientWithProjectsAsync(Guid clientId, Guid companyId)
        {
            return await _context.Clients
                .Include(c => c.Projects)
                    .ThenInclude(p => p.ProjectManager)
                .Include(c => c.Tasks)
                    .ThenInclude(t => t.AssignedTo)
                .FirstOrDefaultAsync(c => c.Id == clientId && c.CompanyId == companyId);
        }

        public async Task<IEnumerable<Client>> GetActiveClientsAsync(Guid companyId)
        {
            return await _context.Clients
                .Include(c => c.Projects)
                .Where(c => c.CompanyId == companyId && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Client?> GetClientByEmailAsync(string email, Guid companyId)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == email && c.CompanyId == companyId);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid companyId, Guid? excludeClientId = null)
        {
            var query = _context.Clients
                .Where(c => c.Email == email && c.CompanyId == companyId);

            if (excludeClientId.HasValue)
            {
                query = query.Where(c => c.Id != excludeClientId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Client>> SearchClientsAsync(string searchTerm, Guid companyId)
        {
            return await _context.Clients
                .Where(c => c.CompanyId == companyId && 
                    (c.Name.Contains(searchTerm) || 
                     c.Email.Contains(searchTerm) ||
                     c.Phone.Contains(searchTerm) ||
                     c.ContactPerson.Contains(searchTerm)))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<int> GetActiveClientsCountAsync(Guid companyId)
        {
            return await _context.Clients
                .CountAsync(c => c.CompanyId == companyId && c.IsActive);
        }
    }
}
