USE TaskManagementDB;

-- Create a test company first
INSERT INTO [Core].[Companies] (Id, Name, Domain, ContactEmail, ContactPhone, SubscriptionType, MaxUsers, IsActive, CreatedAt, UpdatedAt)
VALUES (
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'Test Company',
    'testcompany',
    'test@testcompany.com',
    '+1234567890',
    'Premium',
    100,
    1,
    GETUTCDATE(),
    GETUTCDATE()
);

-- Create a test user with hashed password (Test123!)
-- BCrypt hash for "Test123!" is: $2a$11$8pF4p3VkKe4.ZGmYgKwpYeE8rFCJ4b3q5K4F7DhPxoY4N6Q3YgXQW
INSERT INTO [Security].[Users] (Id, CompanyId, Email, PasswordHash, PasswordSalt, FirstName, LastName, Role, IsActive, EmailVerified, CreatedAt, UpdatedAt)
VALUES (
    'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB',
    'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA',
    'test@testcompany.com',
    '$2a$11$8pF4p3VkKe4.ZGmYgKwpYeE8rFCJ4b3q5K4F7DhPxoY4N6Q3YgXQW',
    'salt123',
    'Test',
    'User',
    'CompanyAdmin',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE()
);

SELECT 'User created successfully' as Result;