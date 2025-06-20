# Task Management System - Windows Setup Guide

## 🚀 Quick Setup for Windows

This guide will help you set up the Task Management System on a Windows computer from scratch.

## 📋 Prerequisites

Please install the following software:

1. **Docker Desktop for Windows** (Recommended)
   - Download from: https://www.docker.com/products/docker-desktop/
   - This will handle SQL Server database automatically

2. **.NET 8.0 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Choose the Windows x64 installer

3. **Visual Studio 2022** (Optional but recommended)
   - Download Community Edition (free): https://visualstudio.microsoft.com/downloads/
   - Select "ASP.NET and web development" workload during installation

4. **Git for Windows**
   - Download from: https://git-scm.com/download/win

## 🛠️ Setup Instructions

### Option 1: Using Docker (Recommended - Easiest Method)

1. **Clone the Repository**
   ```cmd
   git clone <repository-url>
   cd TaskManagementSystem
   ```

2. **Start Database with Docker**
   ```cmd
   cd Database\Docker
   docker-compose up -d
   ```

3. **Wait for Database to Start** (about 30-60 seconds)
   ```cmd
   docker ps
   ```
   You should see `taskmanagement-sqlserver` container running.

4. **Run Database Setup Script**
   ```cmd
   cd ..\..\
   setup-database-windows.bat
   ```

5. **Run the Application**
   ```cmd
   cd Backend\TaskManagement.API
   dotnet restore
   dotnet build
   dotnet run
   ```

6. **Access the Application**
   - API Documentation: http://localhost:5194/swagger
   - Use the test credentials from README.md

### Option 2: Using SQL Server Express (Without Docker)

If Docker is not available, you can use SQL Server Express:

1. **Download SQL Server Express 2022**
   - Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
   - Choose "Express" edition (free)
   - During installation:
     - Choose "Mixed Mode Authentication"
     - Set SA password to: `SecureTask2025#@!`
     - Note the instance name (usually `SQLEXPRESS`)

2. **Clone the Repository**
   ```cmd
   git clone <repository-url>
   cd TaskManagementSystem
   ```

3. **Update Connection String**
   Open `Backend\TaskManagement.API\appsettings.json` and update:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TaskManagementDB;User Id=sa;Password=SecureTask2025#@!;TrustServerCertificate=true;MultipleActiveResultSets=true"
   }
   ```

4. **Create Database Using SSMS**
   - Open SQL Server Management Studio
   - Connect to `.\SQLEXPRESS` with SA credentials
   - Execute scripts from `Database\Scripts\` folder in order:
     - 01-CreateDatabase.sql
     - 02-CreateTables.sql
     - 03-CreateIndexes.sql
     - 05-SeedData.sql

5. **Run the Application**
   ```cmd
   cd Backend\TaskManagement.API
   dotnet restore
   dotnet build
   dotnet run
   ```

## 🔧 Troubleshooting

### Common Issues and Solutions

1. **Port 1433 Already in Use**
   - Open `Database\Docker\docker-compose.yml`
   - Change `"1433:1433"` to `"1434:1433"`
   - Update connection string port to `1434`

2. **Docker Not Starting**
   - Ensure Hyper-V is enabled in Windows Features
   - Restart Docker Desktop
   - Try running as Administrator

3. **Database Connection Failed**
   - Wait 60 seconds after starting Docker
   - Check if SQL Server container is running: `docker ps`
   - Try connection test: `test-connection-windows.bat`

4. **Build Errors**
   - Ensure .NET 8.0 SDK is installed: `dotnet --version`
   - Clear NuGet cache: `dotnet nuget locals all --clear`
   - Restore packages: `dotnet restore`

## 📁 Important Files to Check

1. **Connection Configuration**
   - `Backend\TaskManagement.API\appsettings.json` - Main configuration
   - `Database\Docker\.env` - Docker environment variables

2. **Database Scripts**
   - `Database\Scripts\` - All SQL scripts for manual setup

3. **Test Scripts**
   - `Tests\test-windows.bat` - Automated API testing

## 🧪 Testing the Setup

Once the application is running, test with:

1. **Open Swagger UI**: http://localhost:5194/swagger

2. **Test Login**:
   - Click on `/api/auth/login`
   - Try it out
   - Use credentials:
     ```json
     {
       "email": "superadmin@taskmanagement.com",
       "password": "Admin@123"
     }
     ```

3. **Test Other Endpoints**:
   - Copy the token from login response
   - Click "Authorize" button in Swagger
   - Enter: `Bearer <your-token>`
   - Now you can test other endpoints

## 📝 Quick Commands Reference

```cmd
# Start database
cd Database\Docker && docker-compose up -d

# Stop database
cd Database\Docker && docker-compose down

# View logs
docker logs taskmanagement-sqlserver

# Run application
cd Backend\TaskManagement.API && dotnet run

# Run tests
cd Tests && test-windows.bat
```

## 🆘 Need Help?

If you encounter any issues:
1. Check the error message carefully
2. Look at the Troubleshooting section above
3. Check if all prerequisites are installed
4. Ensure ports 1433 and 5194 are not in use by other applications

---

**Note**: Default passwords are set for development. Change them for production use!
