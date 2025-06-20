USE TaskManagementDB;
GO

-- Create the EF Core migrations history table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='__EFMigrationsHistory' AND xtype='U')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId nvarchar(150) NOT NULL,
        ProductVersion nvarchar(32) NOT NULL,
        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );
END
GO

-- Mark the InitialCreate migration as applied since we created the database using SQL scripts
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20250616072430_InitialCreate')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250616072430_InitialCreate', '9.0.0');
END
GO

PRINT 'Initial migration marked as applied successfully!';
