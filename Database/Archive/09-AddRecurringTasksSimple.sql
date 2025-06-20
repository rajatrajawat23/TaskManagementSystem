USE TaskManagementDB;
GO

-- Add a simple daily recurring task
INSERT INTO Core.Tasks (
    Id, CompanyId, Title, Description, TaskNumber, 
    AssignedToId, AssignedById, Priority, Status, 
    DueDate, CreatedById, IsRecurring, RecurrencePattern, StartDate
)
VALUES (
    NEWID(), 
    '11111111-1111-1111-1111-111111111111', 
    'Daily Standup Meeting', 
    'Daily team standup meeting at 9:00 AM', 
    'TSK-2024-0004', 
    'cccccccc-cccc-cccc-cccc-cccccccccccc', 
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 
    'Medium', 
    'Pending', 
    DATEADD(day, 1, CAST(GETDATE() AS DATE)), 
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 
    1, 
    '{"Type":"Daily","Interval":1}', 
    CAST(GETDATE() AS DATE)
);
GO

-- Add a weekly recurring task
INSERT INTO Core.Tasks (
    Id, CompanyId, Title, Description, TaskNumber, 
    AssignedToId, AssignedById, Priority, Status, 
    DueDate, CreatedById, IsRecurring, RecurrencePattern, StartDate
)
VALUES (
    NEWID(), 
    '11111111-1111-1111-1111-111111111111', 
    'Weekly Project Status Report', 
    'Prepare and submit weekly project status report', 
    'TSK-2024-0005', 
    'cccccccc-cccc-cccc-cccc-cccccccccccc', 
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 
    'High', 
    'Pending', 
    DATEADD(day, 7, CAST(GETDATE() AS DATE)), 
    'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 
    1, 
    '{"Type":"Weekly","Interval":1,"DaysOfWeek":[5]}', 
    CAST(GETDATE() AS DATE)
);
GO

-- Add a monthly recurring task
INSERT INTO Core.Tasks (
    Id, CompanyId, Title, Description, TaskNumber, 
    AssignedToId, AssignedById, Priority, Status, 
    DueDate, CreatedById, IsRecurring, RecurrencePattern, StartDate
)
VALUES (
    NEWID(), 
    '11111111-1111-1111-1111-111111111111', 
    'Monthly Security Audit', 
    'Perform monthly security audit and update documentation', 
    'TSK-2024-0006', 
    'dddddddd-dddd-dddd-dddd-dddddddddddd', 
    'cccccccc-cccc-cccc-cccc-cccccccccccc', 
    'Critical', 
    'Pending', 
    DATEADD(month, 1, CAST(GETDATE() AS DATE)), 
    'cccccccc-cccc-cccc-cccc-cccccccccccc', 
    1, 
    '{"Type":"Monthly","Interval":1,"DayOfMonth":15}', 
    CAST(GETDATE() AS DATE)
);
GO

PRINT 'Recurring tasks added successfully!';
GO
