using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task<IEnumerable<Client>> GetClientsByCompanyAsync(Guid companyId);
        Task<Client?> GetClientWithProjectsAsync(Guid clientId, Guid companyId);
        Task<bool> IsEmailUniqueAsync(string email, Guid companyId, Guid? excludeClientId = null);
    }
}