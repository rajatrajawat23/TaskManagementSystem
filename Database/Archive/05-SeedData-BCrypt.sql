SET QUOTED_IDENTIFIER ON;
GO

USE TaskManagementDB;
GO

-- First, disable constraints
ALTER TABLE Core.Tasks NOCHECK CONSTRAINT ALL;
ALTER TABLE Core.Projects NOCHECK CONSTRAINT ALL;
ALTER TABLE Core.Clients NOCHECK CONSTRAINT ALL;
ALTER TABLE Security.Users NOCHECK CONSTRAINT ALL;
ALTER TABLE Core.Companies NOCHECK CONSTRAINT ALL;
GO

-- Delete existing data in reverse order
DELETE FROM Core.Tasks;
DELETE FROM Core.Projects;
DELETE FROM Core.Clients;
DELETE FROM Security.Users;
DELETE FROM Core.Companies;
GO

-- Re-enable constraints
ALTER TABLE Core.Companies CHECK CONSTRAINT ALL;
ALTER TABLE Security.Users CHECK CONSTRAINT ALL;
ALTER TABLE Core.Clients CHECK CONSTRAINT ALL;
ALTER TABLE Core.Projects CHECK CONSTRAINT ALL;
ALTER TABLE Core.Tasks CHECK CONSTRAINT ALL;
GO

-- Insert test companies
INSERT INTO Core.Companies (Id, Name, Domain, ContactEmail, SubscriptionType, MaxUsers, IsActive)
VALUES 
('11111111-1111-1111-1111-111111111111', 'Tech Innovations Inc', 'techinnovations.com', 'admin@techinnovations.com', 'Enterprise', 100, 1),
('22222222-2222-2222-2222-222222222222', 'Digital Solutions Ltd', 'digitalsolutions.com', 'contact@digitalsolutions.com', 'Premium', 50, 1),
('33333333-3333-3333-3333-333333333333', 'StartUp Hub', 'startuphub.com', 'info@startuphub.com', 'Free', 10, 1);
GO

-- Insert users with BCrypt hashed passwords
-- All passwords are: Admin@123
-- BCrypt hash for Admin@123: $2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a
INSERT INTO Security.Users (Id, CompanyId, Email, PasswordHash, PasswordSalt, FirstName, LastName, Role, Department, IsActive, EmailVerified)
VALUES 
-- SuperAdmin (no company)
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', NULL, 'superadmin@taskmanagement.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Super', 'Admin', 'SuperAdmin', NULL, 1, 1),

-- Tech Innovations Inc users
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '11111111-1111-1111-1111-111111111111', 'john.admin@techinnovations.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'John', 'Admin', 'CompanyAdmin', 'Management', 1, 1),
 
('cccccccc-cccc-cccc-cccc-cccccccccccc', '11111111-1111-1111-1111-111111111111', 'sarah.manager@techinnovations.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Sarah', 'Manager', 'Manager', 'Development', 1, 1),
 
('dddddddd-dddd-dddd-dddd-dddddddddddd', '11111111-1111-1111-1111-111111111111', 'mike.user@techinnovations.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Mike', 'Developer', 'User', 'Development', 1, 1),

-- Digital Solutions Ltd users
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '22222222-2222-2222-2222-222222222222', 'emma.admin@digitalsolutions.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Emma', 'Wilson', 'CompanyAdmin', 'Management', 1, 1),
 
('ffffffff-ffff-ffff-ffff-ffffffffffff', '22222222-2222-2222-2222-222222222222', 'david.manager@digitalsolutions.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'David', 'Brown', 'Manager', 'Operations', 1, 1);
GO

-- Insert sample clients
INSERT INTO Core.Clients (Id, CompanyId, Name, Email, Phone, ContactPerson, IsActive, CreatedById)
VALUES 
('11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-111111111111', 'ABC Corporation', 'contact@abccorp.com', '123-456-7890', 'Alice Brown', 1, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'),
('11111111-1111-1111-1111-000000000002', '11111111-1111-1111-1111-111111111111', 'XYZ Industries', 'info@xyzindustries.com', '098-765-4321', 'Bob Smith', 1, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb');
GO

-- Insert sample projects
INSERT INTO Core.Projects (Id, CompanyId, ClientId, Name, Description, ProjectCode, Status, ProjectManagerId, StartDate, EndDate, CreatedById)
VALUES 
('11111111-1111-1111-1111-100000000001', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-000000000001', 
 'Website Redesign', 'Complete redesign of corporate website', 'PROJ-2024-001', 'Active', 
 'cccccccc-cccc-cccc-cccc-cccccccccccc', GETDATE(), DATEADD(month, 3, GETDATE()), 'cccccccc-cccc-cccc-cccc-cccccccccccc');
GO

-- Insert sample tasks
INSERT INTO Core.Tasks (Id, CompanyId, Title, Description, TaskNumber, AssignedToId, AssignedById, ProjectId, Priority, Status, DueDate, CreatedById)
VALUES 
('11111111-1111-1111-1111-200000000001', '11111111-1111-1111-1111-111111111111', 'Design Homepage Mockup', 
 'Create initial design mockups for the new homepage', 'TSK-2024-0001', 
 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 
 '11111111-1111-1111-1111-100000000001', 'High', 'InProgress', 
 DATEADD(day, 7, GETDATE()), 'cccccccc-cccc-cccc-cccc-cccccccccccc'),
 
('11111111-1111-1111-1111-200000000002', '11111111-1111-1111-1111-111111111111', 'Setup Development Environment', 
 'Configure development environment for the project', 'TSK-2024-0002', 
 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 
 '11111111-1111-1111-1111-100000000001', 'Medium', 'Completed', 
 DATEADD(day, -2, GETDATE()), 'cccccccc-cccc-cccc-cccc-cccccccccccc'),
 
('11111111-1111-1111-1111-200000000003', '11111111-1111-1111-1111-111111111111', 'Database Schema Design', 
 'Design database schema for new features', 'TSK-2024-0003', 
 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 
 '11111111-1111-1111-1111-100000000001', 'High', 'Pending', 
 DATEADD(day, 14, GETDATE()), 'cccccccc-cccc-cccc-cccc-cccccccccccc');
GO

PRINT 'Seed data with BCrypt passwords inserted successfully!';
PRINT 'All users have password: Admin@123';
PRINT '';
PRINT 'Test users:';
PRINT '- superadmin@taskmanagement.com (SuperAdmin)';
PRINT '- john.admin@techinnovations.com (CompanyAdmin)';
PRINT '- sarah.manager@techinnovations.com (Manager)';
PRINT '- mike.user@techinnovations.com (User)';
PRINT '- emma.admin@digitalsolutions.com (CompanyAdmin)';
PRINT '- david.manager@digitalsolutions.com (Manager)';
GO
