# Complete Setup Guide - Task Management System

## 🎯 Overview
This guide provides two ways to set up the database for the Task Management System on Windows:
1. **Option A**: Using Docker (Recommended for consistency)
2. **Option B**: Using Local SQL Server (If SQL Server is already installed)

---

## 📋 Prerequisites

### For Both Options:
- **Visual Studio 2022** with .NET 8.0 SDK
- **Git for Windows**
- **Postman** (for API testing)

### For Option A (Docker):
- **Docker Desktop for Windows**
  - Download: https://www.docker.com/products/docker-desktop/

### For Option B (Local SQL Server):
- **SQL Server 2022** (Any edition)
  - Download: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- **SQL Server Management Studio (SSMS)**
  - Download: https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms

---

## 🚀 Setup Instructions

### Step 1: Clone the Repository
```bash
git clone [repository-url]
cd TaskManagementSystem
```

### Step 2: Choose Your Database Setup Method

## Option A: Docker Setup (Recommended)

### Why Docker?
- Consistent environment across all machines
- No SQL Server installation required
- Isolated from your system
- Easy to reset/recreate

### Steps:
1. **Ensure Docker Desktop is running**

2. **Run the setup script:**
   ```cmd
   setup-database-windows.bat
   ```

3. **Update connection string in `Backend/TaskManagement.API/appsettings.json`:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=TaskManagementDB;User Id=sa;Password=SecureTask2025#@!;TrustServerCertificate=true;"
     }
   }
   ```

## Option B: Local SQL Server Setup

### Steps:
1. **Ensure SQL Server is running**
   - Open Services (services.msc)
   - Find "SQL Server (MSSQLSERVER)" or "SQL Server (SQLEXPRESS)"
   - Ensure it's running

2. **Run the local setup script:**
   ```cmd
   setup-database-local-windows.bat
   ```
   
   The script will prompt you for:
   - Server name (e.g., `localhost` or `.\SQLEXPRESS`)
   - Authentication type (Windows or SQL)
   - Username/Password (if using SQL authentication)

3. **Manual Setup Alternative (Using SSMS):**
   If the script doesn't work, open SSMS and run these scripts in order:
   - `Database/Scripts/01-CreateDatabase.sql`
   - `Database/Scripts/02-CreateTables.sql`
   - `Database/Scripts/03-CreateIndexes.sql`
   - `Database/Scripts/05-SeedData.sql`

4. **Update connection string based on your setup:**

   For Windows Authentication:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TaskManagementDB;Trusted_Connection=True;TrustServerCertificate=true;"
     }
   }
   ```

   For SQL Authentication:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TaskManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
     }
   }
   ```

---

## 🏃‍♂️ Running the Application

### Using Visual Studio:
1. Open `Backend/TaskManagement.sln`
2. Right-click solution → "Restore NuGet Packages"
3. Set `TaskManagement.API` as startup project
4. Press F5

### Using Command Line:
```cmd
cd Backend
dotnet restore
dotnet build
cd TaskManagement.API
dotnet run
```

### Verify Setup:
- API URL: `https://localhost:7237` or `http://localhost:5237`
- Swagger UI: `https://localhost:7237/swagger`

---

## 🔐 Default Test Credentials

After running the seed scripts, use these credentials:

| Role | Email | Password |
|------|-------|----------|
| SuperAdmin | superadmin@taskmanagement.com | SuperAdmin123! |
| Company Admin | admin@techcorp.com | Admin123! |
| Manager | manager@techcorp.com | Manager123! |
| User | user@techcorp.com | User123! |

---

## 🔧 Troubleshooting

### Docker Issues:
- **"Docker is not running"**: Start Docker Desktop
- **"Container already exists"**: The script will handle this automatically
- **"Cannot connect to database"**: Wait for SQL Server to fully start (60 seconds)

### Local SQL Server Issues:
- **"Cannot connect to SQL Server"**: 
  - Check if SQL Server service is running
  - Try different server names: `localhost`, `.\SQLEXPRESS`, `(local)`
  - Enable TCP/IP in SQL Server Configuration Manager
  
- **"Login failed"**:
  - For Windows Auth: Run command prompt as administrator
  - For SQL Auth: Ensure SQL authentication is enabled in server properties

### Application Issues:
- **"Connection string error"**: Double-check the connection string format
- **"Port already in use"**: Change port in `launchSettings.json`
- **"Package restore failed"**: Clear cache with `dotnet nuget locals all --clear`

---

## 📁 Project Structure

```
TaskManagementSystem/
├── Backend/                    # .NET Core application
│   ├── TaskManagement.API/     # Web API project
│   ├── TaskManagement.Core/    # Core entities and interfaces
│   └── TaskManagement.Infrastructure/ # Data access layer
├── Database/                   # Database files
│   ├── Docker/                 # Docker configuration
│   └── Scripts/                # SQL scripts
├── Documentation/              # Project documentation
└── Tests/                      # Test files and scripts
```

---

## 🤝 Sharing Your Work

When pushing to Git:
1. The `.gitignore` file excludes sensitive files
2. Connection strings in `appsettings.json` use default values
3. Database scripts are included for easy setup
4. Both Docker and local SQL Server options are documented

Your professor can:
1. Clone the repository
2. Choose their preferred database setup method
3. Run the application following this guide

---

## 📞 Support

If issues persist:
1. Check logs in `Backend/TaskManagement.API/Logs/`
2. Verify all prerequisites are installed
3. Ensure firewall isn't blocking ports
4. Try both Docker and local SQL Server options

---

**Note**: This project demonstrates enterprise-level architecture with multi-tenant support, JWT authentication, and clean architecture principles.
