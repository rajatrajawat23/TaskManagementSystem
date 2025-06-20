@echo off
echo ========================================
echo Task Management System - Database Setup
echo ========================================
echo.

REM Check if Docker is running
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Docker is not installed or not in PATH
    echo Please install Docker Desktop from https://www.docker.com/products/docker-desktop/
    pause
    exit /b 1
)

echo [1/6] Checking Docker service...
docker ps >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Docker is not running. Please start Docker Desktop.
    pause
    exit /b 1
)

echo [2/6] Checking if SQL Server container exists...
docker ps -a | findstr taskmanagement-sqlserver >nul 2>&1
if %errorlevel% equ 0 (
    echo Found existing container. Removing...
    docker stop taskmanagement-sqlserver >nul 2>&1
    docker rm taskmanagement-sqlserver >nul 2>&1
)

echo [3/6] Starting SQL Server in Docker...
cd Database\Docker
docker-compose up -d

echo [4/6] Waiting for SQL Server to start (60 seconds)...
echo Please wait...
timeout /t 60 /nobreak >nul

echo [5/6] Testing database connection...
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SecureTask2025#@!" -Q "SELECT 'Connection successful'" -C
if %errorlevel% neq 0 (
    echo ERROR: Could not connect to database
    echo Trying one more time after 30 seconds...
    timeout /t 30 /nobreak >nul
    docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SecureTask2025#@!" -Q "SELECT 'Connection successful'" -C
    if %errorlevel% neq 0 (
        echo ERROR: Database connection failed
        pause
        exit /b 1
    )
)

echo [6/6] Running database scripts...
echo Creating database...
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SecureTask2025#@!" -i /docker-entrypoint-initdb.d/../Scripts/01-CreateDatabase.sql -C

echo Creating tables...
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SecureTask2025#@!" -d TaskManagementDB -i /docker-entrypoint-initdb.d/../Scripts/02-CreateTables.sql -C

echo Creating indexes...
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SecureTask2025#@!" -d TaskManagementDB -i /docker-entrypoint-initdb.d/../Scripts/03-CreateIndexes.sql -C

echo Seeding data...
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SecureTask2025#@!" -d TaskManagementDB -i /docker-entrypoint-initdb.d/../Scripts/05-SeedData.sql -C

echo.
echo ========================================
echo Database setup completed successfully!
echo ========================================
echo.
echo Database Details:
echo - Server: localhost,1433
echo - Database: TaskManagementDB
echo - Username: sa
echo - Password: SecureTask2025#@!
echo.
echo You can now run the application:
echo   cd Backend\TaskManagement.API
echo   dotnet run
echo.
pause
