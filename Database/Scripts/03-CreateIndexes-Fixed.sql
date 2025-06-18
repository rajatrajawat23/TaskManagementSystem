SET QUOTED_IDENTIFIER ON;
GO

USE TaskManagementDB;
GO

-- Performance Indexes for Companies
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Companies_Domain')
    CREATE NONCLUSTERED INDEX IX_Companies_Domain ON Core.Companies(Domain) WHERE IsActive = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Companies_SubscriptionType')
    CREATE NONCLUSTERED INDEX IX_Companies_SubscriptionType ON Core.Companies(SubscriptionType) WHERE IsActive = 1;

-- Performance Indexes for Users
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
    CREATE NONCLUSTERED INDEX IX_Users_Email ON Security.Users(Email) WHERE IsActive = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_CompanyId')
    CREATE NONCLUSTERED INDEX IX_Users_CompanyId ON Security.Users(CompanyId) WHERE IsActive = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Role')
    CREATE NONCLUSTERED INDEX IX_Users_Role ON Security.Users(Role) WHERE IsActive = 1;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_CompanyId_Role')
    CREATE NONCLUSTERED INDEX IX_Users_CompanyId_Role ON Security.Users(CompanyId, Role) WHERE IsActive = 1;

-- Performance Indexes for Tasks
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_CompanyId')
    CREATE NONCLUSTERED INDEX IX_Tasks_CompanyId ON Core.Tasks(CompanyId) WHERE IsArchived = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_AssignedToId')
    CREATE NONCLUSTERED INDEX IX_Tasks_AssignedToId ON Core.Tasks(AssignedToId) WHERE IsArchived = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_Status')
    CREATE NONCLUSTERED INDEX IX_Tasks_Status ON Core.Tasks(Status) WHERE IsArchived = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_DueDate')
    CREATE NONCLUSTERED INDEX IX_Tasks_DueDate ON Core.Tasks(DueDate) WHERE IsArchived = 0;

-- Other indexes without filters (simpler approach)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SubTasks_TaskId')
    CREATE NONCLUSTERED INDEX IX_SubTasks_TaskId ON Core.SubTasks(TaskId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_CompanyId')
    CREATE NONCLUSTERED INDEX IX_Projects_CompanyId ON Core.Projects(CompanyId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatMessages_GroupId')
    CREATE NONCLUSTERED INDEX IX_ChatMessages_GroupId ON Communication.ChatMessages(GroupId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_UserId')
    CREATE NONCLUSTERED INDEX IX_Notifications_UserId ON Communication.Notifications(UserId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ActivityLogs_CreatedAt')
    CREATE NONCLUSTERED INDEX IX_ActivityLogs_CreatedAt ON Core.ActivityLogs(CreatedAt DESC);
GO