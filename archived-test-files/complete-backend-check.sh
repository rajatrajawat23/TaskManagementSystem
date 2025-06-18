#!/bin/bash

echo "================================================"
echo "Complete Backend Status Check"
echo "================================================"

cd /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API

echo -e "\n1. Building the backend..."
dotnet build 2>&1 | tail -20

echo -e "\n2. Checking Docker database status..."
cd ../../Database/Docker
docker ps | grep taskmanagement-sqlserver

echo -e "\n3. Controllers present:"
find /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Controllers -name "*.cs" | wc -l
echo "Controllers:"
ls /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Controllers/

echo -e "\n4. Services present:"
find /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Services/Implementation -name "*.cs" | wc -l
echo "Services:"
ls /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Services/Implementation/

echo -e "\n5. Entities present:"
find /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.Core/Entities -name "*.cs" | wc -l
echo "Entities:"
ls /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.Core/Entities/

echo -e "\n6. Checking if API is running..."
lsof -i :5175 > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "API is running on port 5175"
    echo -e "\n7. Testing API endpoints..."
    curl -s http://localhost:5175/health | head -20
else
    echo "API is NOT running"
fi

echo -e "\n================================================"
echo "Summary of Issues Found:"
echo "================================================"

# Check for build errors
cd /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API
BUILD_OUTPUT=$(dotnet build 2>&1)
if echo "$BUILD_OUTPUT" | grep -q "Build failed"; then
    echo "❌ Build is failing"
    echo "$BUILD_OUTPUT" | grep "error" | head -10
else
    echo "✅ Build is successful"
fi

# Check for missing services
if [ ! -f "Services/Implementation/DashboardService.cs" ]; then
    echo "❌ DashboardService implementation is missing"
else
    echo "✅ DashboardService exists"
fi

# Check database
docker ps | grep taskmanagement-sqlserver > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ Database container is running"
else
    echo "❌ Database container is NOT running"
fi

echo -e "\n================================================"