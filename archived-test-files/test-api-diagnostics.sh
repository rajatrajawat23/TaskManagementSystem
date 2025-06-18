#!/bin/bash

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

BASE_URL="http://localhost:5175"

echo -e "${BLUE}=== API Diagnostics ===${NC}"

# 1. Check API health
echo -e "\n${YELLOW}1. Checking API Health...${NC}"
curl -s "$BASE_URL/health" && echo -e "\n${GREEN}✅ API is healthy${NC}" || echo -e "\n${RED}❌ API is not responding${NC}"

# 2. Check Swagger
echo -e "\n${YELLOW}2. Checking Swagger UI...${NC}"
swagger_response=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/swagger/v1/swagger.json")
if [ "$swagger_response" = "200" ]; then
    echo -e "${GREEN}✅ Swagger is available at $BASE_URL${NC}"
else
    echo -e "${RED}❌ Swagger is not available${NC}"
fi

# 3. Test authentication endpoint directly
echo -e "\n${YELLOW}3. Testing Authentication...${NC}"
echo "Request body:"
echo '{"email":"superadmin@taskmanagement.com","password":"Admin@123"}'

auth_response=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -H "Accept: application/json" \
    -d '{"email":"superadmin@taskmanagement.com","password":"Admin@123"}' \
    "$BASE_URL/api/auth/login")

echo -e "\nResponse:"
echo "$auth_response" | jq '.' 2>/dev/null || echo "$auth_response"

if echo "$auth_response" | jq -e '.accessToken' > /dev/null 2>&1; then
    TOKEN=$(echo "$auth_response" | jq -r '.accessToken')
    echo -e "\n${GREEN}✅ Authentication successful${NC}"
    
    # 4. Test simple GET endpoint with token
    echo -e "\n${YELLOW}4. Testing GET /api/task with authentication...${NC}"
    tasks_response=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/api/task?pageSize=1")
    echo "$tasks_response" | jq '.' 2>/dev/null || echo "$tasks_response"
    
    # 5. Test POST with different content types
    echo -e "\n${YELLOW}5. Testing POST /api/task with proper headers...${NC}"
    
    # First, let's get a valid company ID
    companies=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/api/company?pageSize=1")
    COMPANY_ID=$(echo "$companies" | jq -r '.items[0].id // empty')
    
    if [ -n "$COMPANY_ID" ]; then
        echo "Using Company ID: $COMPANY_ID"
        
        # Test with explicit content type and charset
        echo -e "\nSending POST request..."
        
        task_json="{
            \"title\": \"Test Task\",
            \"description\": \"Test Description\",
            \"priority\": \"High\",
            \"status\": \"Pending\",
            \"dueDate\": \"2025-07-01T00:00:00Z\",
            \"companyId\": \"$COMPANY_ID\"
        }"
        
        echo "Request body:"
        echo "$task_json" | jq '.'
        
        create_response=$(curl -s -v -X POST \
            -H "Content-Type: application/json; charset=utf-8" \
            -H "Accept: application/json" \
            -H "Authorization: Bearer $TOKEN" \
            -d "$task_json" \
            "$BASE_URL/api/task" 2>&1)
        
        echo -e "\nFull response with headers:"
        echo "$create_response"
    else
        echo -e "${RED}No company found to test with${NC}"
    fi
    
    # 6. Test diagnostics endpoint
    echo -e "\n${YELLOW}6. Testing diagnostics endpoint...${NC}"
    diag_response=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/api/diagnostics/test")
    echo "$diag_response" | jq '.' 2>/dev/null || echo "$diag_response"
    
else
    echo -e "\n${RED}❌ Authentication failed${NC}"
fi

echo -e "\n${BLUE}=== Diagnostics Complete ===${NC}"
