#!/bin/bash

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

BASE_URL="http://localhost:5175/api"

echo -e "${BLUE}=== Testing All Create Operations ===${NC}\n"

# 1. Login first
echo -e "${YELLOW}1. Login as SuperAdmin...${NC}"
LOGIN_RESPONSE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -d '{"email":"superadmin@taskmanagement.com","password":"Admin@123"}' \
    "$BASE_URL/auth/login")

TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.accessToken // empty')
if [ -z "$TOKEN" ]; then
    echo -e "${RED}Failed to login!${NC}"
    exit 1
fi
echo -e "${GREEN}✅ Login successful${NC}"

# 2. Get existing company ID
echo -e "\n${YELLOW}2. Getting existing company...${NC}"
COMPANIES=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/company")
COMPANY_ID=$(echo "$COMPANIES" | jq -r '.items[0].id // empty')
echo "Found Company ID: $COMPANY_ID"

# 3. Create a new company with admin user
echo -e "\n${YELLOW}3. Creating new company with admin...${NC}"
TIMESTAMP=$(date +%s)
COMPANY_CREATE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{
        \"name\": \"Test Company $TIMESTAMP\",
        \"domain\": \"testco$TIMESTAMP\",
        \"contactEmail\": \"contact@testco$TIMESTAMP.com\",
        \"contactPhone\": \"+1234567890\",
        \"address\": \"123 Test Street\",
        \"subscriptionType\": \"Premium\",
        \"maxUsers\": 50,
        \"isActive\": true,
        \"adminEmail\": \"admin@testco$TIMESTAMP.com\",
        \"adminPassword\": \"Admin@123\",
        \"adminFirstName\": \"Admin\",
        \"adminLastName\": \"User\"
    }" \
    "$BASE_URL/company")

echo "$COMPANY_CREATE" | jq '.'
NEW_COMPANY_ID=$(echo "$COMPANY_CREATE" | jq -r '.id // empty')

if [ ! -z "$NEW_COMPANY_ID" ]; then
    echo -e "${GREEN}✅ Company created successfully - ID: $NEW_COMPANY_ID${NC}"
else
    echo -e "${RED}❌ Failed to create company${NC}"
fi

# 4. Create a user
echo -e "\n${YELLOW}4. Creating new user...${NC}"
USER_CREATE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{
        \"email\": \"testuser$TIMESTAMP@example.com\",
        \"password\": \"Test@123\",
        \"firstName\": \"Test\",
        \"lastName\": \"User\",
        \"role\": \"User\",
        \"phoneNumber\": \"+1234567890\",
        \"department\": \"IT\",
        \"jobTitle\": \"Developer\",
        \"isActive\": true
    }" \
    "$BASE_URL/user")

echo "$USER_CREATE" | jq '.'
USER_ID=$(echo "$USER_CREATE" | jq -r '.id // empty')

if [ ! -z "$USER_ID" ]; then
    echo -e "${GREEN}✅ User created successfully - ID: $USER_ID${NC}"
else
    echo -e "${RED}❌ Failed to create user${NC}"
fi

# 5. Create a client
echo -e "\n${YELLOW}5. Creating new client...${NC}"
CLIENT_CREATE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{
        \"name\": \"Test Client $TIMESTAMP\",
        \"email\": \"client$TIMESTAMP@testclient.com\",
        \"phone\": \"+1234567890\",
        \"company\": \"Test Client Corp\",
        \"address\": \"456 Client Ave\",
        \"contactPerson\": \"John Doe\",
        \"notes\": \"Test client\",
        \"status\": \"Active\"
    }" \
    "$BASE_URL/client")

echo "$CLIENT_CREATE" | jq '.'
CLIENT_ID=$(echo "$CLIENT_CREATE" | jq -r '.id // empty')

if [ ! -z "$CLIENT_ID" ]; then
    echo -e "${GREEN}✅ Client created successfully - ID: $CLIENT_ID${NC}"
else
    echo -e "${RED}❌ Failed to create client${NC}"
fi

# 6. Create a project
echo -e "\n${YELLOW}6. Creating new project...${NC}"

# Get a manager user ID
MANAGERS=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/user?role=Manager")
MANAGER_ID=$(echo "$MANAGERS" | jq -r '.items[0].id // empty')

if [ -z "$MANAGER_ID" ]; then
    # Use the SuperAdmin as manager
    MANAGER_ID="aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
fi

PROJECT_CREATE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{
        \"name\": \"Test Project $TIMESTAMP\",
        \"description\": \"Test Project Description\",
        \"startDate\": \"2025-06-18T00:00:00Z\",
        \"endDate\": \"2025-12-31T00:00:00Z\",
        \"status\": \"Planning\",
        \"budget\": 100000,
        \"managerId\": \"$MANAGER_ID\",
        \"clientId\": \"$CLIENT_ID\"
    }" \
    "$BASE_URL/project")

echo "$PROJECT_CREATE" | jq '.'
PROJECT_ID=$(echo "$PROJECT_CREATE" | jq -r '.id // empty')

if [ ! -z "$PROJECT_ID" ]; then
    echo -e "${GREEN}✅ Project created successfully - ID: $PROJECT_ID${NC}"
else
    echo -e "${RED}❌ Failed to create project${NC}"
fi

# 7. Create a task (as SuperAdmin, need to specify CompanyId)
echo -e "\n${YELLOW}7. Creating new task...${NC}"
TASK_CREATE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{
        \"title\": \"Test Task $TIMESTAMP\",
        \"description\": \"Test Task Description\",
        \"priority\": \"High\",
        \"status\": \"Pending\",
        \"category\": \"Development\",
        \"dueDate\": \"2025-07-01T00:00:00Z\",
        \"estimatedHours\": 8,
        \"companyId\": \"$COMPANY_ID\",
        \"assignedToId\": \"$USER_ID\",
        \"projectId\": \"$PROJECT_ID\"
    }" \
    "$BASE_URL/task")

echo "$TASK_CREATE" | jq '.'
TASK_ID=$(echo "$TASK_CREATE" | jq -r '.id // empty')

if [ ! -z "$TASK_ID" ]; then
    echo -e "${GREEN}✅ Task created successfully - ID: $TASK_ID${NC}"
else
    echo -e "${RED}❌ Failed to create task${NC}"
fi

# 8. Login as CompanyAdmin to test without specifying CompanyId
echo -e "\n${YELLOW}8. Testing as CompanyAdmin...${NC}"
ADMIN_LOGIN=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -d '{"email":"john.admin@techinnovations.com","password":"Admin@123"}' \
    "$BASE_URL/auth/login")

ADMIN_TOKEN=$(echo "$ADMIN_LOGIN" | jq -r '.accessToken // empty')
if [ ! -z "$ADMIN_TOKEN" ]; then
    echo -e "${GREEN}✅ CompanyAdmin login successful${NC}"
    
    # Create task as CompanyAdmin (no need to specify CompanyId)
    echo -e "\n${YELLOW}Creating task as CompanyAdmin...${NC}"
    ADMIN_TASK=$(curl -s -X POST \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $ADMIN_TOKEN" \
        -d "{
            \"title\": \"Admin Task $TIMESTAMP\",
            \"description\": \"Created by CompanyAdmin\",
            \"priority\": \"Medium\",
            \"status\": \"Pending\",
            \"dueDate\": \"2025-07-15T00:00:00Z\"
        }" \
        "$BASE_URL/task")
    
    echo "$ADMIN_TASK" | jq '.'
fi

# 9. Check database records
echo -e "\n${YELLOW}9. Checking database records...${NC}"
DB_CHECK=$(docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U doubler -P 'Ramjat@1998' -d TaskManagementDB -C \
    -Q "SELECT 'Companies' as TableName, COUNT(*) as RecordCount FROM Companies WHERE IsDeleted = 0
         UNION ALL SELECT 'Users', COUNT(*) FROM Users WHERE IsDeleted = 0
         UNION ALL SELECT 'Tasks', COUNT(*) FROM Tasks WHERE IsDeleted = 0
         UNION ALL SELECT 'Projects', COUNT(*) FROM Projects WHERE IsDeleted = 0
         UNION ALL SELECT 'Clients', COUNT(*) FROM Clients WHERE IsDeleted = 0
         UNION ALL SELECT 'TaskComments', COUNT(*) FROM TaskComments WHERE IsDeleted = 0
         UNION ALL SELECT 'TaskAttachments', COUNT(*) FROM TaskAttachments WHERE IsDeleted = 0" \
    -h -1 2>/dev/null | grep -v "rows affected")

echo -e "${BLUE}Database Record Counts:${NC}"
echo "$DB_CHECK" | column -t

# 10. Test GET operations to verify data
echo -e "\n${YELLOW}10. Verifying created data...${NC}"

if [ ! -z "$TASK_ID" ]; then
    echo "Fetching created task..."
    curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/task/$TASK_ID" | jq '.title, .status'
fi

if [ ! -z "$PROJECT_ID" ]; then
    echo "Fetching created project..."
    curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/project/$PROJECT_ID" | jq '.name, .status'
fi

echo -e "\n${GREEN}=== Test Complete ===${NC}"
