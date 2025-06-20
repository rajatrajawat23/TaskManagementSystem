SET QUOTED_IDENTIFIER ON;
GO

USE TaskManagementDB;
GO

-- Add recurring tasks to demonstrate the recurring task feature
INSERT INTO Core.Tasks (Id, CompanyId, Title, Description, TaskNumber, AssignedToId, AssignedById, ProjectId, Priority, Status, DueDate, CreatedById, IsRecurring, RecurrencePattern, StartDate)
VALUES 
-- Daily standup meeting (recurring daily)
('11111111-1111-1111-1111-200000000004', '11111111-1111-1111-1111-111111111111', 
 'Daily Standup Meeting', 'Daily team standup meeting at 9:00 AM', 'TSK-2024-0004', 
 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 
 '11111111-1111-1111-1111-100000000001', 'Medium', 'Pending', 
 DATEADD(day, 1, CAST(GETDATE() AS DATE)), 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 
 1, '{"Type":"Daily","Interval":1}', CAST(GETDATE() AS DATE)),

-- Weekly project status report (recurring weekly)
('11111111-1111-1111-1111-200000000005', '11111111-1111-1111-1111-111111111111', 
 'Weekly Project Status Report', 'Prepare and submit weekly project status report', 'TSK-2024-0005', 
 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 
 '11111111-1111-1111-1111-100000000001', 'High', 'Pending', 
 DATEADD(day, 7, CAST(GETDATE() AS DATE)), 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 
 1, '{"Type":"Weekly","Interval":1,"DaysOfWeek":[5]}', CAST(GETDATE() AS DATE)),

-- Monthly security audit (recurring monthly)
('11111111-1111-1111-1111-200000000006', '11111111-1111-1111-1111-111111111111', 
 'Monthly Security Audit', 'Perform monthly security audit and update documentation', 'TSK-2024-0006', 
 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 
 NULL, 'Critical', 'Pending', 
 DATEADD(month, 1, CAST(GETDATE() AS DATE)), 'cccccccc-cccc-cccc-cccc-cccccccccccc', 
 1, '{"Type":"Monthly","Interval":1,"DayOfMonth":15}', CAST(GETDATE() AS DATE)),

-- Bi-weekly code review (recurring every 2 weeks)
('11111111-1111-1111-1111-200000000007', '11111111-1111-1111-1111-111111111111', 
 'Code Review Session', 'Bi-weekly code review and refactoring session', 'TSK-2024-0007', 
 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 
 '11111111-1111-1111-1111-100000000001', 'Medium', 'Pending', 
 DATEADD(day, 14, CAST(GETDATE() AS DATE)), 'cccccccc-cccc-cccc-cccc-cccccccccccc', 
 1, '{"Type":"Weekly","Interval":2}', CAST(GETDATE() AS DATE)),

-- Quarterly performance review (recurring every 3 months)
('11111111-1111-1111-1111-200000000008', '11111111-1111-1111-1111-111111111111', 
 'Quarterly Performance Review', 'Team performance review and goal setting', 'TSK-2024-0008', 
 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 
 NULL, 'High', 'Pending', 
 DATEADD(month, 3, CAST(GETDATE() AS DATE)), 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 
 1, '{"Type":"Monthly","Interval":3}', CAST(GETDATE() AS DATE)),

-- Daily backup task (recurring daily with end date)
('11111111-1111-1111-1111-200000000009', '11111111-1111-1111-1111-111111111111', 
 'Daily Backup Verification', 'Verify daily backup completion and integrity', 'TSK-2024-0009', 
 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 
 NULL, 'High', 'Pending', 
 DATEADD(day, 1, CAST(GETDATE() AS DATE)), 'cccccccc-cccc-cccc-cccc-cccccccccccc', 
 1, '{"Type":"Daily","Interval":1,"EndDate":"2025-12-31T00:00:00"}', CAST(GETDATE() AS DATE));

GO

-- Add some recurring tasks for Digital Solutions Ltd as well
INSERT INTO Core.Tasks (Id, CompanyId, Title, Description, TaskNumber, AssignedToId, AssignedById, Priority, Status, DueDate, CreatedById, IsRecurring, RecurrencePattern, StartDate)
VALUES 
-- Weekly team meeting
('22222222-2222-2222-2222-200000000001', '22222222-2222-2222-2222-222222222222', 
 'Weekly Team Meeting', 'Weekly team sync-up meeting', 'TSK-2024-0010', 
 'ffffffff-ffff-ffff-ffff-ffffffffffff', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 
 'Medium', 'Pending', 
 DATEADD(day, 7, CAST(GETDATE() AS DATE)), 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 
 1, '{"Type":"Weekly","Interval":1,"DaysOfWeek":[1]}', CAST(GETDATE() AS DATE));

GO

PRINT 'Recurring tasks added successfully!';
PRINT '';
PRINT 'Recurring tasks created:';
PRINT '- Daily Standup Meeting (Daily)';
PRINT '- Weekly Project Status Report (Weekly on Fridays)';
PRINT '- Monthly Security Audit (Monthly on 15th)';
PRINT '- Code Review Session (Bi-weekly)';
PRINT '- Quarterly Performance Review (Every 3 months)';
PRINT '- Daily Backup Verification (Daily until end of 2025)';
PRINT '- Weekly Team Meeting (Weekly on Mondays)';
GO
