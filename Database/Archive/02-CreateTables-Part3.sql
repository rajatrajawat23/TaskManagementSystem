-- Additional Tables
USE TaskManagementDB;
GO

-- 7. ChatGroups Table
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

-- 8. ChatMessages Table
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

-- 9. Notifications Table
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
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Security.Users(Id) ON DELETE CASCADE
);
GO

-- 10. TaskAttachments Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaskAttachments' AND xtype='U')
CREATE TABLE Core.TaskAttachments
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskId UNIQUEIDENTIFIER NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    FileUrl NVARCHAR(500) NOT NULL,
    FileSize BIGINT,
    FileType NVARCHAR(50),
    UploadedAt DATETIME2 DEFAULT GETUTCDATE(),
    UploadedById UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (TaskId) REFERENCES Core.Tasks(Id) ON DELETE CASCADE,
    FOREIGN KEY (UploadedById) REFERENCES Security.Users(Id)
);
GO

-- 11. TaskComments Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaskComments' AND xtype='U')
CREATE TABLE Core.TaskComments
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TaskId UNIQUEIDENTIFIER NOT NULL,
    Comment NVARCHAR(MAX) NOT NULL,
    IsInternal BIT DEFAULT 0, -- Internal comments not visible to clients
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER NOT NULL,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (TaskId) REFERENCES Core.Tasks(Id) ON DELETE CASCADE,
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
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

-- Add Foreign Key constraints that reference Users table after it's created
ALTER TABLE Core.Companies ADD FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id);
ALTER TABLE Core.Companies ADD FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id);
ALTER TABLE Security.Users ADD FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id);
ALTER TABLE Security.Users ADD FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id);
ALTER TABLE Core.Tasks ADD FOREIGN KEY (ClientId) REFERENCES Core.Clients(Id);
ALTER TABLE Core.Tasks ADD FOREIGN KEY (ProjectId) REFERENCES Core.Projects(Id);
GO