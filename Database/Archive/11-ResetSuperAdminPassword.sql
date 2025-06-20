SET QUOTED_IDENTIFIER ON;
GO

USE TaskManagementDB;
GO

-- Reset SuperAdmin password to 'Admin@123' using BCrypt hash
UPDATE Security.Users
SET PasswordHash = '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu3WKDdPt4RgAF5TmDtWGQmvOj5M3h86a',
    PasswordSalt = '$2a$11$HDKQ7Z/ns6fTZ0MqpU4YYu'
WHERE Email = 'superadmin@taskmanagement.com';

PRINT 'SuperAdmin password reset to Admin@123';
GO