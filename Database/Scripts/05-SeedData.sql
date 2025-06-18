USE TaskManagementDB;
GO

-- Insert test companies
INSERT INTO Core.Companies (Id, Name, Domain, ContactEmail, SubscriptionType, MaxUsers, IsActive)
VALUES 
(NEWID(), 'Tech Innovations Inc', 'techinnovations.com', 'admin@techinnovations.com', 'Enterprise', 100, 1),
(NEWID(), 'Digital Solutions Ltd', 'digitalsolutions.com', 'contact@digitalsolutions.com', 'Premium', 50, 1),
(NEWID(), 'StartUp Hub', 'startuphub.com', 'info@startuphub.com', 'Free', 10, 1);
GO

-- Get Company IDs for reference
DECLARE @CompanyId1 UNIQUEIDENTIFIER = (SELECT Id FROM Core.Companies WHERE Domain = 'techinnovations.com');
DECLARE @CompanyId2 UNIQUEIDENTIFIER = (SELECT Id FROM Core.Companies WHERE Domain = 'digitalsolutions.com');

-- Insert SuperAdmin user
INSERT INTO Security.Users (Id, Email, PasswordHash, PasswordSalt, FirstName, LastName, Role, IsActive, EmailVerified)
VALUES 
(NEWID(), 'superadmin@taskmanagement.com', 
 'AQAAAAEAACcQAAAAEGV5SxvKlCkW3VxOyY7EU1PRAhPdPiLVjFiOZRQtPLdqPx8rBg4F2yKmHtQeHwSi6g==', -- Password: Admin@123
 'salt123', 'Super', 'Admin', 'SuperAdmin', 1, 1);

-- Get SuperAdmin ID
DECLARE @SuperAdminId UNIQUEIDENTIFIER = (SELECT Id FROM Security.Users WHERE Email = 'superadmin@taskmanagement.com');

-- Insert Company Admins and Users
INSERT INTO Security.Users (Id, CompanyId, Email, PasswordHash, PasswordSalt, FirstName, LastName, Role, Department, IsActive, EmailVerified)
VALUES 
-- Tech Innovations Inc users
(NEWID(), @CompanyId1, 'john.admin@techinnovations.com', 
 'AQAAAAEAACcQAAAAEGV5SxvKlCkW3VxOyY7EU1PRAhPdPiLVjFiOZRQtPLdqPx8rBg4F2yKmHtQeHwSi6g==', 
 'salt123', 'John', 'Admin', 'CompanyAdmin', 'Management', 1, 1),
(NEWID(), @CompanyId1, 'sarah.manager@techinnovations.com', 
 'AQAAAAEAACcQAAAAEGV5SxvKlCkW3VxOyY7EU1PRAhPdPiLVjFiOZRQtPLdqPx8rBg4F2yKmHtQeHwSi6g==', 
 'salt123', 'Sarah', 'Manager', 'Manager', 'Development', 1, 1),
(NEWID(), @CompanyId1, 'mike.user@techinnovations.com', 
 'AQAAAAEAACcQAAAAEGV5SxvKlCkW3VxOyY7EU1PRAhPdPiLVjFiOZRQtPLdqPx8rBg4F2yKmHtQeHwSi6g==', 
 'salt123', 'Mike', 'Developer', 'User', 'Development', 1, 1),

-- Digital Solutions Ltd users
(NEWID(), @CompanyId2, 'emma.admin@digitalsolutions.com', 
 'AQAAAAEAACcQAAAAEGV5SxvKlCkW3VxOyY7EU1PRAhPdPiLVjFiOZRQtPLdqPx8rBg4F2yKmHtQeHwSi6g==', 
 'salt123', 'Emma', 'Wilson', 'CompanyAdmin', 'Management', 1, 1),
(NEWID(), @CompanyId2, 'david.manager@digitalsolutions.com', 
 'AQAAAAEAACcQAAAAEGV5SxvKlCkW3VxOyY7EU1PRAhPdPiLVjFiOZRQtPLdqPx8rBg4F2yKmHtQeHwSi6g==', 
 'salt123', 'David', 'Brown', 'Manager', 'Operations', 1, 1);
GO

-- Get User IDs for reference
DECLARE @JohnId UNIQUEIDENTIFIER = (SELECT Id FROM Security.Users WHERE Email = 'john.admin@techinnovations.com');
DECLARE @SarahId UNIQUEIDENTIFIER = (SELECT Id FROM Security.Users WHERE Email = 'sarah.manager@techinnovations.com');
DECLARE @MikeId UNIQUEIDENTIFIER = (SELECT Id FROM Security.Users WHERE Email = 'mike.user@techinnovations.com');
DECLARE @CompanyId1 UNIQUEIDENTIFIER = (SELECT Id FROM Core.Companies WHERE Domain = 'techinnovations.com');

-- Insert sample clients
INSERT INTO Core.Clients (Id, CompanyId, Name, Email, Phone, ContactPerson, IsActive, CreatedById)
VALUES 
(NEWID(), @CompanyId1, 'ABC Corporation', 'contact@abccorp.com', '123-456-7890', 'Alice Brown', 1, @JohnId),
(NEWID(), @CompanyId1, 'XYZ Industries', 'info@xyzindustries.com', '098-765-4321', 'Bob Smith', 1, @JohnId);
GO

-- Get Client ID for reference
DECLARE @ClientId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Core.Clients WHERE Name = 'ABC Corporation');
DECLARE @CompanyId1 UNIQUEIDENTIFIER = (SELECT Id FROM Core.Companies WHERE Domain = 'techinnovations.com');
DECLARE @SarahId UNIQUEIDENTIFIER = (SELECT Id FROM Security.Users WHERE Email = 'sarah.manager@techinnovations.com');

-- Insert sample projects
INSERT INTO Core.Projects (Id, CompanyId, ClientId, Name, Description, ProjectCode, Status, ProjectManagerId, StartDate, EndDate, CreatedById)
VALUES 
(NEWID(), @CompanyId1, @ClientId, 'Website Redesign', 'Complete redesign of corporate website', 'PROJ-2024-001', 'Active', @SarahId, GETDATE(), DATEADD(month, 3, GETDATE()), @SarahId);
GO

-- Insert sample tasks
DECLARE @ProjectId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Core.Projects WHERE ProjectCode = 'PROJ-2024-001');
DECLARE @CompanyId1 UNIQUEIDENTIFIER = (SELECT Id FROM Core.Companies WHERE Domain = 'techinnovations.com');
DECLARE @SarahId UNIQUEIDENTIFIER = (SELECT Id FROM Security.Users WHERE Email = 'sarah.manager@techinnovations.com');
DECLARE @MikeId UNIQUEIDENTIFIER = (SELECT Id FROM Security.Users WHERE Email = 'mike.user@techinnovations.com');

INSERT INTO Core.Tasks (Id, CompanyId, Title, Description, TaskNumber, AssignedToId, AssignedById, ProjectId, Priority, Status, DueDate, CreatedById)
VALUES 
(NEWID(), @CompanyId1, 'Design Homepage Mockup', 'Create initial design mockups for the new homepage', 'TSK-2024-0001', @MikeId, @SarahId, @ProjectId, 'High', 'InProgress', DATEADD(day, 7, GETDATE()), @SarahId),
(NEWID(), @CompanyId1, 'Setup Development Environment', 'Configure development environment for the project', 'TSK-2024-0002', @MikeId, @SarahId, @ProjectId, 'Medium', 'Completed', DATEADD(day, -2, GETDATE()), @SarahId),
(NEWID(), @CompanyId1, 'Database Schema Design', 'Design database schema for new features', 'TSK-2024-0003', @MikeId, @SarahId, @ProjectId, 'High', 'Pending', DATEADD(day, 14, GETDATE()), @SarahId);
GO

PRINT 'Seed data inserted successfully!';
GO