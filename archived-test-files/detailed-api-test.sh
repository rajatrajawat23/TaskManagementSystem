#!/bin/bash

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

BASE_URL="http://localhost:5175/api"

echo -e "${BLUE}=== Detailed API Testing ===${NC}\n"

# Start API if not running
if ! curl -s http://localhost:5175/health > /dev/null; then
    echo "Starting API..."
    cd Backend/TaskManagement.API && dotnet run > ../../api-detailed.log 2>&1 &
    sleep 10
fi

# 1. Login first
echo -e "${YELLOW}1. Testing Login...${NC}"
LOGIN_RESPONSE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -d '{"email":"superadmin@taskmanagement.com","password":"Admin@123"}' \
    "$BASE_URL/auth/login")

echo "Login Response:"
echo "$LOGIN_RESPONSE" | jq '.'

TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.accessToken // empty')
if [ -z "$TOKEN" ]; then
    echo -e "${RED}Failed to get token!${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Login successful${NC}"
echo "Token: ${TOKEN:0:50}..."

# 2. Test Task Creation with detailed error
echo -e "\n${YELLOW}2. Testing Task Creation...${NC}"
echo "Creating task with minimal data..."

TASK_CREATE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d '{
        "title": "Test Task",
        "description": "Test Description",
        "priority": "High",
        "status": "Pending"
    }' \
    "$BASE_URL/task")

echo "Task Create Response:"
echo "$TASK_CREATE" | jq '.'

# 3. Test Company Creation
echo -e "\n${YELLOW}3. Testing Company Creation...${NC}"

COMPANY_CREATE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d '{
        "name": "New Test Company",
        "domain": "newtestco",
        "contactEmail": "admin@newtestco.com",
        "contactPhone": "+1234567890",
        "address": "123 Test Street",
        "subscriptionType": "Premium",
        "maxUsers": 50,
        "isActive": true
    }' \
    "$BASE_URL/company")

echo "Company Create Response:"
echo "$COMPANY_CREATE" | jq '.'

# 4. Test User Creation
echo -e "\n${YELLOW}4. Testing User Creation...${NC}"

# First get a valid company ID
COMPANIES=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/company")
COMPANY_ID=$(echo "$COMPANIES" | jq -r '.items[0].id // empty')
echo "Using Company ID: $COMPANY_ID"

USER_CREATE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{
        \"email\": \"testuser$(date +%s)@example.com\",
        \"password\": \"Test@123\",
        \"firstName\": \"Test\",
        \"lastName\": \"User\",
        \"role\": \"User\",
        \"companyId\": \"$COMPANY_ID\"
    }" \
    "$BASE_URL/user")

echo "User Create Response:"
echo "$USER_CREATE" | jq '.'

# 5. Test Client Creation
echo -e "\n${YELLOW}5. Testing Client Creation...${NC}"

CLIENT_CREATE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d '{
        "name": "Test Client",
        "email": "client@testclient.com",
        "phone": "+1234567890",
        "company": "Test Client Corp",
        "address": "456 Client Ave"
    }' \
    "$BASE_URL/client")

echo "Client Create Response:"
echo "$CLIENT_CREATE" | jq '.'

# 6. Test Project Creation
echo -e "\n${YELLOW}6. Testing Project Creation...${NC}"

PROJECT_CREATE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d '{
        "name": "Test Project",
        "description": "Test Project Description",
        "startDate": "2025-06-18T00:00:00Z",
        "endDate": "2025-12-31T00:00:00Z",
        "status": "Planning",
        "budget": 100000
    }' \
    "$BASE_URL/project")

echo "Project Create Response:"
echo "$PROJECT_CREATE" | jq '.'

# 7. Check Database Records
echo -e "\n${YELLOW}7. Checking Database Records...${NC}"

DB_CHECK=$(docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U doubler -P 'Ramjat@1998' -d TaskManagementDB -C \
    -Q "SELECT 'Companies' as TableName, COUNT(*) as Count FROM Companies WHERE IsDeleted = 0
         UNION ALL SELECT 'Users', COUNT(*) FROM Users WHERE IsDeleted = 0
         UNION ALL SELECT 'Tasks', COUNT(*) FROM Tasks WHERE IsDeleted = 0
         UNION ALL SELECT 'Projects', COUNT(*) FROM Projects WHERE IsDeleted = 0
         UNION ALL SELECT 'Clients', COUNT(*) FROM Clients WHERE IsDeleted = 0" \
    -h -1 2>/dev/null | grep -v "rows affected")

echo "Database Record Counts:"
echo "$DB_CHECK"

# 8. Check recent errors in API log
echo -e "\n${YELLOW}8. Recent API Errors:${NC}"
tail -n 50 api-detailed.log | grep -E "Error|Exception|fail" | tail -10

echo -e "\n${BLUE}=== Test Complete ===${NC}"
