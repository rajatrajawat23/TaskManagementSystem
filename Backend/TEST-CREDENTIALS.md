# Task Management System - Test Credentials

## 🔐 Default Test Users (from Seed Data)

Based on the seed data scripts, these users should be available if the seed data was executed:

### 1. Super Admin
- **Email**: `superadmin@taskmanagement.com`
- **Password**: `Admin@123`
- **Role**: SuperAdmin
- **Access**: Full system access

### 2. Company Admin (Tech Innovations Inc)
- **Email**: `john.admin@techinnovations.com`
- **Password**: `Admin@123`
- **Role**: CompanyAdmin
- **Company**: Tech Innovations Inc

### 3. Manager (Tech Innovations Inc)
- **Email**: `sarah.manager@techinnovations.com`
- **Password**: `Admin@123`
- **Role**: Manager
- **Company**: Tech Innovations Inc

### 4. Regular User (Tech Innovations Inc)
- **Email**: `mike.user@techinnovations.com`
- **Password**: `Admin@123`
- **Role**: User
- **Company**: Tech Innovations Inc

### 5. Company Admin (Digital Solutions Ltd)
- **Email**: `emma.admin@digitalsolutions.com`
- **Password**: `Admin@123`
- **Role**: CompanyAdmin
- **Company**: Digital Solutions Ltd

## 🚀 Quick Start - Create Your Own Test User

If the above users don't work, you can create your own test user:

### Step 1: Open Swagger UI
Navigate to: http://localhost:5175

### Step 2: Register a New User
Use the `/api/auth/register` endpoint with this JSON:

```json
{
  "email": "test@example.com",
  "password": "Test@123456",
  "confirmPassword": "Test@123456",
  "firstName": "Test",
  "lastName": "User",
  "role": "SuperAdmin"
}
```

### Step 3: Login
Use the `/api/auth/login` endpoint with:

```json
{
  "email": "test@example.com",
  "password": "Test@123456"
}
```

### Step 4: Use the Token
1. Copy the JWT token from the login response
2. Click the "Authorize" button in Swagger UI
3. Enter: `Bearer <your-token-here>`
4. Click "Authorize"

## 📝 Test Companies

If seed data was executed, these companies exist:
1. **Tech Innovations Inc** - Domain: techinnovations.com
2. **Digital Solutions Ltd** - Domain: digitalsolutions.com
3. **StartUp Hub** - Domain: startuphub.com

## 🔧 Run Seed Data (If Needed)

If the default users don't exist, you can run the seed data:

```bash
# Connect to SQL Server using sqlcmd
docker exec -it taskmanagement-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "SecureTask2025#@!" -i /path/to/05-SeedData.sql
```

Or use any SQL client to execute the seed data script located at:
`/Database/Scripts/05-SeedData.sql`

## 💡 Password Requirements
- Minimum 6 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character

## 🎯 Recommended Testing Flow
1. Try logging in with `superadmin@taskmanagement.com` / `Admin@123`
2. If that doesn't work, create a new user via registration
3. Use SuperAdmin role for full access during testing
4. Create additional users with different roles as needed
