@echo off
echo ========================================
echo Testing Database Connection
echo ========================================
echo.

REM Test Docker
echo Checking Docker...
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [X] Docker is not installed
    goto :dockerfailed
) else (
    echo [✓] Docker is installed
)

REM Test Docker running
docker ps >nul 2>&1
if %errorlevel% neq 0 (
    echo [X] Docker is not running
    goto :dockerfailed
) else (
    echo [✓] Docker is running
)

REM Test SQL Server container
docker ps | findstr taskmanagement-sqlserver >nul 2>&1
if %errorlevel% neq 0 (
    echo [X] SQL Server container is not running
    echo.
    echo Starting SQL Server container...
    cd Database\Docker
    docker-compose up -d
    cd ..\..
    echo Waiting 30 seconds for startup...
    timeout /t 30 /nobreak >nul
) else (
    echo [✓] SQL Server container is running
)

REM Test database connection
echo.
echo Testing database connection...
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SecureTask2025#@!" -Q "SELECT 'Connection Test: SUCCESS'" -C
if %errorlevel% neq 0 (
    echo [X] Database connection failed
    goto :end
)

REM Test database exists
echo.
echo Checking if TaskManagementDB exists...
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SecureTask2025#@!" -Q "SELECT name FROM sys.databases WHERE name = 'TaskManagementDB'" -C
if %errorlevel% neq 0 (
    echo [X] Could not check database
    goto :end
)

REM Test tables exist
echo.
echo Checking tables...
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SecureTask2025#@!" -d TaskManagementDB -Q "SELECT COUNT(*) as TableCount FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -C

echo.
echo ========================================
echo All connection tests completed!
echo ========================================
goto :end

:dockerfailed
echo.
echo Docker is required but not found or not running.
echo Please install Docker Desktop from:
echo https://www.docker.com/products/docker-desktop/
echo.

:end
pause
