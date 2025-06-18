#!/bin/bash

API_BASE="http://localhost:5175/api"

# Login
LOGIN_RESPONSE=$(curl -s -X POST "$API_BASE/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "superadmin@taskmanagement.com",
    "password": "Admin@123"
  }')

TOKEN=$(echo $LOGIN_RESPONSE | jq -r '.accessToken')

# First get an existing task
echo "Getting existing tasks..."
TASKS=$(curl -s -X GET "$API_BASE/task?pageSize=1" \
  -H "Authorization: Bearer $TOKEN")

TASK_ID=$(echo $TASKS | jq -r '.items[0].id')
echo "Using task ID: $TASK_ID"

# Get the full task details
echo -e "\nGetting task details..."
TASK=$(curl -s -X GET "$API_BASE/task/$TASK_ID" \
  -H "Authorization: Bearer $TOKEN")

echo "Current task:"
echo $TASK | jq '{id, title, priority, status}'

# Extract current values
CURRENT_TITLE=$(echo $TASK | jq -r '.title')
CURRENT_PRIORITY=$(echo $TASK | jq -r '.priority')
CURRENT_STATUS=$(echo $TASK | jq -r '.status')

# Try update with all fields from current task
echo -e "\nUpdating with complete DTO..."
UPDATE_PAYLOAD=$(cat <<EOF
{
  "id": "$TASK_ID",
  "title": "Updated: $CURRENT_TITLE",
  "description": "Updated at $(date)",
  "priority": "$CURRENT_PRIORITY",
  "status": "$CURRENT_STATUS",
  "isRecurring": false,
  "progress": 10,
  "tags": []
}
EOF
)

echo "Update payload:"
echo $UPDATE_PAYLOAD | jq '.'

UPDATE_RESPONSE=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X PUT "$API_BASE/task/$TASK_ID" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "$UPDATE_PAYLOAD")

echo -e "\nUpdate response:"
echo "$UPDATE_RESPONSE"
