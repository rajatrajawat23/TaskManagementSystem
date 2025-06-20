#!/bin/bash

# Get authentication token
TOKEN=$(curl -s -X POST http://localhost:5175/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "superadmin@taskmanagement.com", "password": "Admin@123"}' | jq -r '.accessToken')

echo "Creating recurring tasks..."

# 1. Daily Standup Meeting
curl -X POST http://localhost:5175/api/task \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Daily Standup Meeting",
    "description": "Daily team standup meeting at 9:00 AM",
    "companyId": "11111111-1111-1111-1111-111111111111",
    "priority": "Medium",
    "status": "Pending",
    "assignedToId": "cccccccc-cccc-cccc-cccc-cccccccccccc",
    "dueDate": "'$(date -v+1d +%Y-%m-%d)'T09:00:00",
    "startDate": "'$(date +%Y-%m-%d)'T09:00:00",
    "isRecurring": true,
    "recurrencePattern": "{\"Type\":\"Daily\",\"Interval\":1}"
  }'
echo -e "\n✓ Daily Standup Meeting created"

# 2. Weekly Project Status Report
curl -X POST http://localhost:5175/api/task \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Weekly Project Status Report",
    "description": "Prepare and submit weekly project status report",
    "companyId": "11111111-1111-1111-1111-111111111111",
    "priority": "High",
    "status": "Pending",
    "assignedToId": "cccccccc-cccc-cccc-cccc-cccccccccccc",
    "dueDate": "'$(date -v+7d +%Y-%m-%d)'T17:00:00",
    "startDate": "'$(date +%Y-%m-%d)'T09:00:00",
    "isRecurring": true,
    "recurrencePattern": "{\"Type\":\"Weekly\",\"Interval\":1,\"DaysOfWeek\":[5]}"
  }'
echo -e "\n✓ Weekly Project Status Report created"

# 3. Monthly Security Audit
curl -X POST http://localhost:5175/api/task \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Monthly Security Audit",
    "description": "Perform monthly security audit and update documentation",
    "companyId": "11111111-1111-1111-1111-111111111111",
    "priority": "Critical",
    "status": "Pending",
    "assignedToId": "dddddddd-dddd-dddd-dddd-dddddddddddd",
    "dueDate": "'$(date -v+1m +%Y-%m-15)'T17:00:00",
    "startDate": "'$(date +%Y-%m-%d)'T09:00:00",
    "isRecurring": true,
    "recurrencePattern": "{\"Type\":\"Monthly\",\"Interval\":1,\"DayOfMonth\":15}"
  }'
echo -e "\n✓ Monthly Security Audit created"

# 4. Bi-weekly Code Review
curl -X POST http://localhost:5175/api/task \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Code Review Session",
    "description": "Bi-weekly code review and refactoring session",
    "companyId": "11111111-1111-1111-1111-111111111111",
    "priority": "Medium",
    "status": "Pending",
    "assignedToId": "dddddddd-dddd-dddd-dddd-dddddddddddd",
    "dueDate": "'$(date -v+14d +%Y-%m-%d)'T14:00:00",
    "startDate": "'$(date +%Y-%m-%d)'T14:00:00",
    "isRecurring": true,
    "recurrencePattern": "{\"Type\":\"Weekly\",\"Interval\":2}"
  }'
echo -e "\n✓ Bi-weekly Code Review created"

# 5. Quarterly Performance Review
curl -X POST http://localhost:5175/api/task \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Quarterly Performance Review",
    "description": "Team performance review and goal setting",
    "companyId": "11111111-1111-1111-1111-111111111111",
    "priority": "High",
    "status": "Pending",
    "assignedToId": "cccccccc-cccc-cccc-cccc-cccccccccccc",
    "dueDate": "'$(date -v+3m +%Y-%m-%d)'T10:00:00",
    "startDate": "'$(date +%Y-%m-%d)'T10:00:00",
    "isRecurring": true,
    "recurrencePattern": "{\"Type\":\"Monthly\",\"Interval\":3}"
  }'
echo -e "\n✓ Quarterly Performance Review created"

echo -e "\n\nAll recurring tasks created successfully!"

# Verify the recurring tasks
echo -e "\n\nVerifying recurring tasks..."
curl -s -X GET "http://localhost:5175/api/task?pageSize=20" \
  -H "Authorization: Bearer $TOKEN" | jq '.items[] | select(.isRecurring == true) | {title: .title, recurrencePattern: .recurrencePattern}'
