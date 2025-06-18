#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

# API Base URL
BASE_URL="http://localhost:5175/api"

# Test results file
RESULTS_FILE="service-test-results.md"

# Initialize results file
echo "# Service Testing Results - $(date)" > $RESULTS_FILE
echo "" >> $RESULTS_FILE

# Function to print colored output
print_status() {
    if [ $1 -eq 0 ]; then
        echo -e "${GREEN}✅ $2${NC}"
        echo "✅ $2" >> $RESULTS_FILE
    else
        echo -e "${RED}❌ $2${NC}"
        echo "❌ $2" >> $RESULTS_FILE
    fi
}

# Function to test endpoint
test_endpoint() {
    local method=$1
    local endpoint=$2
    local data=$3
    local token=$4
    local expected_status=$5
    local description=$6
    
    if [ -n "$token" ]; then
        response=$(curl -s -w "\n%{http_code}" -X $method \
            -H "Content-Type: application/json" \
            -H "Authorization: Bearer $token" \
            -d "$data" \
            "$BASE_URL$endpoint")
    else
        response=$(curl -s -w "\n%{http_code}" -X $method \
            -H "Content-Type: application/json" \
            -d "$data" \
            "$BASE_URL$endpoint")
    fi
    
    status_code=$(echo "$response" | tail -n 1)
    body=$(echo "$response" | head -n -1)
    
    if [ "$status_code" = "$expected_status" ]; then
        print_status 0 "$description - Status: $status_code"
        echo "$body" | jq '.' 2>/dev/null || echo "$body"
        return 0
    else
        print_status 1 "$description - Expected: $expected_status, Got: $status_code"
        echo "Response: $body"
        return 1
    fi
}

echo "=== Starting API Service Tests ==="
echo ""

# Check if API is running
echo "Checking API health..."
health_check=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5175/health)
if [ "$health_check" != "200" ]; then
    echo -e "${RED}API is not running! Please start it first.${NC}"
    exit 1
fi
print_status 0 "API is running"
echo ""

# 1. AUTH SERVICE TESTS
echo "## 1. Testing Auth Service" >> $RESULTS_FILE
echo "=== 1. Testing Auth Service ==="

# Test login
echo "Testing login..."
login_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -d '{"email":"superadmin@taskmanagement.com","password":"Admin@123"}' \
    "$BASE_URL/auth/login")

if echo "$login_response" | jq -e '.accessToken' > /dev/null 2>&1; then
    TOKEN=$(echo "$login_response" | jq -r '.accessToken')
    print_status 0 "Login successful"
    echo "Token obtained: ${TOKEN:0:20}..."
    
    # Also get the CompanyId for the superadmin's first company
    COMPANY_ID="b8b8e094-5d7f-4b7d-8d8b-5b5d7f8d8b8b"
else
    print_status 1 "Login failed"
    echo "$login_response"
    exit 1
fi

# Test register (requires admin token)
echo "Testing registration..."
# Get first company for testing
COMPANY_RESPONSE=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/company?pageSize=1")
COMPANY_ID=$(echo "$COMPANY_RESPONSE" | jq -r '.items[0].id // empty')
if [ -z "$COMPANY_ID" ]; then
    COMPANY_ID="b8b8e094-5d7f-4b7d-8d8b-5b5d7f8d8b8b"
fi

test_endpoint "POST" "/auth/register" \
    "{\"email\":\"test.user.$(date +%s)@example.com\",\"password\":\"Test@123\",\"firstName\":\"Test\",\"lastName\":\"User\",\"role\":\"User\",\"companyId\":\"$COMPANY_ID\"}" \
    "$TOKEN" "200" "Register new user"

# Test change password
echo "Testing change password..."
test_endpoint "POST" "/auth/change-password" \
    '{"currentPassword":"Admin@123","newPassword":"Admin@123"}' \
    "$TOKEN" "200" "Change password"

echo "" >> $RESULTS_FILE

# 2. COMPANY SERVICE TESTS
echo "## 2. Testing Company Service" >> $RESULTS_FILE
echo -e "\n=== 2. Testing Company Service ==="

# Get all companies (SuperAdmin only)
echo "Testing get all companies..."
test_endpoint "GET" "/company" "" "$TOKEN" "200" "Get all companies"

# Create new company
echo "Testing create company..."
COMPANY_ID=$(uuidgen | tr '[:upper:]' '[:lower:]')
test_endpoint "POST" "/company" \
    "{\"id\":\"$COMPANY_ID\",\"name\":\"Test Company\",\"domain\":\"testcompany\",\"contactEmail\":\"admin@testcompany.com\",\"subscriptionType\":\"Premium\",\"maxUsers\":50}" \
    "$TOKEN" "201" "Create new company"

# Get company by ID
echo "Testing get company by ID..."
test_endpoint "GET" "/company/$COMPANY_ID" "" "$TOKEN" "200" "Get company by ID"

# Update company
echo "Testing update company..."
test_endpoint "PUT" "/company/$COMPANY_ID" \
    "{\"name\":\"Test Company Updated\",\"contactEmail\":\"updated@testcompany.com\"}" \
    "$TOKEN" "200" "Update company"

echo "" >> $RESULTS_FILE

# 3. USER SERVICE TESTS
echo "## 3. Testing User Service" >> $RESULTS_FILE
echo -e "\n=== 3. Testing User Service ==="

# Get all users
echo "Testing get all users..."
test_endpoint "GET" "/user" "" "$TOKEN" "200" "Get all users"

# Get current user profile
echo "Testing get profile..."
test_endpoint "GET" "/user/profile" "" "$TOKEN" "200" "Get user profile"

# Create new user
echo "Testing create user..."
USER_ID=$(uuidgen | tr '[:upper:]' '[:lower:]')
test_endpoint "POST" "/user" \
    "{\"email\":\"newuser@example.com\",\"password\":\"User@123\",\"firstName\":\"New\",\"lastName\":\"User\",\"role\":\"User\"}" \
    "$TOKEN" "201" "Create new user"

echo "" >> $RESULTS_FILE

# 4. CLIENT SERVICE TESTS
echo "## 4. Testing Client Service" >> $RESULTS_FILE
echo -e "\n=== 4. Testing Client Service ==="

# Get all clients
echo "Testing get all clients..."
test_endpoint "GET" "/client" "" "$TOKEN" "200" "Get all clients"

# Create new client
echo "Testing create client..."
CLIENT_ID=$(uuidgen | tr '[:upper:]' '[:lower:]')
test_endpoint "POST" "/client" \
    "{\"name\":\"Test Client\",\"email\":\"client@example.com\",\"phone\":\"+1234567890\",\"company\":\"Client Corp\",\"address\":\"123 Client St\"}" \
    "$TOKEN" "201" "Create new client"

# Get client by ID
echo "Testing get client by ID..."
test_endpoint "GET" "/client/$CLIENT_ID" "" "$TOKEN" "200" "Get client by ID"

echo "" >> $RESULTS_FILE

# 5. PROJECT SERVICE TESTS
echo "## 5. Testing Project Service" >> $RESULTS_FILE
echo -e "\n=== 5. Testing Project Service ==="

# Get all projects
echo "Testing get all projects..."
test_endpoint "GET" "/project" "" "$TOKEN" "200" "Get all projects"

# Create new project
echo "Testing create project..."
PROJECT_ID=$(uuidgen | tr '[:upper:]' '[:lower:]')
test_endpoint "POST" "/project" \
    "{\"name\":\"Test Project\",\"description\":\"Test project description\",\"startDate\":\"2025-06-18\",\"endDate\":\"2025-12-31\",\"status\":\"Planning\",\"budget\":100000}" \
    "$TOKEN" "201" "Create new project"

# Get project by ID
echo "Testing get project by ID..."
test_endpoint "GET" "/project/$PROJECT_ID" "" "$TOKEN" "200" "Get project by ID"

echo "" >> $RESULTS_FILE

# 6. TASK SERVICE TESTS
echo "## 6. Testing Task Service" >> $RESULTS_FILE
echo -e "\n=== 6. Testing Task Service ==="

# Get all tasks
echo "Testing get all tasks..."
test_endpoint "GET" "/task" "" "$TOKEN" "200" "Get all tasks"

# Create new task
echo "Testing create task..."
TASK_ID=$(uuidgen | tr '[:upper:]' '[:lower:]')
test_endpoint "POST" "/task" \
    "{\"title\":\"Test Task\",\"description\":\"Test task description\",\"priority\":\"High\",\"status\":\"Pending\",\"dueDate\":\"2025-07-01\",\"companyId\":\"b8b8e094-5d7f-4b7d-8d8b-5b5d7f8d8b8b\"}" \
    "$TOKEN" "201" "Create new task"

# Get task by ID
echo "Testing get task by ID..."
test_endpoint "GET" "/task/$TASK_ID" "" "$TOKEN" "200" "Get task by ID"

# Update task status
echo "Testing update task status..."
test_endpoint "PUT" "/task/$TASK_ID/status" \
    "{\"status\":\"InProgress\"}" \
    "$TOKEN" "200" "Update task status"

# Get task statistics
echo "Testing get task statistics..."
test_endpoint "GET" "/task/statistics" "" "$TOKEN" "200" "Get task statistics"

echo "" >> $RESULTS_FILE

# Test database verification
echo -e "\n=== 7. Verifying Database Saves ===" >> $RESULTS_FILE
echo -e "\n=== 7. Verifying Database Saves ==="

# Check if data was saved in database
echo "Checking database for saved records..."
docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U doubler -P 'Ramjat@1998' -d TaskManagementDB -C \
    -Q "SELECT 'Companies' as TableName, COUNT(*) as RecordCount FROM Companies WHERE IsDeleted = 0
         UNION ALL
         SELECT 'Users', COUNT(*) FROM Users WHERE IsDeleted = 0
         UNION ALL
         SELECT 'Tasks', COUNT(*) FROM Tasks WHERE IsDeleted = 0
         UNION ALL
         SELECT 'Projects', COUNT(*) FROM Projects WHERE IsDeleted = 0
         UNION ALL
         SELECT 'Clients', COUNT(*) FROM Clients WHERE IsDeleted = 0" \
    -h -1 2>/dev/null | grep -v "rows affected" >> $RESULTS_FILE

echo "" >> $RESULTS_FILE
echo "## Summary" >> $RESULTS_FILE
echo "Test completed at: $(date)" >> $RESULTS_FILE

# Display summary
echo -e "\n=== Test Summary ==="
echo "Results saved to: $RESULTS_FILE"
echo ""
cat $RESULTS_FILE | grep -E "^✅|^❌" | sort | uniq -c

# Kill the API process
echo -e "\n${YELLOW}Stopping API...${NC}"
pkill -f "dotnet.*TaskManagement.API"
