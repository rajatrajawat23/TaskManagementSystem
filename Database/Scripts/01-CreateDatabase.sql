-- Create Database
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
USE TaskManagementDB;
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'taskadmin')
BEGIN
    CREATE USER taskadmin FOR LOGIN taskadmin;
    ALTER ROLE db_owner ADD MEMBER taskadmin;
END
GO