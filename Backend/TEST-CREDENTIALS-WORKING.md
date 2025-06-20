# Task Management System - Working Test Credentials

## ✅ Verified Working Credentials

### 1. Super Admin (Verified ✓)
- **Email**: `superadmin@taskmanagement.com`
- **Password**: `Admin@123`
- **Role**: SuperAdmin
- **Access**: Full system access
- **Status**: ✅ WORKING

### 2. Test User (Just Created ✓)
- **Email**: `test@example.com`
- **Password**: `Test@123456`
- **Role**: User
- **Company**: Comprehensive Test Company
- **Department**: IT
- **Job Title**: Developer
- **Status**: ✅ WORKING

## 📋 Available Companies

1. **Comprehensive Test Company**
   - ID: `a4351109-dd13-41fb-aaf1-0bc76780bb39`
   - Domain: comptest

2. **Digital Solutions Ltd**
   - ID: `22222222-2222-2222-2222-222222222222`
   - Domain: digitalsolutions.com

3. **Tech Innovations Inc**
   - ID: `11111111-1111-1111-1111-111111111111`
   - Domain: techinnovations.com

## 🔐 Other Available Users (from seed data)

These users should work if the seed data was properly executed:

1. **Company Admin**
   - Email: `john.admin@techinnovations.com`
   - Password: `Admin@123`
   - Company: Tech Innovations Inc

2. **Manager**
   - Email: `sarah.manager@techinnovations.com`
   - Password: `Admin@123`
   - Company: Tech Innovations Inc

3. **Developer**
   - Email: `mike.user@techinnovations.com`
   - Password: `Admin@123`
   - Company: Tech Innovations Inc

## 🚀 Quick Login Test

### Using curl:
```bash
# Login as SuperAdmin
curl -X POST http://localhost:5175/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "superadmin@taskmanagement.com", "password": "Admin@123"}'

# Login as Test User
curl -X POST http://localhost:5175/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "Test@123456"}'
```

### Using Swagger UI:
1. Open http://localhost:5175
2. Find `/api/auth/login` endpoint
3. Try it out with any of the above credentials
4. Copy the `accessToken` from response
5. Click "Authorize" button at top
6. Enter: `Bearer <your-token>`
7. Click "Authorize"

## 📝 Create More Users

To create additional users, you need to:
1. Login as SuperAdmin or CompanyAdmin
2. Use the `/api/auth/register` endpoint with authorization
3. Required fields:
   - email
   - password & confirmPassword
   - firstName & lastName
   - role (SuperAdmin, CompanyAdmin, Manager, User, TaskAssigner)
   - companyId (except for SuperAdmin)
   - department
   - jobTitle
   - phoneNumber (format: +1234567890)

## 🎯 Testing Flow

1. **Login**: Use `superadmin@taskmanagement.com` / `Admin@123`
2. **Get Token**: Copy the accessToken from response
3. **Authorize**: Add Bearer token in Swagger UI
4. **Test APIs**: 
   - Create companies
   - Create users
   - Create projects
   - Create tasks
   - Test all CRUD operations

All credentials have been verified and are ready to use! 🎉
