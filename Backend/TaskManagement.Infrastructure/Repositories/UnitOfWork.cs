using System;
using Microsoft.EntityFrameworkCore.Storage;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskManagementDbContext _context;
        private IDbContextTransaction? _transaction;

        // Specific repositories
        private ITaskRepository? _tasks;
        private IProjectRepository? _projects;
        private IClientRepository? _clients;
        private IUserRepository? _users;
        private ICompanyRepository? _companies;
        
        // Generic repositories
        private IGenericRepository<SubTask>? _subTasks;
        private IGenericRepository<ChatGroup>? _chatGroups;
        private IGenericRepository<ChatMessage>? _chatMessages;
        private IGenericRepository<Notification>? _notifications;
        private IGenericRepository<TaskAttachment>? _taskAttachments;
        private IGenericRepository<TaskComment>? _taskComments;
        private IGenericRepository<UserPermission>? _userPermissions;

        public UnitOfWork(TaskManagementDbContext context)
        {
            _context = context;
        }

        // Specific repositories with custom methods
        public ITaskRepository Tasks => 
            _tasks ??= new TaskRepository(_context);

        public IProjectRepository Projects => 
            _projects ??= new ProjectRepository(_context);

        public IClientRepository Clients => 
            _clients ??= new ClientRepository(_context);

        public IUserRepository Users => 
            _users ??= new UserRepository(_context);

        public ICompanyRepository Companies => 
            _companies ??= new CompanyRepository(_context);

        // Generic repositories
        public IGenericRepository<SubTask> SubTasks => 
            _subTasks ??= new GenericRepository<SubTask>(_context);

        public IGenericRepository<ChatGroup> ChatGroups => 
            _chatGroups ??= new GenericRepository<ChatGroup>(_context);

        public IGenericRepository<ChatMessage> ChatMessages => 
            _chatMessages ??= new GenericRepository<ChatMessage>(_context);

        public IGenericRepository<Notification> Notifications => 
            _notifications ??= new GenericRepository<Notification>(_context);

        public IGenericRepository<TaskAttachment> TaskAttachments => 
            _taskAttachments ??= new GenericRepository<TaskAttachment>(_context);

        public IGenericRepository<TaskComment> TaskComments => 
            _taskComments ??= new GenericRepository<TaskComment>(_context);

        public IGenericRepository<UserPermission> UserPermissions => 
            _userPermissions ??= new GenericRepository<UserPermission>(_context);

        public async System.Threading.Tasks.Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async System.Threading.Tasks.Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async System.Threading.Tasks.Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async System.Threading.Tasks.Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}