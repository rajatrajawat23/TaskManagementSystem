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
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(TaskManagementDbContext context) : base(context)
        {
        }

        public async Task<Company?> GetByDomainAsync(string domain)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Domain == domain);
        }

        public async Task<Company?> GetCompanyWithDetailsAsync(Guid companyId)
        {
            return await _context.Companies
                .Include(c => c.Users)
                .Include(c => c.Projects)
                .Include(c => c.Clients)
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == companyId);
        }

        public async Task<IEnumerable<Company>> GetActiveCompaniesAsync()
        {
            return await _context.Companies
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Company>> GetCompaniesBySubscriptionTypeAsync(string subscriptionType)
        {
            return await _context.Companies
                .Where(c => c.SubscriptionType == subscriptionType && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Company>> GetExpiringSubscriptionsAsync(int daysBeforeExpiry)
        {
            var expiryDate = DateTime.UtcNow.AddDays(daysBeforeExpiry);
            
            return await _context.Companies
                .Where(c => c.SubscriptionExpiryDate.HasValue && 
                           c.SubscriptionExpiryDate.Value <= expiryDate &&
                           c.IsActive)
                .OrderBy(c => c.SubscriptionExpiryDate)
                .ToListAsync();
        }

        public async Task<bool> IsDomainUniqueAsync(string domain, Guid? excludeCompanyId = null)
        {
            var query = _context.Companies.Where(c => c.Domain == domain);

            if (excludeCompanyId.HasValue)
            {
                query = query.Where(c => c.Id != excludeCompanyId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<int> GetActiveUsersCountAsync(Guid companyId)
        {
            return await _context.Users
                .CountAsync(u => u.CompanyId == companyId && u.IsActive);
        }

        public async Task<bool> CanAddMoreUsersAsync(Guid companyId)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
                return false;

            var currentUserCount = await GetActiveUsersCountAsync(companyId);
            return currentUserCount < company.MaxUsers;
        }

        public async Task<Dictionary<string, int>> GetCompanyStatisticsAsync(Guid companyId)
        {
            var company = await _context.Companies
                .Include(c => c.Users)
                .Include(c => c.Projects)
                .Include(c => c.Clients)
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
                return new Dictionary<string, int>();

            return new Dictionary<string, int>
            {
                ["TotalUsers"] = company.Users.Count(u => u.IsActive),
                ["TotalProjects"] = company.Projects.Count(p => !p.IsDeleted),
                ["ActiveProjects"] = company.Projects.Count(p => !p.IsDeleted && p.Status == "Active"),
                ["TotalClients"] = company.Clients.Count(c => !c.IsDeleted && c.IsActive),
                ["TotalTasks"] = company.Tasks.Count(t => !t.IsDeleted),
                ["CompletedTasks"] = company.Tasks.Count(t => !t.IsDeleted && t.Status == "Completed"),
                ["OverdueTasks"] = company.Tasks.Count(t => !t.IsDeleted && t.DueDate < DateTime.UtcNow && t.Status != "Completed")
            };
        }
    }

    public class CompanyStatistics
    {
        public int TotalUsers { get; set; }
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int TotalClients { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
    }
}
