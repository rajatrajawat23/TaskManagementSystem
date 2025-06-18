-- Add IsDeleted column to all tables
USE TaskManagementDB;
GO

-- Add IsDeleted to all tables in Core schema
ALTER TABLE Core.Companies ADD IsDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE Core.Clients ADD IsDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE Core.Projects ADD IsDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE Core.Tasks ADD IsDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE Core.SubTasks ADD IsDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE Core.TaskComments ADD IsDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE Core.TaskAttachments ADD IsDeleted BIT NOT NULL DEFAULT 0;
GO

-- Add IsDeleted to all tables in Security schema
ALTER TABLE Security.Users ADD IsDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE Security.UserPermissions ADD IsDeleted BIT NOT NULL DEFAULT 0;
GO

-- Add IsDeleted to all tables in Communication schema
ALTER TABLE Communication.ChatGroups ADD IsDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE Communication.ChatGroupMembers ADD IsDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE Communication.ChatMessages ADD IsDeleted BIT NOT NULL DEFAULT 0;
ALTER TABLE Communication.Notifications ADD IsDeleted BIT NOT NULL DEFAULT 0;
GO

PRINT 'IsDeleted column added to all tables successfully!';
GO
