#!/bin/bash

API_BASE="http://localhost:5175/api"

# Login first
echo "Logging in..."
LOGIN_RESPONSE=$(curl -s -X POST "$API_BASE/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@taskmanagement.com",
    "password": "Admin@123"
  }')

TOKEN=$(echo $LOGIN_RESPONSE | jq -r '.accessToken')
echo "Token obtained"

# Create a task first
echo -e "\nCreating a task..."
CREATE_RESPONSE=$(curl -s -X POST "$API_BASE/task" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Task for Update",
    "description": "Original description",
    "priority": "Medium",
    "status": "Pending",
    "companyId": "11111111-1111-1111-1111-111111111111"
  }')

TASK_ID=$(echo $CREATE_RESPONSE | jq -r '.id')
echo "Task created with ID: $TASK_ID"

# Get the task details
echo -e "\nGetting task details..."
TASK_DETAILS=$(curl -s -X GET "$API_BASE/task/$TASK_ID" \
  -H "Authorization: Bearer $TOKEN")

echo "Task details:"
echo $TASK_DETAILS | jq '.'

# Try update with minimal fields
echo -e "\nTrying update with minimal fields..."
UPDATE_RESPONSE=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X PUT "$API_BASE/task/$TASK_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Updated Title Only"
  }')

echo "Update response:"
echo "$UPDATE_RESPONSE"

# Try update with all required fields based on DTO
echo -e "\nTrying update with all required fields..."
UPDATE_RESPONSE2=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X PUT "$API_BASE/task/$TASK_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "'$TASK_ID'",
    "title": "Fully Updated Task",
    "description": "Updated description",
    "priority": "High",
    "status": "InProgress",
    "progress": 25,
    "isRecurring": false,
    "estimatedHours": 5.5
  }')

echo "Update response 2:"
echo "$UPDATE_RESPONSE2"

# Test status update endpoint
echo -e "\nTesting status update endpoint..."
STATUS_UPDATE=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X PUT "$API_BASE/task/$TASK_ID/status" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Review"
  }')

echo "Status update response:"
echo "$STATUS_UPDATE"

# Test delete
echo -e "\nTesting delete..."
DELETE_RESPONSE=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X DELETE "$API_BASE/task/$TASK_ID" \
  -H "Authorization: Bearer $TOKEN")

echo "Delete response:"
echo "$DELETE_RESPONSE"
