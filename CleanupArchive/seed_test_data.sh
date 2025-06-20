#!/bin/bash

# Create a test user directly by calling the registration endpoint without authentication
# We'll create a simple HTTP client request

# First, let's try to create a super admin user account that can create other accounts
echo "Testing if we can create a super admin account..."

# Try creating a super admin account (this will require manual database seeding or a special setup endpoint)
# For now, let's create a temporary test user using existing valid credentials

echo "Let's try with known credentials from the existing tokens..."

# Read the debug token and try to refresh it
TOKEN=$(cat company_admin_token.txt)
echo "Trying to refresh the existing token..."

curl -X POST http://localhost:5175/api/Auth/refresh-token \
  -H "Content-Type: application/json" \
  -d "{\"refreshToken\": \"\"}" | jq .

echo "If refresh fails, we'll need to seed data directly..."
