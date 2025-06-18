using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByCompanyAsync(Guid companyId);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role, Guid companyId);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null);
        Task<User?> GetUserWithPermissionsAsync(Guid userId);
    }
}