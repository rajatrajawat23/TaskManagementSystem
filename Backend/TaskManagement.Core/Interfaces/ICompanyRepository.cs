using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company?> GetByDomainAsync(string domain);
        Task<bool> IsDomainUniqueAsync(string domain, Guid? excludeCompanyId = null);
        Task<Dictionary<string, int>> GetCompanyStatisticsAsync(Guid companyId);
    }
}