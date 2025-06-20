# 👑 Company Admin Role Management Guide

## 🎯 Quick Overview
As a **Company Admin**, you have complete control over your company's users and their roles. Here's exactly how to manage roles and permissions.

---

## 🔐 Role Hierarchy in Your System

### **Available Roles (in order of authority):**

1. **CompanyAdmin** (You) 
   - ✅ Create/manage all users
   - ✅ Assign/change any role
   - ✅ View all company data
   - ✅ Manage company settings

2. **Manager**
   - ✅ Create/manage projects
   - ✅ Create/assign tasks
   - ✅ View team performance
   - ✅ Manage clients

3. **User** (Regular Employee)
   - ✅ Work on assigned tasks
   - ✅ Update task progress
   - ✅ View own tasks only

4. **TaskAssigner**
   - ✅ Create simple tasks
   - ✅ Assign tasks to users
   - ✅ View task status

---

## 🚀 Step-by-Step: How to Assign Roles

### **Step 1: Login as Company Admin**
```bash
# Use your Company Admin credentials
POST /api/Auth/login
{
  "email": "your-admin-email@company.com",
  "password": "your-password"
}
```

### **Step 2: Create New User with Role**
```bash
POST /api/User
Authorization: Bearer {your-admin-token}

{
  "email": "newemployee@company.com",
  "password": "TempPassword123!",
  "firstName": "John",
  "lastName": "Doe",
  "role": "User",           # ← Set initial role here
  "department": "Engineering",
  "jobTitle": "Software Developer"
}
```

### **Step 3: Change Existing User's Role**
```bash
PUT /api/User/{userId}/role
Authorization: Bearer {your-admin-token}

{
  "role": "Manager",        # ← New role
  "notes": "Promoted to team lead position"
}
```

### **Step 4: View All Company Users**
```bash
GET /api/User?pageNumber=1&pageSize=50
Authorization: Bearer {your-admin-token}

# Filter by role
GET /api/User?role=Manager
GET /api/User?role=User
```

---

## 🎯 Common Role Assignment Scenarios

### **Scenario 1: Promote User to Manager**
```bash
# Example: Promote a developer to engineering manager
PUT /api/User/{developer-user-id}/role
{
  "role": "Manager",
  "notes": "Promoted to Engineering Manager"
}
```

### **Scenario 2: Create Department Manager**
```bash
# Create new manager for specific department
POST /api/User
{
  "email": "sarah.manager@company.com",
  "password": "ManagerPass123!",
  "firstName": "Sarah",
  "lastName": "Johnson",
  "role": "Manager",
  "department": "Marketing",
  "jobTitle": "Marketing Manager"
}
```

### **Scenario 3: Create Task Coordinator**
```bash
# Create someone who just assigns tasks
POST /api/User
{
  "email": "coordinator@company.com",
  "password": "CoordPass123!",
  "firstName": "Lisa",
  "lastName": "Coordinator",
  "role": "TaskAssigner",
  "department": "Operations",
  "jobTitle": "Task Coordinator"
}
```

### **Scenario 4: Bulk User Creation**
Create multiple users at once using the same pattern:

1. **Developers** → Role: "User"
2. **Team Leads** → Role: "Manager"  
3. **Coordinators** → Role: "TaskAssigner"

---

## 🔧 Advanced User Management

### **Activate/Deactivate Users**
```bash
# Deactivate user (keeps data, stops access)
PUT /api/User/{userId}/deactivate

# Reactivate user
PUT /api/User/{userId}/activate
```

### **Reset User Password**
```bash
# User requests password reset
POST /api/Auth/forgot-password
{
  "email": "employee@company.com"
}
# → System sends reset email
```

### **View User's Tasks**
```bash
# See what tasks are assigned to any user
GET /api/User/{userId}/tasks
```

### **Manage User Permissions**
```bash
# View current permissions
GET /api/User/{userId}/permissions

# Update permissions
POST /api/User/{userId}/permissions
{
  "permissions": [
    "CREATE_TASKS",
    "ASSIGN_TASKS", 
    "VIEW_REPORTS",
    "MANAGE_PROJECTS"
  ]
}
```

---

## 📊 Monitor Your Team

### **View Company Overview**
```bash
GET /api/Dashboard/company-overview
# Shows: total users, projects, tasks, completion rates
```

### **View User Performance**
```bash
GET /api/Dashboard/user-performance
# Shows: top performers, completion rates, productivity trends
```

### **View All Company Data**
```bash
GET /api/User           # All your users
GET /api/Project        # All your projects  
GET /api/Task           # All your tasks
GET /api/Client         # All your clients
```

---

## 🎯 Real-World Role Assignment Examples

### **Small Team (5-10 people):**
- **1 Company Admin** (You)
- **1-2 Managers** (Project/Team leads)
- **3-7 Users** (Regular employees)
- **0-1 TaskAssigner** (Optional coordinator)

### **Medium Team (20-50 people):**
- **1 Company Admin** (You)
- **3-5 Managers** (Department heads)
- **15-40 Users** (Regular employees)
- **1-2 TaskAssigners** (Coordinators)

### **Large Team (50+ people):**
- **1-2 Company Admins** (You + Deputy)
- **5-10 Managers** (Department/Team managers)
- **40+ Users** (Regular employees)
- **2-5 TaskAssigners** (Project coordinators)

---

## ⚡ Quick Commands for Role Management

### **Get Your Admin Token**
```bash
curl -X POST http://localhost:5175/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"your-email","password":"your-password"}'
```

### **Create Manager**
```bash
curl -X POST http://localhost:5175/api/User \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"email":"manager@company.com","password":"Password123!","firstName":"Manager","lastName":"Name","role":"Manager","department":"Engineering"}'
```

### **Promote User to Manager**
```bash
curl -X PUT http://localhost:5175/api/User/USER_ID/role \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"role":"Manager","notes":"Promoted to manager"}'
```

### **View All Users**
```bash
curl -X GET http://localhost:5175/api/User \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 🚨 Important Security Notes

1. **Role Restrictions:**
   - You can only manage users in YOUR company
   - You cannot create SuperAdmins (only system SuperAdmin can)
   - Deleted users lose all access immediately

2. **Role Permissions:**
   - **Managers** can see all company data but can't manage users
   - **Users** can only see their own tasks and profile
   - **TaskAssigners** can create/assign tasks but can't manage projects

3. **Best Practices:**
   - Give minimum required role (start with "User", promote as needed)
   - Use departments to organize teams
   - Regularly audit user access (deactivate unused accounts)
   - Keep job titles updated for better organization

---

## 🎯 Summary: Your Role Assignment Workflow

1. **Login** as Company Admin
2. **Create users** with appropriate initial roles
3. **Promote/demote** users as needed using role update API
4. **Monitor performance** through dashboards
5. **Manage permissions** for fine-tuned access control
6. **Deactivate/reactivate** users as employment changes

**Your system automatically handles:**
- ✅ Multi-tenant data isolation (users only see your company's data)
- ✅ Role-based access control (each role has appropriate permissions)
- ✅ Audit trails (all changes are logged)
- ✅ Security (JWT tokens, password hashing, etc.)

You're all set to manage your team! 🚀
