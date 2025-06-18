USE TaskManagementDB;
GO

-- Add missing indexes for TaskComments
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaskComments_TaskId' AND object_id = OBJECT_ID('Core.TaskComments'))
BEGIN
    CREATE INDEX IX_TaskComments_TaskId ON Core.TaskComments(TaskId);
    PRINT 'Created index IX_TaskComments_TaskId';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaskComments_CreatedById' AND object_id = OBJECT_ID('Core.TaskComments'))
BEGIN
    CREATE INDEX IX_TaskComments_CreatedById ON Core.TaskComments(CreatedById);
    PRINT 'Created index IX_TaskComments_CreatedById';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaskComments_TaskId_CreatedAt' AND object_id = OBJECT_ID('Core.TaskComments'))
BEGIN
    CREATE INDEX IX_TaskComments_TaskId_CreatedAt ON Core.TaskComments(TaskId, CreatedAt);
    PRINT 'Created index IX_TaskComments_TaskId_CreatedAt';
END
GO

-- Add missing foreign key for TaskComments.User if not exists
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TaskComments_Users_CreatedById')
BEGIN
    ALTER TABLE Core.TaskComments
    ADD CONSTRAINT FK_TaskComments_Users_CreatedById
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id);
    PRINT 'Created foreign key FK_TaskComments_Users_CreatedById';
END
GO

-- Mark the AddTaskCommentsAndAttachments migration as applied
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20250617091717_AddTaskCommentsAndAttachments')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250617091717_AddTaskCommentsAndAttachments', '9.0.0');
    PRINT 'Migration marked as applied successfully!';
END
GO

PRINT 'All indexes and relations added successfully!';
