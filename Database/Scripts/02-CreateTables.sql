USE TaskManagementDB;
GO

-- 1. Companies Table (Multi-tenant foundation)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Companies' AND xtype='U')
CREATE TABLE Core.Companies
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Domain NVARCHAR(50) UNIQUE NOT NULL,
    ContactEmail NVARCHAR(100) NOT NULL,
    ContactPhone NVARCHAR(20),
    Address NVARCHAR(500),
    SubscriptionType NVARCHAR(20) DEFAULT 'Free' CHECK (SubscriptionType IN ('Free', 'Premium', 'Enterprise')),
    SubscriptionExpiryDate DATETIME2,
    MaxUsers INT DEFAULT 10,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER
);
GO

-- 2. Users Table (Authentication & Authorization)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
CREATE TABLE Security.Users
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER, -- NULL for SuperAdmin
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    PasswordSalt NVARCHAR(500) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    PhoneNumber NVARCHAR(20),
    ProfileImageUrl NVARCHAR(500),
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('SuperAdmin', 'CompanyAdmin', 'Manager', 'User', 'TaskAssigner')),
    Department NVARCHAR(50),
    JobTitle NVARCHAR(100),
    IsActive BIT DEFAULT 1,
    EmailVerified BIT DEFAULT 0,
    LastLoginAt DATETIME2,
    PasswordResetToken NVARCHAR(500),
    PasswordResetExpiry DATETIME2,
    RefreshToken NVARCHAR(500),
    RefreshTokenExpiry DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id)
);
GO

-- 3. Tasks Table (Core business entity)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tasks' AND xtype='U')
CREATE TABLE Core.Tasks
(
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    TaskNumber NVARCHAR(20) UNIQUE, -- Auto-generated: TSK-2024-0001
    AssignedToId UNIQUEIDENTIFIER,
    AssignedById UNIQUEIDENTIFIER NOT NULL,
    ClientId UNIQUEIDENTIFIER, -- Optional
    ProjectId UNIQUEIDENTIFIER, -- Optional
    Priority NVARCHAR(10) DEFAULT 'Medium' CHECK (Priority IN ('Low', 'Medium', 'High', 'Critical')),
    Status NVARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'InProgress', 'Review', 'Completed', 'Cancelled')),
    Category NVARCHAR(50),
    Tags NVARCHAR(500), -- JSON array of tags
    EstimatedHours DECIMAL(5,2),
    ActualHours DECIMAL(5,2),
    StartDate DATETIME2,
    DueDate DATETIME2,
    CompletedDate DATETIME2,
    IsRecurring BIT DEFAULT 0,
    RecurrencePattern NVARCHAR(100), -- JSON for recurrence rules
    ParentTaskId UNIQUEIDENTIFIER, -- For subtasks
    Progress INT DEFAULT 0 CHECK (Progress >= 0 AND Progress <= 100),
    IsArchived BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedById UNIQUEIDENTIFIER,
    UpdatedById UNIQUEIDENTIFIER,
    FOREIGN KEY (CompanyId) REFERENCES Core.Companies(Id),
    FOREIGN KEY (AssignedToId) REFERENCES Security.Users(Id),
    FOREIGN KEY (AssignedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (ParentTaskId) REFERENCES Core.Tasks(Id),
    FOREIGN KEY (CreatedById) REFERENCES Security.Users(Id),
    FOREIGN KEY (UpdatedById) REFERENCES Security.Users(Id)
);
GO