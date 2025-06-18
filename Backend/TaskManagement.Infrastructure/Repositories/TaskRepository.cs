using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : GenericRepository<Core.Entities.Task>, ITaskRepository
    {
        public TaskRepository(TaskManagementDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Core.Entities.Task>> GetTasksByCompanyAsync(Guid companyId)
        {
            return await _context.Tasks
                .Where(t => t.CompanyId == companyId)
                .Include(t => t.AssignedTo)
                .Include(t => t.AssignedBy)
                .Include(t => t.Client)
                .Include(t => t.Project)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Core.Entities.Task>> GetTasksByUserAsync(Guid userId, Guid companyId)
        {
            return await _context.Tasks
                .Where(t => t.CompanyId == companyId && t.AssignedToId == userId)
                .Include(t => t.AssignedBy)
                .Include(t => t.Client)
                .Include(t => t.Project)
                .OrderByDescending(t => t.DueDate ?? t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Core.Entities.Task>> GetTasksByProjectAsync(Guid projectId, Guid companyId)
        {
            return await _context.Tasks
                .Where(t => t.CompanyId == companyId && t.ProjectId == projectId)
                .Include(t => t.AssignedTo)
                .Include(t => t.AssignedBy)
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Core.Entities.Task>> GetTasksByClientAsync(Guid clientId, Guid companyId)
        {
            return await _context.Tasks
                .Where(t => t.CompanyId == companyId && t.ClientId == clientId)
                .Include(t => t.AssignedTo)
                .Include(t => t.AssignedBy)
                .Include(t => t.Project)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Core.Entities.Task>> GetOverdueTasksAsync(Guid companyId)
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Tasks
                .Where(t => t.CompanyId == companyId && 
                           t.DueDate != null && 
                           t.DueDate.Value.Date < today &&
                           t.Status != "Completed" && 
                           t.Status != "Cancelled")
                .Include(t => t.AssignedTo)
                .Include(t => t.AssignedBy)
                .Include(t => t.Client)
                .Include(t => t.Project)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Core.Entities.Task>> GetTasksForCalendarAsync(Guid companyId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await _context.Tasks
                .Where(t => t.CompanyId == companyId &&
                           ((t.StartDate != null && t.StartDate >= startDate && t.StartDate <= endDate) ||
                            (t.DueDate != null && t.DueDate >= startDate && t.DueDate <= endDate)))
                .Include(t => t.AssignedTo)
                .Include(t => t.Project)
                .OrderBy(t => t.StartDate ?? t.DueDate)
                .ToListAsync();
        }

        public async Task<string> GenerateTaskNumberAsync(Guid companyId)
        {
            var year = DateTime.UtcNow.Year;
            var lastTask = await _context.Tasks
                .Where(t => t.CompanyId == companyId && t.TaskNumber.StartsWith($"TSK-{year}-"))
                .OrderByDescending(t => t.TaskNumber)
                .FirstOrDefaultAsync();

            if (lastTask == null)
            {
                return $"TSK-{year}-0001";
            }

            var lastNumber = int.Parse(lastTask.TaskNumber.Split('-').Last());
            return $"TSK-{year}-{(lastNumber + 1).ToString().PadLeft(4, '0')}";
        }

        public async Task<Dictionary<string, int>> GetTaskStatisticsByStatusAsync(Guid companyId)
        {
            return await _context.Tasks
                .Where(t => t.CompanyId == companyId)
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetTaskStatisticsByPriorityAsync(Guid companyId)
        {
            return await _context.Tasks
                .Where(t => t.CompanyId == companyId)
                .GroupBy(t => t.Priority)
                .Select(g => new { Priority = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Priority, x => x.Count);
        }
    }
}