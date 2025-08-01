USE TaskManagementDB;
GO

-- Add missing columns to Notifications table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Communication' AND TABLE_NAME = 'Notifications' AND COLUMN_NAME = 'IsDeleted')
BEGIN
    ALTER TABLE Communication.Notifications ADD IsDeleted BIT NOT NULL DEFAULT(0);
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Communication' AND TABLE_NAME = 'Notifications' AND COLUMN_NAME = 'CreatedById')
BEGIN
    ALTER TABLE Communication.Notifications ADD CreatedById UNIQUEIDENTIFIER NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Communication' AND TABLE_NAME = 'Notifications' AND COLUMN_NAME = 'UpdatedById')
BEGIN
    ALTER TABLE Communication.Notifications ADD UpdatedById UNIQUEIDENTIFIER NULL;
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Communication' AND TABLE_NAME = 'Notifications' AND COLUMN_NAME = 'UpdatedAt')
BEGIN
    ALTER TABLE Communication.Notifications ADD UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
END
GO

PRINT 'Notifications table updated successfully!';
GO
