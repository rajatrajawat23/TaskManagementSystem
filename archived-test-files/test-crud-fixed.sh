#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# API Base URL
BASE_URL="http://localhost:5175/api"

# Function to print section headers
print_header() {
    echo -e "\n${BLUE}=== $1 ===${NC}"
}

# Function to test endpoint with detailed output
test_endpoint() {
    local method=$1
    local endpoint=$2
    local data=$3
    local token=$4
    local description=$5
    
    echo -e "\n${YELLOW}Testing: $description${NC}"
    
    if [ -n "$token" ]; then
        response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X $method \
            -H "Content-Type: application/json" \
            -H "Authorization: Bearer $token" \
            -d "$data" \
            "$BASE_URL$endpoint" 2>&1)
    else
        response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X $method \
            -H "Content-Type: application/json" \
            -d "$data" \
            "$BASE_URL$endpoint" 2>&1)
    fi
    
    # Extract status code and body
    status_code=$(echo "$response" | grep -o "HTTP_STATUS:[0-9]*" | cut -d: -f2)
    body=$(echo "$response" | sed '$d')
    
    echo "Status: $status_code"
    if [[ "$status_code" =~ ^2[0-9][0-9]$ ]]; then
        echo -e "${GREEN}✅ Success${NC}"
        echo "$body" | jq '.' 2>/dev/null || echo "$body"
        return 0
    else
        echo -e "${RED}❌ Failed${NC}"
        echo "$body" | jq '.' 2>/dev/null || echo "$body"
        return 1
    fi
}

# Start of tests
echo -e "${BLUE}Task Management System - CRUD Operations Test${NC}"
echo -e "${BLUE}============================================${NC}"

# 1. LOGIN TO GET TOKEN
print_header "1. Authentication"
login_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -d '{"email":"superadmin@taskmanagement.com","password":"Admin@123"}' \
    "$BASE_URL/auth/login")

if echo "$login_response" | jq -e '.accessToken' > /dev/null 2>&1; then
    TOKEN=$(echo "$login_response" | jq -r '.accessToken')
    echo -e "${GREEN}✅ Login successful${NC}"
else
    echo -e "${RED}❌ Login failed${NC}"
    echo "$login_response" | jq '.'
    exit 1
fi

# 2. GET OR CREATE A COMPANY
print_header "2. Company Setup"

# First try to get existing companies
companies_response=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/company?pageSize=1")
COMPANY_ID=$(echo "$companies_response" | jq -r '.items[0].id // empty')

if [ -z "$COMPANY_ID" ]; then
    echo "No existing company found. Creating one..."
    # Create a company
    company_data='{
        "name": "Test Company",
        "domain": "testcompany",
        "contactEmail": "admin@testcompany.com",
        "contactPhone": "+1234567890",
        "address": "123 Test Street",
        "subscriptionType": "Premium",
        "maxUsers": 50
    }'
    
    create_company_response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X POST \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $TOKEN" \
        -d "$company_data" \
        "$BASE_URL/company")
    
    status_code=$(echo "$create_company_response" | grep -o "HTTP_STATUS:[0-9]*" | cut -d: -f2)
    body=$(echo "$create_company_response" | sed '$d')
    
    if [[ "$status_code" =~ ^2[0-9][0-9]$ ]]; then
        COMPANY_ID=$(echo "$body" | jq -r '.id')
        echo -e "${GREEN}✅ Company created with ID: $COMPANY_ID${NC}"
    else
        echo -e "${RED}❌ Failed to create company${NC}"
        echo "$body" | jq '.'
    fi
else
    echo -e "${GREEN}✅ Using existing company ID: $COMPANY_ID${NC}"
fi

# 3. CREATE A USER TO BE MANAGER
print_header "3. Manager User Setup"

# Create a manager user
manager_data="{
    \"email\": \"manager$(date +%s)@example.com\",
    \"password\": \"Manager@123\",
    \"firstName\": \"Test\",
    \"lastName\": \"Manager\",
    \"role\": \"Manager\",
    \"companyId\": \"$COMPANY_ID\"
}"

manager_response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "$manager_data" \
    "$BASE_URL/user")

status_code=$(echo "$manager_response" | grep -o "HTTP_STATUS:[0-9]*" | cut -d: -f2)
body=$(echo "$manager_response" | sed '$d')

if [[ "$status_code" =~ ^2[0-9][0-9]$ ]]; then
    MANAGER_ID=$(echo "$body" | jq -r '.id')
    echo -e "${GREEN}✅ Manager created with ID: $MANAGER_ID${NC}"
else
    # If creation fails, try to get an existing manager
    users_response=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/user?role=Manager&pageSize=1")
    MANAGER_ID=$(echo "$users_response" | jq -r '.items[0].id // empty')
    if [ -z "$MANAGER_ID" ]; then
        MANAGER_ID="aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" # Use superadmin as fallback
    fi
    echo -e "${YELLOW}Using existing manager ID: $MANAGER_ID${NC}"
fi

# 4. TEST TASK CRUD OPERATIONS
print_header "4. Task CRUD Operations"

# Create a task with CompanyId
task_data="{
    \"title\": \"Test Task $(date +%s)\",
    \"description\": \"This is a test task description\",
    \"priority\": \"High\",
    \"status\": \"Pending\",
    \"dueDate\": \"2025-07-01T00:00:00\",
    \"category\": \"Development\",
    \"estimatedHours\": 8,
    \"companyId\": \"$COMPANY_ID\"
}"

test_endpoint "POST" "/task" "$task_data" "$TOKEN" "Create Task"

if [ $? -eq 0 ]; then
    TASK_ID=$(echo "$body" | jq -r '.id // empty')
    if [ -n "$TASK_ID" ]; then
        # Read task
        test_endpoint "GET" "/task/$TASK_ID" "" "$TOKEN" "Get Task by ID"
        
        # Update task
        update_data="{
            \"title\": \"Updated Test Task\",
            \"description\": \"Updated description\",
            \"priority\": \"Medium\",
            \"status\": \"InProgress\"
        }"
        test_endpoint "PUT" "/task/$TASK_ID" "$update_data" "$TOKEN" "Update Task"
        
        # Delete task
        test_endpoint "DELETE" "/task/$TASK_ID" "" "$TOKEN" "Delete Task"
    fi
fi

# 5. TEST CLIENT CRUD OPERATIONS
print_header "5. Client CRUD Operations"

# Create a client with CompanyId
client_data="{
    \"name\": \"Test Client $(date +%s)\",
    \"email\": \"testclient@example.com\",
    \"phone\": \"+1234567890\",
    \"contactPerson\": \"John Doe\",
    \"industry\": \"Technology\",
    \"address\": \"123 Main St, City, Country\",
    \"companyId\": \"$COMPANY_ID\"
}"

test_endpoint "POST" "/client" "$client_data" "$TOKEN" "Create Client"

if [ $? -eq 0 ]; then
    CLIENT_ID=$(echo "$body" | jq -r '.id // empty')
    if [ -n "$CLIENT_ID" ]; then
        # Read client
        test_endpoint "GET" "/client/$CLIENT_ID" "" "$TOKEN" "Get Client by ID"
        
        # Update client
        update_client="{
            \"name\": \"Updated Client Name\",
            \"contactPerson\": \"Jane Smith\"
        }"
        test_endpoint "PUT" "/client/$CLIENT_ID" "$update_client" "$TOKEN" "Update Client"
        
        # Delete client
        test_endpoint "DELETE" "/client/$CLIENT_ID" "" "$TOKEN" "Delete Client"
    fi
fi

# 6. TEST PROJECT CRUD OPERATIONS
print_header "6. Project CRUD Operations"

# Create a project with ManagerId
project_data="{
    \"name\": \"Test Project $(date +%s)\",
    \"description\": \"Test project description\",
    \"projectCode\": \"PROJ-$(date +%s)\",
    \"startDate\": \"2025-06-18T00:00:00\",
    \"endDate\": \"2025-12-31T00:00:00\",
    \"status\": \"Planning\",
    \"budget\": 100000.00,
    \"projectManagerId\": \"$MANAGER_ID\",
    \"companyId\": \"$COMPANY_ID\"
}"

test_endpoint "POST" "/project" "$project_data" "$TOKEN" "Create Project"

if [ $? -eq 0 ]; then
    PROJECT_ID=$(echo "$body" | jq -r '.id // empty')
    if [ -n "$PROJECT_ID" ]; then
        # Read project
        test_endpoint "GET" "/project/$PROJECT_ID" "" "$TOKEN" "Get Project by ID"
        
        # Update project
        update_project="{
            \"name\": \"Updated Project Name\",
            \"status\": \"InProgress\",
            \"progress\": 25
        }"
        test_endpoint "PUT" "/project/$PROJECT_ID" "$update_project" "$TOKEN" "Update Project"
        
        # Delete project
        test_endpoint "DELETE" "/project/$PROJECT_ID" "" "$TOKEN" "Delete Project"
    fi
fi

# 7. TEST USER CRUD OPERATIONS
print_header "7. User CRUD Operations"

# Create a user
user_data="{
    \"email\": \"testuser$(date +%s)@example.com\",
    \"password\": \"User@123\",
    \"firstName\": \"Test\",
    \"lastName\": \"User\",
    \"role\": \"User\",
    \"companyId\": \"$COMPANY_ID\"
}"

test_endpoint "POST" "/user" "$user_data" "$TOKEN" "Create User"

if [ $? -eq 0 ]; then
    USER_ID=$(echo "$body" | jq -r '.id // empty')
    if [ -n "$USER_ID" ]; then
        # Read user
        test_endpoint "GET" "/user/$USER_ID" "" "$TOKEN" "Get User by ID"
        
        # Update user
        update_user="{
            \"firstName\": \"Updated\",
            \"lastName\": \"Name\",
            \"department\": \"IT\"
        }"
        test_endpoint "PUT" "/user/$USER_ID" "$update_user" "$TOKEN" "Update User"
        
        # Delete user (soft delete)
        test_endpoint "DELETE" "/user/$USER_ID" "" "$TOKEN" "Delete User"
    fi
fi

# 8. SUMMARY
print_header "Test Summary"

# Check database records
echo -e "\n${YELLOW}Checking database records...${NC}"
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P 'SecureTask2025#@!' -d TaskManagementDB -C \
    -Q "SELECT 'Entity' as Entity, 'Count' as Count
         UNION ALL
         SELECT 'Tasks', CAST(COUNT(*) as varchar) FROM [Core].[Tasks] WHERE IsDeleted = 0
         UNION ALL
         SELECT 'Clients', CAST(COUNT(*) as varchar) FROM [Core].[Clients] WHERE IsDeleted = 0
         UNION ALL
         SELECT 'Projects', CAST(COUNT(*) as varchar) FROM [Core].[Projects] WHERE IsDeleted = 0
         UNION ALL
         SELECT 'Users', CAST(COUNT(*) as varchar) FROM [Security].[Users] WHERE IsDeleted = 0
         UNION ALL
         SELECT 'Companies', CAST(COUNT(*) as varchar) FROM [Core].[Companies] WHERE IsDeleted = 0" \
    -s "|" -W 2>/dev/null || echo "Database query failed"

echo -e "\n${GREEN}Test Complete!${NC}"
echo -e "${YELLOW}Check the output above for detailed results of each CRUD operation.${NC}"
