SET QUOTED_IDENTIFIER ON;
GO

USE TaskManagementDB;
GO

-- Clear existing data
DELETE FROM Core.Tasks;
DELETE FROM Core.Projects;
DELETE FROM Core.Clients;
DELETE FROM Security.Users;
DELETE FROM Core.Companies;
GO

-- Insert test companies in a single batch with predefined IDs
DECLARE @Company1Id UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @Company2Id UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';

INSERT INTO Core.Companies (Id, Name, Domain, ContactEmail, SubscriptionType, MaxUsers, IsActive)
VALUES 
(@Company1Id, 'Tech Innovations Inc', 'techinnovations.com', 'admin@techinnovations.com', 'Enterprise', 100, 1),
(@Company2Id, 'Digital Solutions Ltd', 'digitalsolutions.com', 'contact@digitalsolutions.com', 'Premium', 50, 1);

-- Insert users with predefined IDs
DECLARE @SuperAdminId UNIQUEIDENTIFIER = 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA';
DECLARE @JohnId UNIQUEIDENTIFIER = 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB';
DECLARE @SarahId UNIQUEIDENTIFIER = 'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC';
DECLARE @MikeId UNIQUEIDENTIFIER = 'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD';

INSERT INTO Security.Users (Id, CompanyId, Email, PasswordHash, PasswordSalt, FirstName, LastName, Role, IsActive, EmailVerified)
VALUES 
(@SuperAdminId, NULL, 'superadmin@taskmanagement.com', 'hashed_password', 'salt123', 'Super', 'Admin', 'SuperAdmin', 1, 1),
(@JohnId, @Company1Id, 'john.admin@techinnovations.com', 'hashed_password', 'salt123', 'John', 'Admin', 'CompanyAdmin', 1, 1),
(@SarahId, @Company1Id, 'sarah.manager@techinnovations.com', 'hashed_password', 'salt123', 'Sarah', 'Manager', 'Manager', 1, 1),
(@MikeId, @Company1Id, 'mike.user@techinnovations.com', 'hashed_password', 'salt123', 'Mike', 'Developer', 'User', 1, 1);

-- Insert sample clients
DECLARE @Client1Id UNIQUEIDENTIFIER = 'EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE';
INSERT INTO Core.Clients (Id, CompanyId, Name, Email, Phone, ContactPerson, IsActive, CreatedById)
VALUES 
(@Client1Id, @Company1Id, 'ABC Corporation', 'contact@abccorp.com', '123-456-7890', 'Alice Brown', 1, @JohnId);

-- Insert sample projects
DECLARE @Project1Id UNIQUEIDENTIFIER = 'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF';
INSERT INTO Core.Projects (Id, CompanyId, ClientId, Name, Description, ProjectCode, Status, ProjectManagerId, StartDate, EndDate, CreatedById)
VALUES 
(@Project1Id, @Company1Id, @Client1Id, 'Website Redesign', 'Complete redesign of corporate website', 'PROJ-2024-001', 'Active', @SarahId, GETDATE(), DATEADD(month, 3, GETDATE()), @SarahId);

-- Insert sample tasks
INSERT INTO Core.Tasks (CompanyId, Title, Description, TaskNumber, AssignedToId, AssignedById, ProjectId, Priority, Status, DueDate, CreatedById)
VALUES 
(@Company1Id, 'Design Homepage Mockup', 'Create initial design mockups for the new homepage', 'TSK-2024-0001', @MikeId, @SarahId, @Project1Id, 'High', 'InProgress', DATEADD(day, 7, GETDATE()), @SarahId),
(@Company1Id, 'Setup Development Environment', 'Configure development environment for the project', 'TSK-2024-0002', @MikeId, @SarahId, @Project1Id, 'Medium', 'Completed', DATEADD(day, -2, GETDATE()), @SarahId),
(@Company1Id, 'Database Schema Design', 'Design database schema for new features', 'TSK-2024-0003', @MikeId, @SarahId, @Project1Id, 'High', 'Pending', DATEADD(day, 14, GETDATE()), @SarahId);

PRINT 'Seed data inserted successfully!';
GO