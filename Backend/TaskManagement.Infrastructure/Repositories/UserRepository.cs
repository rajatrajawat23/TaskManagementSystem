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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(TaskManagementDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Permissions)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserWithDetailsAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Permissions)
                .Include(u => u.AssignedTasks)
                .Include(u => u.ManagedProjects)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> GetUsersByCompanyAsync(Guid companyId)
        {
            return await _context.Users
                .Include(u => u.Permissions)
                .Where(u => u.CompanyId == companyId)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersByCompanyAsync(Guid companyId)
        {
            return await _context.Users
                .Where(u => u.CompanyId == companyId && u.IsActive)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role, Guid companyId)
        {
            return await _context.Users
                .Where(u => u.Role == role && u.CompanyId == companyId)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Email == email);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);
        }

        public async Task<User?> GetUserByPasswordResetTokenAsync(string resetToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == resetToken && u.PasswordResetExpiry > DateTime.UtcNow);
        }

        public async Task<int> GetActiveUsersCountAsync(Guid companyId)
        {
            return await _context.Users
                .CountAsync(u => u.CompanyId == companyId && u.IsActive);
        }

        public async Task<User?> GetUserWithPermissionsAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Company)
                .Include(u => u.Permissions)
                    .ThenInclude(p => p.GrantedBy)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, Guid? companyId = null)
        {
            var query = _context.Users.AsQueryable();

            if (companyId.HasValue)
            {
                query = query.Where(u => u.CompanyId == companyId.Value);
            }

            return await query
                .Where(u => u.FirstName.Contains(searchTerm) || 
                           u.LastName.Contains(searchTerm) ||
                           u.Email.Contains(searchTerm))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }
    }
}
