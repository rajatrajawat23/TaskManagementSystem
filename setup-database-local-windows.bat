@echo off
echo =============================================
echo Task Management System - Local SQL Server Setup
echo =============================================
echo.
echo This script will set up the database on your local SQL Server instance.
echo Make sure SQL Server is installed and running.
echo.

:: Check if sqlcmd is available
where sqlcmd >nul 2>nul
if %errorlevel% neq 0 (
    echo ERROR: sqlcmd is not found in PATH.
    echo Please ensure SQL Server command line tools are installed.
    echo Download from: https://learn.microsoft.com/en-us/sql/tools/sqlcmd-utility
    echo.
    echo Alternatively, you can run the scripts manually in SSMS.
    pause
    exit /b 1
)

:: Prompt for SQL Server details
set /p SERVER="Enter SQL Server instance (default: localhost): "
if "%SERVER%"=="" set SERVER=localhost

set /p AUTH="Use Windows Authentication? (Y/N, default: Y): "
if /i "%AUTH%"=="" set AUTH=Y
if /i "%AUTH%"=="Y" (
    set AUTH_MODE=-E
    set CONNECTION=%SERVER% -E
) else (
    set /p USERNAME="Enter SQL username (default: sa): "
    if "%USERNAME%"=="" set USERNAME=sa
    
    set /p PASSWORD="Enter SQL password: "
    if "%PASSWORD%"=="" (
        echo ERROR: Password cannot be empty for SQL authentication.
        pause
        exit /b 1
    )
    set CONNECTION=%SERVER% -U %USERNAME% -P %PASSWORD%
)

echo.
echo Connecting to SQL Server...
sqlcmd -S %CONNECTION% -Q "SELECT @@VERSION" -C >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Cannot connect to SQL Server.
    echo Please check your connection details.
    pause
    exit /b 1
)

echo Connection successful!
echo.

:: Execute scripts in order
echo [1/4] Creating database...
sqlcmd -S %CONNECTION% -i "Database\Scripts\01-CreateDatabase.sql" -C
if %errorlevel% neq 0 (
    echo ERROR: Failed to create database.
    pause
    exit /b 1
)

echo [2/4] Creating tables...
sqlcmd -S %CONNECTION% -d TaskManagementDB -i "Database\Scripts\02-CreateTables.sql" -C
if %errorlevel% neq 0 (
    echo ERROR: Failed to create tables.
    pause
    exit /b 1
)

echo [3/4] Creating indexes...
sqlcmd -S %CONNECTION% -d TaskManagementDB -i "Database\Scripts\03-CreateIndexes.sql" -C
if %errorlevel% neq 0 (
    echo WARNING: Some indexes might have failed. Continuing...
)

echo [4/4] Seeding sample data...
sqlcmd -S %CONNECTION% -d TaskManagementDB -i "Database\Scripts\05-SeedData.sql" -C
if %errorlevel% neq 0 (
    echo WARNING: Some seed data might have failed. Continuing...
)

echo.
echo =============================================
echo Database setup completed successfully!
echo =============================================
echo.
echo Database Name: TaskManagementDB
echo.
echo Next steps:
echo 1. Update your connection string in:
echo    Backend\TaskManagement.API\appsettings.json
echo.
echo 2. Example connection string:
if /i "%AUTH%"=="Y" (
    echo    "Server=%SERVER%;Database=TaskManagementDB;Trusted_Connection=True;TrustServerCertificate=true;"
) else (
    echo    "Server=%SERVER%;Database=TaskManagementDB;User Id=%USERNAME%;Password=YOUR_PASSWORD;TrustServerCertificate=true;"
)
echo.
echo 3. Run the application:
echo    cd Backend\TaskManagement.API
echo    dotnet run
echo.
pause
