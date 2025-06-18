using System;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Specific repositories with custom methods
        ITaskRepository Tasks { get; }
        IProjectRepository Projects { get; }
        IClientRepository Clients { get; }
        IUserRepository Users { get; }
        ICompanyRepository Companies { get; }
        
        // Generic repositories for other entities
        IGenericRepository<SubTask> SubTasks { get; }
        IGenericRepository<ChatGroup> ChatGroups { get; }
        IGenericRepository<ChatMessage> ChatMessages { get; }
        IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<TaskAttachment> TaskAttachments { get; }
        IGenericRepository<TaskComment> TaskComments { get; }
        IGenericRepository<UserPermission> UserPermissions { get; }

        // Transaction support
        System.Threading.Tasks.Task<int> SaveChangesAsync();
        int SaveChanges();
        System.Threading.Tasks.Task BeginTransactionAsync();
        System.Threading.Tasks.Task CommitTransactionAsync();
        System.Threading.Tasks.Task RollbackTransactionAsync();
    }
}