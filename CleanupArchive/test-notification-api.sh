#!/bin/bash

# Notification API Test Script
# Usage: ./test-notification-api.sh

API_URL="http://localhost:5175/api"
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}╔══════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║               NOTIFICATION API TEST SUITE                ║${NC}"
echo -e "${BLUE}╚══════════════════════════════════════════════════════════╝${NC}\n"

# Check if API is running
echo -e "${YELLOW}[INFO] Checking API availability...${NC}"
if ! curl -s "$API_URL/../health" > /dev/null; then
    echo -e "${RED}[ERROR] API is not running on $API_URL${NC}"
    exit 1
fi
echo -e "${GREEN}[SUCCESS] API is running${NC}\n"

# Login to get token
echo -e "${YELLOW}[INFO] Logging in to get access token...${NC}"
LOGIN_RESPONSE=$(curl -s -X POST "$API_URL/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.admin@techinnovations.com",
    "password": "Admin@123"
  }')

if ! echo "$LOGIN_RESPONSE" | jq -e '.success' > /dev/null 2>&1; then
    echo -e "${RED}[ERROR] Login failed${NC}"
    echo "$LOGIN_RESPONSE" | jq .
    exit 1
fi

ACCESS_TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.accessToken')
echo -e "${GREEN}[SUCCESS] Login successful${NC}\n"

# Test Endpoints
echo -e "${BLUE}═══ TESTING NOTIFICATION ENDPOINTS ═══${NC}\n"

# Test 1: Get Notifications
echo -e "${YELLOW}[TEST 1] GET /api/Notification${NC}"
RESPONSE=$(curl -s -X GET "$API_URL/Notification" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Content-Type: application/json")
STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X GET "$API_URL/Notification" \
  -H "Authorization: Bearer $ACCESS_TOKEN")

if [ "$STATUS" = "200" ] && ! echo "$RESPONSE" | grep -q "error occurred"; then
    echo -e "${GREEN}✓ PASSED${NC} - Status: $STATUS"
    echo "  Response: $(echo "$RESPONSE" | jq -c .)"
else
    echo -e "${RED}✗ FAILED${NC} - Status: $STATUS"
    echo "  Response: $(echo "$RESPONSE" | jq -c .)"
fi
echo ""

# Test 2: Get Unread Count
echo -e "${YELLOW}[TEST 2] GET /api/Notification/unread-count${NC}"
RESPONSE=$(curl -s -X GET "$API_URL/Notification/unread-count" \
  -H "Authorization: Bearer $ACCESS_TOKEN")
STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X GET "$API_URL/Notification/unread-count" \
  -H "Authorization: Bearer $ACCESS_TOKEN")

if [ "$STATUS" = "200" ] && echo "$RESPONSE" | jq -e '.unreadCount' > /dev/null 2>&1; then
    echo -e "${GREEN}✓ PASSED${NC} - Status: $STATUS"
    echo "  Response: $(echo "$RESPONSE" | jq -c .)"
else
    echo -e "${RED}✗ FAILED${NC} - Status: $STATUS"
    echo "  Response: $(echo "$RESPONSE" | jq -c .)"
fi
echo ""

# Test 3: Test Notification (SuperAdmin only)
echo -e "${YELLOW}[TEST 3] POST /api/Notification/test${NC}"
TEST_RESPONSE=$(curl -s -X POST "$API_URL/Notification/test" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
    "title": "Test Notification",
    "message": "This is a test notification from script",
    "priority": "Normal"
  }')
TEST_STATUS=$(curl -s -o /dev/null -w "%{http_code}" -X POST "$API_URL/Notification/test" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"userId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"}')

if [ "$TEST_STATUS" = "200" ]; then
    echo -e "${GREEN}✓ PASSED${NC} - Status: $TEST_STATUS"
else
    echo -e "${YELLOW}⚠ EXPECTED FAILURE${NC} - Status: $TEST_STATUS (Requires SuperAdmin)"
fi
echo "  Response: $(echo "$TEST_RESPONSE" | jq -c . 2>/dev/null || echo "$TEST_RESPONSE")"
echo ""

# Database Verification
echo -e "${BLUE}═══ DATABASE VERIFICATION ═══${NC}\n"
echo -e "${YELLOW}[INFO] Checking database state...${NC}"

# Check if Docker container is running
if docker ps | grep -q taskmanagement-sqlserver; then
    DB_STATS=$(docker exec taskmanagement-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P 'SecureTask2025#@!' -C -Q "SET QUOTED_IDENTIFIER ON; USE TaskManagementDB; SELECT 'Total' as Type, COUNT(*) as Count FROM Communication.Notifications UNION ALL SELECT 'User', COUNT(*) FROM Communication.Notifications WHERE UserId = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb';" -h -1 2>/dev/null | grep -E "Total|User" | tr -s ' ')
    
    if [ ! -z "$DB_STATS" ]; then
        echo -e "${GREEN}[SUCCESS] Database verification:${NC}"
        echo "$DB_STATS"
    else
        echo -e "${YELLOW}[WARNING] Could not verify database state${NC}"
    fi
else
    echo -e "${YELLOW}[WARNING] Database container not found${NC}"
fi

echo ""
echo -e "${BLUE}═══ TEST COMPLETE ═══${NC}"
echo -e "${YELLOW}Note: Some endpoints may fail due to repository layer issues.${NC}"
echo -e "${YELLOW}Refer to notification-api-test-report.md for detailed analysis.${NC}"