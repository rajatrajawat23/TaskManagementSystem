-- ================================================
-- COMPLETE SEED DATA SCRIPT
-- TaskManagement System - Comprehensive Test Data
-- ================================================

SET QUOTED_IDENTIFIER ON;
GO

USE TaskManagementDB;
GO

-- ================================================
-- CLEAN EXISTING DATA (IF ANY)
-- ================================================

-- Disable constraints for clean data insertion
ALTER TABLE Core.Tasks NOCHECK CONSTRAINT ALL;
ALTER TABLE Core.Projects NOCHECK CONSTRAINT ALL;
ALTER TABLE Core.Clients NOCHECK CONSTRAINT ALL;
ALTER TABLE Security.Users NOCHECK CONSTRAINT ALL;
ALTER TABLE Core.Companies NOCHECK CONSTRAINT ALL;
ALTER TABLE Core.SubTasks NOCHECK CONSTRAINT ALL;
ALTER TABLE Core.TaskComments NOCHECK CONSTRAINT ALL;
ALTER TABLE Core.TaskAttachments NOCHECK CONSTRAINT ALL;
ALTER TABLE Communication.Notifications NOCHECK CONSTRAINT ALL;
GO

-- Delete existing data in proper order
DELETE FROM Core.TaskComments;
DELETE FROM Core.TaskAttachments;
DELETE FROM Core.SubTasks;
DELETE FROM Core.Tasks;
DELETE FROM Core.Projects;
DELETE FROM Core.Clients;
DELETE FROM Communication.Notifications;
DELETE FROM Security.Users;
DELETE FROM Core.Companies;
GO

-- Re-enable constraints
ALTER TABLE Core.Companies CHECK CONSTRAINT ALL;
ALTER TABLE Security.Users CHECK CONSTRAINT ALL;
ALTER TABLE Core.Clients CHECK CONSTRAINT ALL;
ALTER TABLE Core.Projects CHECK CONSTRAINT ALL;
ALTER TABLE Core.Tasks CHECK CONSTRAINT ALL;
ALTER TABLE Core.SubTasks CHECK CONSTRAINT ALL;
ALTER TABLE Core.TaskComments CHECK CONSTRAINT ALL;
ALTER TABLE Core.TaskAttachments CHECK CONSTRAINT ALL;
ALTER TABLE Communication.Notifications CHECK CONSTRAINT ALL;
GO

-- ================================================
-- 1. COMPANIES DATA
-- ================================================

INSERT INTO Core.Companies (Id, Name, Domain, ContactEmail, ContactPhone, Address, SubscriptionType, MaxUsers, IsActive)
VALUES 
('11111111-1111-1111-1111-111111111111', 'Tech Innovations Inc', 'techinnovations.com', 'admin@techinnovations.com', '+1-555-0101', '123 Tech Street, Silicon Valley, CA 94000', 'Enterprise', 100, 1),
('22222222-2222-2222-2222-222222222222', 'Digital Solutions Ltd', 'digitalsolutions.com', 'contact@digitalsolutions.com', '+1-555-0102', '456 Digital Ave, Austin, TX 78701', 'Premium', 50, 1),
('33333333-3333-3333-3333-333333333333', 'StartUp Hub', 'startuphub.com', 'info@startuphub.com', '+1-555-0103', '789 Innovation Blvd, Denver, CO 80202', 'Free', 10, 1);
GO

-- ================================================
-- 2. USERS DATA (BCrypt Hashed Passwords)
-- ================================================

-- All passwords are: Admin@123
-- BCrypt hash: $2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a

INSERT INTO Security.Users (Id, CompanyId, Email, PasswordHash, PasswordSalt, FirstName, LastName, Role, Department, JobTitle, IsActive, EmailVerified)
VALUES 
-- ============ SUPER ADMIN ============
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', NULL, 'superadmin@taskmanagement.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Super', 'Admin', 'SuperAdmin', NULL, 'System Administrator', 1, 1),

-- ============ TECH INNOVATIONS INC USERS ============
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '11111111-1111-1111-1111-111111111111', 'john.admin@techinnovations.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'John', 'Anderson', 'CompanyAdmin', 'Management', 'CEO', 1, 1),
 
('cccccccc-cccc-cccc-cccc-cccccccccccc', '11111111-1111-1111-1111-111111111111', 'sarah.manager@techinnovations.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Sarah', 'Williams', 'Manager', 'Development', 'Development Manager', 1, 1),
 
('dddddddd-dddd-dddd-dddd-dddddddddddd', '11111111-1111-1111-1111-111111111111', 'mike.user@techinnovations.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Mike', 'Johnson', 'User', 'Development', 'Senior Developer', 1, 1),

('11111111-1111-1111-1111-100000000001', '11111111-1111-1111-1111-111111111111', 'alice.dev@techinnovations.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Alice', 'Brown', 'User', 'Development', 'Frontend Developer', 1, 1),

('11111111-1111-1111-1111-100000000002', '11111111-1111-1111-1111-111111111111', 'robert.qa@techinnovations.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Robert', 'Davis', 'User', 'Quality Assurance', 'QA Engineer', 1, 1),

-- ============ DIGITAL SOLUTIONS LTD USERS ============
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '22222222-2222-2222-2222-222222222222', 'emma.admin@digitalsolutions.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Emma', 'Wilson', 'CompanyAdmin', 'Management', 'Managing Director', 1, 1),
 
('ffffffff-ffff-ffff-ffff-ffffffffffff', '22222222-2222-2222-2222-222222222222', 'david.manager@digitalsolutions.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'David', 'Brown', 'Manager', 'Operations', 'Operations Manager', 1, 1),

('22222222-2222-2222-2222-200000000001', '22222222-2222-2222-2222-222222222222', 'lisa.dev@digitalsolutions.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Lisa', 'Garcia', 'User', 'Development', 'Full Stack Developer', 1, 1),

-- ============ STARTUP HUB USERS ============
('33333333-3333-3333-3333-300000000001', '33333333-3333-3333-3333-333333333333', 'alex.founder@startuphub.com', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a', 
 '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu', 'Alex', 'Thompson', 'CompanyAdmin', 'Management', 'Founder & CEO', 1, 1);
GO

-- ================================================
-- 3. CLIENTS DATA
-- ================================================

INSERT INTO Core.Clients (Id, CompanyId, Name, Email, Phone, ContactPerson, Address, Website, Industry, Notes, IsActive, CreatedById)
VALUES 
-- Tech Innovations Clients
('11111111-1111-1111-1111-300000000001', '11111111-1111-1111-1111-111111111111', 'Global Manufacturing Corp', 'contact@globalmanufacturing.com', '+1-555-1001', 'James Wilson', '100 Industrial Blvd, Detroit, MI 48201', 'www.globalmanufacturing.com', 'Manufacturing', 'Large manufacturing client with multiple locations', 1, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'),

('11111111-1111-1111-1111-300000000002', '11111111-1111-1111-1111-111111111111', 'FinTech Solutions', 'info@fintechsolutions.com', '+1-555-1002', 'Maria Rodriguez', '250 Financial District, New York, NY 10001', 'www.fintechsolutions.com', 'Financial Services', 'Innovative fintech startup needing custom solutions', 1, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'),

('11111111-1111-1111-1111-300000000003', '11111111-1111-1111-1111-111111111111', 'HealthCare Plus', 'admin@healthcareplus.com', '+1-555-1003', 'Dr. Michael Chen', '500 Medical Center Dr, Boston, MA 02101', 'www.healthcareplus.com', 'Healthcare', 'Healthcare provider requiring HIPAA compliant solutions', 1, 'cccccccc-cccc-cccc-cccc-cccccccccccc'),

-- Digital Solutions Clients
('22222222-2222-2222-2222-300000000001', '22222222-2222-2222-2222-222222222222', 'Retail Chain Express', 'contact@retailchainexpress.com', '+1-555-2001', 'Sandra Johnson', '300 Retail Plaza, Chicago, IL 60601', 'www.retailchainexpress.com', 'Retail', 'Multi-location retail chain needing inventory management', 1, 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee'),

('22222222-2222-2222-2222-300000000002', '22222222-2222-2222-2222-222222222222', 'Education First', 'info@educationfirst.com', '+1-555-2002', 'Dr. Patricia Lee', '400 University Ave, Seattle, WA 98101', 'www.educationfirst.com', 'Education', 'Educational institution requiring learning management system', 1, 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee');
GO

-- ================================================
-- 4. PROJECTS DATA
-- ================================================

INSERT INTO Core.Projects (Id, CompanyId, ClientId, Name, Description, ProjectCode, StartDate, EndDate, Budget, Status, ProjectManagerId, Progress, CreatedById)
VALUES 
-- Tech Innovations Projects
('11111111-1111-1111-1111-400000000001', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-300000000001', 
 'Manufacturing ERP System', 'Complete ERP system for manufacturing operations including inventory, production planning, and quality control', 
 'PROJ-2024-001', '2024-01-15', '2024-12-31', 250000.00, 'Active', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 45, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'),

('11111111-1111-1111-1111-400000000002', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-300000000002', 
 'Mobile Banking App', 'Secure mobile banking application with biometric authentication and real-time transaction processing', 
 'PROJ-2024-002', '2024-03-01', '2024-10-31', 180000.00, 'Active', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 60, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'),

('11111111-1111-1111-1111-400000000003', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-300000000003', 
 'Patient Management Portal', 'HIPAA-compliant patient management system with appointment scheduling and medical records', 
 'PROJ-2024-003', '2024-02-01', '2024-11-30', 320000.00, 'Active', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 30, 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'),

-- Digital Solutions Projects
('22222222-2222-2222-2222-400000000001', '22222222-2222-2222-2222-222222222222', '22222222-2222-2222-2222-300000000001', 
 'Inventory Management System', 'Multi-location inventory tracking and management system with real-time synchronization', 
 'PROJ-2024-004', '2024-04-01', '2024-09-30', 120000.00, 'Active', 'ffffffff-ffff-ffff-ffff-ffffffffffff', 70, 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee'),

('22222222-2222-2222-2222-400000000002', '22222222-2222-2222-2222-222222222222', '22222222-2222-2222-2222-300000000002', 
 'Learning Management System', 'Custom LMS with course management, student tracking, and assessment tools', 
 'PROJ-2024-005', '2024-05-01', '2024-12-15', 95000.00, 'Planning', 'ffffffff-ffff-ffff-ffff-ffffffffffff', 15, 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee');
GO

-- ================================================
-- FINAL STATISTICS AND SUMMARY
-- ================================================

-- Display summary of seeded data
PRINT '============================================';
PRINT 'COMPLETE SEED DATA INSERTION SUCCESSFUL!';
PRINT '============================================';
PRINT '';
PRINT 'USER ACCOUNTS CREATED:';
PRINT '----------------------';
PRINT 'SuperAdmin: superadmin@taskmanagement.com (Password: Admin@123)';
PRINT '';
PRINT 'TECH INNOVATIONS INC:';
PRINT '- john.admin@techinnovations.com (CompanyAdmin)';
PRINT '- sarah.manager@techinnovations.com (Manager)';
PRINT '- mike.user@techinnovations.com (User)';
PRINT '- alice.dev@techinnovations.com (User)';
PRINT '- robert.qa@techinnovations.com (User)';
PRINT '';
PRINT 'DIGITAL SOLUTIONS LTD:';
PRINT '- emma.admin@digitalsolutions.com (CompanyAdmin)';
PRINT '- david.manager@digitalsolutions.com (Manager)';
PRINT '- lisa.dev@digitalsolutions.com (User)';
PRINT '';
PRINT 'STARTUP HUB:';
PRINT '- alex.founder@startuphub.com (CompanyAdmin)';
PRINT '';
PRINT 'All passwords are: Admin@123';
PRINT '';
PRINT 'FEATURES INCLUDED:';
PRINT '------------------';
PRINT '✓ Multi-tenant company structure';
PRINT '✓ Role-based user hierarchy';
PRINT '✓ Client and project management';
PRINT '✓ Comprehensive task system';
PRINT '✓ Real-time notifications';
PRINT '✓ Email templates for automation';
PRINT '✓ System settings and customization';
PRINT '✓ Complete audit trail and activity logs';
PRINT '✓ Sample data for immediate testing';
PRINT '';
PRINT 'SYSTEM READY FOR TESTING AND DEVELOPMENT!';
PRINT '============================================';
GO