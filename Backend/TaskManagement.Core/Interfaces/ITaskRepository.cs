using System.Threading.Tasks;

namespace TaskManagement.Core.Interfaces
{
    public interface ITaskRepository : IGenericRepository<Core.Entities.Task>
    {
        Task<IEnumerable<Core.Entities.Task>> GetTasksByCompanyAsync(Guid companyId);
        Task<IEnumerable<Core.Entities.Task>> GetTasksByUserAsync(Guid userId, Guid companyId);
        Task<IEnumerable<Core.Entities.Task>> GetTasksByProjectAsync(Guid projectId, Guid companyId);
        Task<IEnumerable<Core.Entities.Task>> GetTasksByClientAsync(Guid clientId, Guid companyId);
        Task<IEnumerable<Core.Entities.Task>> GetOverdueTasksAsync(Guid companyId);
        Task<IEnumerable<Core.Entities.Task>> GetTasksForCalendarAsync(Guid companyId, int year, int month);
        Task<string> GenerateTaskNumberAsync(Guid companyId);
        Task<Dictionary<string, int>> GetTaskStatisticsByStatusAsync(Guid companyId);
        Task<Dictionary<string, int>> GetTaskStatisticsByPriorityAsync(Guid companyId);
    }
}