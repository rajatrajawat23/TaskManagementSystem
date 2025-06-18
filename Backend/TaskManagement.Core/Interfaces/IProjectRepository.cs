using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<IEnumerable<Project>> GetProjectsByCompanyAsync(Guid companyId);
        Task<IEnumerable<Project>> GetProjectsByClientAsync(Guid clientId, Guid companyId);
        Task<IEnumerable<Project>> GetProjectsByManagerAsync(Guid managerId, Guid companyId);
        Task<Project?> GetProjectWithDetailsAsync(Guid projectId, Guid companyId);
        Task<string> GenerateProjectCodeAsync(Guid companyId);
        Task<Dictionary<string, int>> GetProjectStatisticsByStatusAsync(Guid companyId);
    }
}