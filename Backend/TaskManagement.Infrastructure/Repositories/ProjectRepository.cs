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
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(TaskManagementDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Project>> GetProjectsByCompanyAsync(Guid companyId)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.Client)
                .Include(p => p.Tasks)
                .Where(p => p.CompanyId == companyId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync(Guid companyId)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.Client)
                .Where(p => p.CompanyId == companyId && p.Status == "Active")
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Project?> GetProjectWithDetailsAsync(Guid projectId, Guid companyId)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.Client)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedTo)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
        }

        public async Task<IEnumerable<Project>> GetProjectsByManagerAsync(Guid managerId, Guid companyId)
        {
            return await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.Tasks)
                .Where(p => p.ProjectManagerId == managerId && p.CompanyId == companyId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByClientAsync(Guid clientId, Guid companyId)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.Tasks)
                .Where(p => p.ClientId == clientId && p.CompanyId == companyId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetActiveProjectsCountAsync(Guid companyId)
        {
            return await _context.Projects
                .CountAsync(p => p.CompanyId == companyId && p.Status == "Active");
        }

        public async Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm, Guid companyId)
        {
            return await _context.Projects
                .Include(p => p.ProjectManager)
                .Include(p => p.Client)
                .Where(p => p.CompanyId == companyId && 
                    (p.Name.Contains(searchTerm) || 
                     p.Description.Contains(searchTerm) ||
                     p.ProjectCode.Contains(searchTerm)))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<string> GenerateProjectCodeAsync(Guid companyId)
        {
            var year = DateTime.UtcNow.Year;
            var latestProject = await _context.Projects
                .Where(p => p.CompanyId == companyId && p.ProjectCode.StartsWith($"PRJ-{year}-"))
                .OrderByDescending(p => p.ProjectCode)
                .FirstOrDefaultAsync();

            if (latestProject == null)
            {
                return $"PRJ-{year}-0001";
            }

            var lastNumber = int.Parse(latestProject.ProjectCode.Split('-').Last());
            return $"PRJ-{year}-{(lastNumber + 1).ToString("D4")}";
        }

        public async Task<Dictionary<string, int>> GetProjectStatisticsByStatusAsync(Guid companyId)
        {
            var projects = await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            return projects.ToDictionary(p => p.Status, p => p.Count);
        }
    }
}
