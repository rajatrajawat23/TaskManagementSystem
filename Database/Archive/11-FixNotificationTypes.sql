USE TaskManagementDB;
GO

-- First, drop the existing NotificationType CHECK constraint
DECLARE @NotifTypeConstraintName NVARCHAR(128)
SELECT @NotifTypeConstraintName = name 
FROM sys.check_constraints 
WHERE parent_object_id = OBJECT_ID('Communication.Notifications') 
AND COL_NAME(parent_object_id, parent_column_id) = 'NotificationType'

IF @NotifTypeConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Communication.Notifications DROP CONSTRAINT ' + @NotifTypeConstraintName)
    PRINT 'Dropped existing NotificationType constraint'
END
GO

-- Drop the existing Priority CHECK constraint
DECLARE @PriorityConstraintName NVARCHAR(128)
SELECT @PriorityConstraintName = name 
FROM sys.check_constraints 
WHERE parent_object_id = OBJECT_ID('Communication.Notifications') 
AND COL_NAME(parent_object_id, parent_column_id) = 'Priority'

IF @PriorityConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Communication.Notifications DROP CONSTRAINT ' + @PriorityConstraintName)
    PRINT 'Dropped existing Priority constraint'
END
GO

-- Add the updated CHECK constraint with all notification types used by the service
ALTER TABLE Communication.Notifications 
ADD CONSTRAINT CK_Notifications_NotificationType 
CHECK (NotificationType IN (
    'TaskAssigned',
    'TaskAssignment',
    'TaskUpdated', 
    'TaskStatusUpdate',
    'TaskCompleted',
    'TaskOverdue',
    'TaskReminder',
    'ChatMessage',
    'SystemAlert'
))
GO

-- Add the updated Priority constraint to include 'Medium'
ALTER TABLE Communication.Notifications 
ADD CONSTRAINT CK_Notifications_Priority 
CHECK (Priority IN (
    'Low',
    'Normal',
    'Medium',
    'High',
    'Critical'
))
GO

PRINT 'Updated NotificationType and Priority constraints successfully!'
GO
