# Task Management System - Windows Setup Guide

## 📋 Prerequisites

Before setting up the project, ensure you have the following installed on your Windows machine:

1. **Visual Studio 2022** (Community Edition or higher)
   - Download from: https://visualstudio.microsoft.com/
   - During installation, select:
     - ASP.NET and web development workload
     - .NET 8.0 SDK

2. **SQL Server 2022** (Developer Edition is free)
   - Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
   - During installation:
     - Choose "Basic" installation
     - Remember the SA password you set
     - Enable TCP/IP in SQL Server Configuration Manager

3. **SQL Server Management Studio (SSMS)**
   - Download from: https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms

4. **Git for Windows**
   - Download from: https://git-scm.com/download/win

5. **Postman** (for API testing)
   - Download from: https://www.postman.com/downloads/

## 🚀 Quick Setup Steps

### Step 1: Clone the Repository

```bash
# Open Command Prompt or PowerShell
git clone [your-repository-url]
cd TaskManagementSystem
```

### Step 2: Setup Database

#### Option A: Using Provided Scripts (Recommended)

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your local SQL Server instance
3. Run the database setup scripts in order:

```sql
-- First, run these scripts from Database/Scripts folder:
-- 1. Run 01-CreateDatabase.sql
-- 2. Run 02-CreateTables.sql
-- 3. Run 03-CreateIndexes.sql
-- 4. Run 05-SeedData.sql (for sample data)
```

#### Option B: Using Batch File

Double-click the `setup-database-windows.bat` file in the root directory, or run:

```cmd
setup-database-windows.bat
```

### Step 3: Configure Connection String

1. Navigate to `Backend/TaskManagement.API/`
2. Open `appsettings.json`
3. Update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
  }
}
```

Replace `YourPassword` with your SQL Server SA password.

### Step 4: Build and Run the Application

#### Using Visual Studio:
1. Open `Backend/TaskManagement.sln` in Visual Studio 2022
2. Right-click on the solution and select "Restore NuGet Packages"
3. Set `TaskManagement.API` as the startup project
4. Press F5 to run

#### Using Command Line:
```cmd
cd Backend
dotnet restore
dotnet build
cd TaskManagement.API
dotnet run
```

### Step 5: Verify Setup

1. The API should start on `https://localhost:7237` or `http://localhost:5237`
2. Navigate to `https://localhost:7237/swagger` to see the API documentation
3. Test the connection using the provided batch file:

```cmd
test-connection-windows.bat
```

## 🗄️ Database Setup Details

### Database Structure
The system creates the following:
- **Database Name**: TaskManagementDB
- **Tables**: 11 main tables including Users, Tasks, Companies, etc.
- **Sample Data**: Pre-populated with test companies and users

### Default Test Users
After running seed scripts, you can login with:

```
SuperAdmin:
Email: superadmin@taskmanagement.com
Password: SuperAdmin123!

Company Admin:
Email: admin@techcorp.com
Password: Admin123!

Manager:
Email: manager@techcorp.com
Password: Manager123!

User:
Email: user@techcorp.com
Password: User123!
```

## 🔧 Troubleshooting

### Common Issues and Solutions

1. **SQL Server Connection Failed**
   - Ensure SQL Server service is running
   - Check if TCP/IP is enabled in SQL Configuration Manager
   - Verify the server name in connection string (use "localhost" or ".\SQLEXPRESS")

2. **Database Not Found**
   - Run the create database script first
   - Check if you have permissions to create databases

3. **Port Already in Use**
   - Change the port in `launchSettings.json`
   - Kill the process using the port

4. **NuGet Package Restore Failed**
   - Clear NuGet cache: `dotnet nuget locals all --clear`
   - Restart Visual Studio

## 📱 Testing the API

### Using Postman Collection
1. Import `Documentation/API/Postman-Collection.json` into Postman
2. Set the environment variable `baseUrl` to `https://localhost:7237`
3. Run the collection tests

### Manual Testing
1. **Register a new user**:
   ```
   POST https://localhost:7237/api/auth/register
   ```

2. **Login**:
   ```
   POST https://localhost:7237/api/auth/login
   ```

3. **Create a task**:
   ```
   POST https://localhost:7237/api/task
   ```

## 📊 Project Features

- ✅ Multi-tenant architecture
- ✅ JWT Authentication with refresh tokens
- ✅ Role-based authorization (SuperAdmin, CompanyAdmin, Manager, User)
- ✅ Complete CRUD operations for all entities
- ✅ Real-time notifications
- ✅ Task management with assignments
- ✅ Company-wise data isolation
- ✅ Comprehensive error handling
- ✅ API documentation with Swagger

## 📝 Additional Notes

- The project uses .NET 8.0
- Entity Framework Core for database operations
- Clean Architecture pattern
- Repository and Unit of Work patterns
- AutoMapper for object mapping
- FluentValidation for input validation

## 🆘 Need Help?

If you encounter any issues:
1. Check the logs in `Backend/TaskManagement.API/Logs/`
2. Ensure all prerequisites are installed
3. Verify database connection settings
4. Check if all required ports are available

---

**Note**: This project was developed on macOS and tested for Windows compatibility. All database scripts and batch files are included for easy Windows setup.
