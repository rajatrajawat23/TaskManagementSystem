-- ================================================
-- COMPLETE DATABASE MIGRATION SCRIPT
-- TaskManagement System - All-in-One Setup
-- ================================================

-- Create Database and Initial Setup
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TaskManagementDB')
BEGIN
    CREATE DATABASE TaskManagementDB;
END
GO

USE TaskManagementDB;
GO

-- Create schemas for organization
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Core')
    EXEC('CREATE SCHEMA Core');
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Security')
    EXEC('CREATE SCHEMA Security');
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Communication')
    EXEC('CREATE SCHEMA Communication');
GO

-- Create login for the application user
IF NOT EXISTS (SELECT * FROM sys.sql_logins WHERE name = 'taskadmin')
BEGIN
    CREATE LOGIN taskadmin WITH PASSWORD = 'SecureTask2025#@!';
END
GO

-- Create user in the database
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'taskadmin')
BEGIN
    CREATE USER taskadmin FOR LOGIN taskadmin;
    ALTER ROLE db_owner ADD MEMBER taskadmin;
END
GO

-- ================================================
-- CORE TABLES CREATION
-- ================================================

-- 1. Companies Table (Multi-tenant foundation)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Companies' AND xtype='U')
CREATE TABLE Core.Companies
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Domain NVARCHAR(50) UNIQUE NOT NULL,
    ContactEmail NVARCHAR(100) NOT NULL,
    ContactPhone NVARCHAR(20),
    Address NVARCHAR(500),
    SubscriptionType NVARCHAR(20) DEFAULT 'Free' CHECK (SubscriptionType IN ('Free', 'Premium', 'Enterprise')),
    SubscriptionExpiryDate DATETIME2,
    MaxUsers INT DEFAULT 10,
    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER
);
GO

-- 2. Users Table (Authentication & Authorization)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
CREATE TABLE Security.Users
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER, -- NULL for SuperAdmin
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    PasswordSalt NVARCHAR(500) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    PhoneNumber NVARCHAR(20),
    ProfileImageUrl NVARCHAR(500),
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('SuperAdmin', 'CompanyAdmin', 'Manager', 'User', 'TaskAssigner')),
    Department NVARCHAR(50),
    JobTitle NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    EmailVerified BIT DEFAULT 0,
    LastLoginAt DATETIME2,
    PasswordResetToken NVARCHAR(500),
    PasswordResetExpiry DATETIME2,
    RefreshToken NVARCHAR(500),
    RefreshTokenExpiry DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id)
);
GO

-- 3. Clients Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Clients' AND xtype='U')
CREATE TABLE Core.Clients
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    ContactPerson NVARCHAR(100),
    Address NVARCHAR(500),
    Website NVARCHAR(200),
    Industry NVARCHAR(50),
    Notes NVARCHAR(MAX),
    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id),
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO

-- 4. Projects Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Projects' AND xtype='U')
CREATE TABLE Core.Projects
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    ClientId UNIQUEIDENTIFIER,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    ProjectCode NVARCHAR(20) UNIQUE,
    StartDate DATETIME2,
    EndDate DATETIME2,
    Budget DECIMAL(12,2),
    Status NVARCHAR(20) DEFAULT 'Active' CHECK (Status IN ('Planning', 'Active', 'OnHold', 'Completed', 'Cancelled')),
    ProjectManagerId UNIQUEIDENTIFIER,
    TeamMembers NVARCHAR(MAX), -- JSON array of user IDs
    Progress INT DEFAULT 0 CHECK (Progress >= 0 AND Progress <= 100),
    IsArchived BIT DEFAULT 0,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id),
    FOREIGN KEY (ClientId) REFERENCES Core.Clients(Id),
    FOREIGN KEY (ProjectManagerId) REFERENCES Security.Users(Id),
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO

-- 5. Tasks Table (Core business entity)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tasks' AND xtype='U')
CREATE TABLE Core.Tasks
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    TaskNumber NVARCHAR(20) UNIQUE, -- Auto-generated: TSK-2024-0001
    AssignedToId UNIQUEIDENTIFIER,
    AssignedById UNIQUEIDENTIFIER NOT NULL,
    ClientId UNIQUEIDENTIFIER, -- Optional
    ProjectId UNIQUEIDENTIFIER, -- Optional
    Priority NVARCHAR(10) DEFAULT 'Medium' CHECK (Priority IN ('Low', 'Medium', 'High', 'Critical')),
    Status NVARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'InProgress', 'Review', 'Completed', 'Cancelled')),
    Category NVARCHAR(50),
    Tags NVARCHAR(500), -- JSON array of tags
    EstimatedHours DECIMAL(5,2),
    ActualHours DECIMAL(5,2),
    StartDate DATETIME2,
    DueDate DATETIME2,
    CompletedDate DATETIME2,
    IsRecurring BIT DEFAULT 0,
    RecurrencePattern NVARCHAR(100), -- JSON for recurrence rules
    ParentTaskId UNIQUEIDENTIFIER, -- For subtasks
    Progress INT DEFAULT 0 CHECK (Progress >= 0 AND Progress <= 100),
    IsArchived BIT DEFAULT 0,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id),
    FOREIGN KEY (AssignedToId) REFERENCES Security.Users(Id),
    FOREIGN KEY (AssignedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (ClientId) REFERENCES Core.Clients(Id),
    FOREIGN KEY (ProjectId) REFERENCES Core.Projects(Id),
    FOREIGN KEY (ParentTaskId) REFERENCES Core.Tasks(Id),
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO

-- 6. SubTasks Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SubTasks' AND xtype='U')
CREATE TABLE Core.SubTasks
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    IsCompleted BIT DEFAULT 0,
    CompletedAt DATETIME2,
    CompletedById UNIQUEIDENTIFIER,
    SortOrder INT DEFAULT 0,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (TaskId) REFERENCES Core.Tasks(Id) ON DELETE CASCADE,
    FOREIGN KEY (CompletedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO

-- 7. TaskComments Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaskComments' AND xtype='U')
CREATE TABLE Core.TaskComments
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskId UNIQUEIDENTIFIER NOT NULL,
    Comment NVARCHAR(MAX) NOT NULL, -- Using MAX to match current database (Entity has 1000 limit in validation)
    IsInternal BIT DEFAULT 0, -- Internal comments not visible to clients
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER NOT NULL,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (TaskId) REFERENCES Core.Tasks(Id) ON DELETE CASCADE,
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO

-- 8. TaskAttachments Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaskAttachments' AND xtype='U')
CREATE TABLE Core.TaskAttachments
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskId UNIQUEIDENTIFIER NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    FileUrl NVARCHAR(500) NOT NULL,
    FileSize BIGINT,
    FileType NVARCHAR(50),
    IsDeleted BIT DEFAULT 0,
    UploadedAt DATETIME2 DEFAULT GETUTCDATE(),
    UploadedById UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (TaskId) REFERENCES Core.Tasks(Id) ON DELETE CASCADE,
    FOREIGN KEY (UploadedById) REFERENCES Security.Users(Id)
);
GO

-- 9. ChatGroups Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChatGroups' AND xtype='U')
CREATE TABLE Communication.ChatGroups
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    GroupType NVARCHAR(20) DEFAULT 'General' CHECK (GroupType IN ('General', 'Project', 'Department', 'Private')),
    RelatedProjectId UNIQUEIDENTIFIER,
    Members NVARCHAR(MAX) NOT NULL, -- JSON array of user IDs
    IsActive BIT DEFAULT 1,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id),
    FOREIGN KEY (RelatedProjectId) REFERENCES Core.Projects(Id),
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO

-- 10. ChatMessages Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChatMessages' AND xtype='U')
CREATE TABLE Communication.ChatMessages
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GroupId UNIQUEIDENTIFIER NOT NULL,
    SenderId UNIQUEIDENTIFIER NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    MessageType NVARCHAR(20) DEFAULT 'Text' CHECK (MessageType IN ('Text', 'File', 'Image', 'System')),
    AttachmentUrl NVARCHAR(500),
    IsEdited BIT DEFAULT 0,
    EditedAt DATETIME2,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2,
    ReadBy NVARCHAR(MAX), -- JSON array of user IDs who have read
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (GroupId) REFERENCES Communication.ChatGroups(Id) ON DELETE CASCADE,
    FOREIGN KEY (SenderId) REFERENCES Security.Users(Id)
);
GO

-- 11. Notifications Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notifications' AND xtype='U')
CREATE TABLE Communication.Notifications
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    NotificationType NVARCHAR(50) NOT NULL CHECK (NotificationType IN ('TaskAssigned', 'TaskUpdated', 'TaskCompleted', 'TaskOverdue', 'ChatMessage', 'SystemAlert')),
    RelatedEntityId UNIQUEIDENTIFIER,
    RelatedEntityType NVARCHAR(50),
    IsRead BIT DEFAULT 0,
    ReadAt DATETIME2,
    Priority NVARCHAR(10) DEFAULT 'Normal' CHECK (Priority IN ('Low', 'Normal', 'High', 'Critical')),
    ExpiryDate DATETIME2,
    IsDeleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Security.Users(Id) ON DELETE CASCADE
);
GO

-- 12. UserPermissions Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserPermissions' AND xtype='U')
CREATE TABLE Security.UserPermissions
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    PermissionType NVARCHAR(50) NOT NULL,
    PermissionValue NVARCHAR(50) NOT NULL,
    GrantedAt DATETIME2 DEFAULT GETUTCDATE(),
    GrantedById UNIQUEIDENTIFIER,
    ExpiryDate DATETIME2,
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES Security.Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (GrantedById) REFERENCES Security.Users(Id)
);
GO

-- 13. ActivityLogs Table (Audit Trail)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ActivityLogs' AND xtype='U')
CREATE TABLE Core.ActivityLogs
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER,
    UserId UNIQUEIDENTIFIER NOT NULL,
    ActivityType NVARCHAR(50) NOT NULL,
    EntityType NVARCHAR(50),
    EntityId UNIQUEIDENTIFIER,
    OldValues NVARCHAR(MAX), -- JSON of old values
    NewValues NVARCHAR(MAX), -- JSON of new values
    Description NVARCHAR(500),
    IpAddress NVARCHAR(45),
    UserAgent NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id),
    FOREIGN KEY (UserId) REFERENCES Security.Users(Id)
);
GO

-- 14. EmailTemplates Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmailTemplates' AND xtype='U')
CREATE TABLE Communication.EmailTemplates
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER,
    TemplateName NVARCHAR(100) NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    TemplateType NVARCHAR(50) NOT NULL CHECK (TemplateType IN ('TaskAssignment', 'TaskReminder', 'Welcome', 'PasswordReset', 'Custom')),
    Variables NVARCHAR(500), -- JSON array of available variables
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id),
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO

-- 15. SystemSettings Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SystemSettings' AND xtype='U')
CREATE TABLE Core.SystemSettings
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER,
    SettingKey NVARCHAR(100) NOT NULL,
    SettingValue NVARCHAR(MAX),
    SettingType NVARCHAR(50) DEFAULT 'String',
    Description NVARCHAR(500),
    IsEncrypted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id),
    UNIQUE(CompanyId, SettingKey)
);
GO

-- ================================================
-- ADDITIONAL FOREIGN KEY CONSTRAINTS
-- ================================================

-- Add FK constraints for self-referencing tables after creation
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Companies_Users_CreatedBy')
    ALTER TABLE Core.Companies ADD CONSTRAINT FK_Companies_Users_CreatedBy FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id);

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Companies_Users_UpdatedBy')
    ALTER TABLE Core.Companies ADD CONSTRAINT FK_Companies_Users_UpdatedBy FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id);

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Users_CreatedBy')
    ALTER TABLE Security.Users ADD CONSTRAINT FK_Users_Users_CreatedBy FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id);

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Users_UpdatedBy')
    ALTER TABLE Security.Users ADD CONSTRAINT FK_Users_Users_UpdatedBy FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id);

-- ================================================
-- PERFORMANCE INDEXES
-- ================================================

-- Companies Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Companies_Domain')
    CREATE NONCLUSTERED INDEX IX_Companies_Domain ON Core.Companies(Domain) WHERE IsActive = 1 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Companies_SubscriptionType')
    CREATE NONCLUSTERED INDEX IX_Companies_SubscriptionType ON Core.Companies(SubscriptionType) WHERE IsActive = 1 AND IsDeleted = 0;

-- Users Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
    CREATE NONCLUSTERED INDEX IX_Users_Email ON Security.Users(Email) WHERE IsActive = 1 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_CompanyId')
    CREATE NONCLUSTERED INDEX IX_Users_CompanyId ON Security.Users(CompanyId) WHERE IsActive = 1 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Role')
    CREATE NONCLUSTERED INDEX IX_Users_Role ON Security.Users(Role) WHERE IsActive = 1 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_CompanyId_Role')
    CREATE NONCLUSTERED INDEX IX_Users_CompanyId_Role ON Security.Users(CompanyId, Role) WHERE IsActive = 1 AND IsDeleted = 0;

-- Tasks Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_CompanyId')
    CREATE NONCLUSTERED INDEX IX_Tasks_CompanyId ON Core.Tasks(CompanyId) WHERE IsArchived = 0 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_AssignedToId')
    CREATE NONCLUSTERED INDEX IX_Tasks_AssignedToId ON Core.Tasks(AssignedToId) WHERE IsArchived = 0 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_Status')
    CREATE NONCLUSTERED INDEX IX_Tasks_Status ON Core.Tasks(Status) WHERE IsArchived = 0 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_DueDate')
    CREATE NONCLUSTERED INDEX IX_Tasks_DueDate ON Core.Tasks(DueDate) WHERE IsArchived = 0 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_CompanyId_Status')
    CREATE NONCLUSTERED INDEX IX_Tasks_CompanyId_Status ON Core.Tasks(CompanyId, Status) WHERE IsArchived = 0 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_AssignedToId_Status')
    CREATE NONCLUSTERED INDEX IX_Tasks_AssignedToId_Status ON Core.Tasks(AssignedToId, Status) WHERE IsArchived = 0 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_ProjectId')
    CREATE NONCLUSTERED INDEX IX_Tasks_ProjectId ON Core.Tasks(ProjectId) WHERE ProjectId IS NOT NULL AND IsArchived = 0 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_ClientId')
    CREATE NONCLUSTERED INDEX IX_Tasks_ClientId ON Core.Tasks(ClientId) WHERE ClientId IS NOT NULL AND IsArchived = 0 AND IsDeleted = 0;

-- SubTasks Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SubTasks_TaskId')
    CREATE NONCLUSTERED INDEX IX_SubTasks_TaskId ON Core.SubTasks(TaskId) WHERE IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_SubTasks_TaskId_SortOrder')
    CREATE NONCLUSTERED INDEX IX_SubTasks_TaskId_SortOrder ON Core.SubTasks(TaskId, SortOrder) WHERE IsDeleted = 0;

-- Clients Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_CompanyId')
    CREATE NONCLUSTERED INDEX IX_Clients_CompanyId ON Core.Clients(CompanyId) WHERE IsActive = 1 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_Name')
    CREATE NONCLUSTERED INDEX IX_Clients_Name ON Core.Clients(Name) WHERE IsActive = 1 AND IsDeleted = 0;

-- Projects Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_CompanyId')
    CREATE NONCLUSTERED INDEX IX_Projects_CompanyId ON Core.Projects(CompanyId) WHERE IsArchived = 0 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_ClientId')
    CREATE NONCLUSTERED INDEX IX_Projects_ClientId ON Core.Projects(ClientId) WHERE IsArchived = 0 AND IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_Status')
    CREATE NONCLUSTERED INDEX IX_Projects_Status ON Core.Projects(Status) WHERE IsArchived = 0 AND IsDeleted = 0;

-- TaskComments Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaskComments_TaskId')
    CREATE NONCLUSTERED INDEX IX_TaskComments_TaskId ON Core.TaskComments(TaskId) WHERE IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaskComments_CreatedById')
    CREATE NONCLUSTERED INDEX IX_TaskComments_CreatedById ON Core.TaskComments(CreatedById) WHERE IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaskComments_TaskId_CreatedAt')
    CREATE NONCLUSTERED INDEX IX_TaskComments_TaskId_CreatedAt ON Core.TaskComments(TaskId, CreatedAt) WHERE IsDeleted = 0;

-- TaskAttachments Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaskAttachments_TaskId')
    CREATE NONCLUSTERED INDEX IX_TaskAttachments_TaskId ON Core.TaskAttachments(TaskId) WHERE IsDeleted = 0;

-- Notifications Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_UserId')
    CREATE NONCLUSTERED INDEX IX_Notifications_UserId ON Communication.Notifications(UserId) WHERE IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_UserId_IsRead')
    CREATE NONCLUSTERED INDEX IX_Notifications_UserId_IsRead ON Communication.Notifications(UserId, IsRead) WHERE IsDeleted = 0;

-- ChatGroups Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatGroups_CompanyId')
    CREATE NONCLUSTERED INDEX IX_ChatGroups_CompanyId ON Communication.ChatGroups(CompanyId) WHERE IsActive = 1 AND IsDeleted = 0;

-- ChatMessages Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatMessages_GroupId')
    CREATE NONCLUSTERED INDEX IX_ChatMessages_GroupId ON Communication.ChatMessages(GroupId) WHERE IsDeleted = 0;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatMessages_SenderId')
    CREATE NONCLUSTERED INDEX IX_ChatMessages_SenderId ON Communication.ChatMessages(SenderId) WHERE IsDeleted = 0;

-- ActivityLogs Indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ActivityLogs_CompanyId')
    CREATE NONCLUSTERED INDEX IX_ActivityLogs_CompanyId ON Core.ActivityLogs(CompanyId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ActivityLogs_UserId')
    CREATE NONCLUSTERED INDEX IX_ActivityLogs_UserId ON Core.ActivityLogs(UserId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ActivityLogs_CreatedAt')
    CREATE NONCLUSTERED INDEX IX_ActivityLogs_CreatedAt ON Core.ActivityLogs(CreatedAt);

-- ================================================
-- CREATE ENTITY FRAMEWORK MIGRATIONS HISTORY TABLE
-- ================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='__EFMigrationsHistory' AND xtype='U')
CREATE TABLE [dbo].[__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

-- Mark initial migration as applied
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20250617091717_InitialMigration')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250617091717_InitialMigration', '9.0.0');
END
GO

PRINT '============================================';
PRINT 'COMPLETE DATABASE MIGRATION SUCCESSFUL!';
PRINT '============================================';
PRINT 'Database: TaskManagementDB created';
PRINT 'Schemas: Core, Security, Communication created';
PRINT 'Tables: 15 core tables created with relationships';
PRINT 'Indexes: Performance indexes applied';
PRINT 'User: taskadmin created with full access';
PRINT 'Ready for data seeding!';
PRINT '============================================';
GO