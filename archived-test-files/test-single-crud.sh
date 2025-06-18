#!/bin/bash

# Simple test for a single CRUD operation
BASE_URL="http://localhost:5175/api"

echo "=== Simple CRUD Test ==="

# 1. Login
echo -e "\n1. Login..."
LOGIN_RESPONSE=$(curl -s -X POST \
    -H "Content-Type: application/json" \
    -d '{"email":"superadmin@taskmanagement.com","password":"Admin@123"}' \
    "$BASE_URL/auth/login")

TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.accessToken')
echo "Token obtained: ${TOKEN:0:30}..."

# 2. Get existing company
echo -e "\n2. Getting existing company..."
COMPANIES=$(curl -s -H "Authorization: Bearer $TOKEN" "$BASE_URL/company?pageSize=1")
COMPANY_ID=$(echo "$COMPANIES" | jq -r '.items[0].id // empty')
echo "Company ID: $COMPANY_ID"

# 3. Create a simple task
echo -e "\n3. Creating task..."
TASK_DATA="{
    \"title\": \"Simple Test Task\",
    \"description\": \"Test\",
    \"priority\": \"High\",
    \"status\": \"Pending\",
    \"companyId\": \"$COMPANY_ID\"
}"

echo "Request body:"
echo "$TASK_DATA" | jq '.'

# Make the request with verbose output
echo -e "\nMaking POST request..."
curl -v -X POST \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "$TASK_DATA" \
    "$BASE_URL/task" 2>&1

# 4. Check the API logs
echo -e "\n\n=== Recent API Logs ==="
tail -50 /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/api.log | grep -E "POST /api/task|Error|Exception|validation"
