# 🎯 Complete Business Workflow Guide

## 📋 Overview
This guide shows you **exactly** how your Task Management System works from start to finish - from setting up a new company to completing tasks.

---

## 🚀 Phase 1: System Setup (One-time)

### **SuperAdmin Sets Up Platform**
```bash
# 1. SuperAdmin logs in (already exists or created via register-test-user)
POST /api/Auth/login
{
  "email": "superadmin@system.com",
  "password": "SuperAdmin123!"
}

# 2. SuperAdmin creates new company
POST /api/Company
{
  "name": "Acme Corporation",
  "domain": "acme.com", 
  "contactEmail": "contact@acme.com",
  "subscriptionType": "Premium",
  "maxUsers": 100
}
# → Returns companyId

# 3. SuperAdmin creates Company Admin for the new company
POST /api/Auth/register
{
  "email": "admin@acme.com",
  "password": "CompanyAdmin123!",
  "firstName": "John",
  "lastName": "Smith",
  "role": "CompanyAdmin",
  "companyId": "company-guid-here"
}
```

---

## 👑 Phase 2: Company Admin Sets Up Team

### **Company Admin Login and Team Creation**
```bash
# 1. Company Admin logs in
POST /api/Auth/login
{
  "email": "admin@acme.com", 
  "password": "CompanyAdmin123!"
}
# → Returns JWT token with CompanyId

# 2. Create Department Managers
POST /api/User
{
  "email": "engineering.manager@acme.com",
  "password": "Manager123!",
  "firstName": "Sarah",
  "lastName": "Johnson", 
  "role": "Manager",
  "department": "Engineering",
  "jobTitle": "Engineering Manager"
}

POST /api/User  
{
  "email": "marketing.manager@acme.com",
  "password": "Manager123!",
  "firstName": "David",
  "lastName": "Wilson",
  "role": "Manager", 
  "department": "Marketing",
  "jobTitle": "Marketing Manager"
}

# 3. Create Regular Employees
POST /api/User
{
  "email": "developer1@acme.com",
  "password": "Developer123!",
  "firstName": "Mike",
  "lastName": "Developer",
  "role": "User",
  "department": "Engineering", 
  "jobTitle": "Senior Developer"
}

POST /api/User
{
  "email": "designer@acme.com", 
  "password": "Designer123!",
  "firstName": "Lisa",
  "lastName": "Designer",
  "role": "User",
  "department": "Design",
  "jobTitle": "UI/UX Designer"
}

# 4. Create Task Coordinators
POST /api/User
{
  "email": "coordinator@acme.com",
  "password": "Coordinator123!", 
  "firstName": "Emma",
  "lastName": "Coordinator",
  "role": "TaskAssigner",
  "department": "Operations",
  "jobTitle": "Project Coordinator"
}
```

---

## 👨‍💼 Phase 3: Manager Sets Up Projects

### **Engineering Manager Creates Client and Project**
```bash
# 1. Manager logs in
POST /api/Auth/login
{
  "email": "engineering.manager@acme.com",
  "password": "Manager123!"
}

# 2. Create Client
POST /api/Client
{
  "name": "TechStart Inc",
  "email": "contact@techstart.com",
  "phone": "+1-555-0199",
  "contactPerson": "Robert Tech",
  "industry": "Technology",
  "notes": "New startup client, mobile app project"
}
# → Returns clientId

# 3. Create Project
POST /api/Project
{
  "name": "Mobile App Development",
  "description": "iOS and Android app for client's business",
  "clientId": "client-guid-here",
  "managerId": "manager-user-guid", 
  "startDate": "2025-06-21T00:00:00Z",
  "endDate": "2025-09-30T23:59:59Z",
  "budget": 75000.00,
  "status": "Active"
}
# → Returns projectId, auto-generates ProjectCode: PRJ-2025-0001

# 4. Add Team Members to Project
POST /api/Project/{projectId}/team-member
{
  "userId": "developer1-guid",
  "role": "Lead Developer", 
  "notes": "iOS development lead"
}

POST /api/Project/{projectId}/team-member
{
  "userId": "designer-guid",
  "role": "UI Designer",
  "notes": "App interface design"
}
```

---

## 📝 Phase 4: Task Creation and Assignment

### **Manager Creates Development Tasks**
```bash
# 1. Create main development tasks
POST /api/Task
{
  "title": "Design App Architecture",
  "description": "Create technical architecture for iOS and Android apps",
  "assignedToId": "developer1-guid",
  "projectId": "project-guid",
  "clientId": "client-guid", 
  "priority": "High",
  "category": "Architecture",
  "estimatedHours": 20.0,
  "startDate": "2025-06-21T09:00:00Z",
  "dueDate": "2025-06-28T17:00:00Z",
  "tags": ["architecture", "planning", "ios", "android"]
}
# → Returns taskId, auto-generates TaskNumber: TSK-2025-0001

POST /api/Task
{
  "title": "Create UI Mockups", 
  "description": "Design user interface mockups for all app screens",
  "assignedToId": "designer-guid",
  "projectId": "project-guid",
  "clientId": "client-guid",
  "priority": "High", 
  "category": "Design",
  "estimatedHours": 30.0,
  "startDate": "2025-06-21T09:00:00Z", 
  "dueDate": "2025-07-05T17:00:00Z",
  "tags": ["ui", "design", "mockups"]
}
# → TSK-2025-0002

POST /api/Task
{
  "title": "Implement User Authentication",
  "description": "Build login/register functionality with JWT",
  "assignedToId": "developer1-guid", 
  "projectId": "project-guid",
  "clientId": "client-guid",
  "priority": "Medium",
  "category": "Development",
  "estimatedHours": 15.0,
  "startDate": "2025-06-28T09:00:00Z",
  "dueDate": "2025-07-08T17:00:00Z", 
  "tags": ["authentication", "security", "backend"]
}
# → TSK-2025-0003
```

### **Task Coordinator Creates Additional Tasks**
```bash
# 1. Task Coordinator logs in
POST /api/Auth/login
{
  "email": "coordinator@acme.com",
  "password": "Coordinator123!"
}

# 2. Create supporting tasks
POST /api/Task
{
  "title": "Update Company Website",
  "description": "Add new client project to portfolio section", 
  "assignedToId": "designer-guid",
  "priority": "Low",
  "category": "Marketing",
  "estimatedHours": 4.0,
  "dueDate": "2025-06-25T17:00:00Z",
  "tags": ["website", "portfolio", "marketing"]
}
# → TSK-2025-0004

POST /api/Task
{
  "title": "Prepare Project Documentation",
  "description": "Create project brief and requirements document",
  "assignedToId": "coordinator-guid",
  "projectId": "project-guid", 
  "priority": "Medium",
  "category": "Documentation",
  "estimatedHours": 8.0,
  "dueDate": "2025-06-30T17:00:00Z",
  "tags": ["documentation", "requirements"]
}
# → TSK-2025-0005
```

---

## 👨‍💻 Phase 5: Employees Work on Tasks

### **Developer Updates Task Progress**
```bash
# 1. Developer logs in
POST /api/Auth/login
{
  "email": "developer1@acme.com",
  "password": "Developer123!"
}

# 2. View assigned tasks
GET /api/User/profile
GET /api/User/{userId}/tasks

# 3. Start working on architecture task
PUT /api/Task/{architecture-task-id}/status
{
  "status": "InProgress",
  "progress": 25,
  "actualHours": 5.0,
  "notes": "Started research on React Native vs native development"
}

# 4. Add progress comment
POST /api/Task/{architecture-task-id}/comment
{
  "comment": "Completed technology research. Recommending React Native for faster development.",
  "isPublic": true
}

# 5. Continue progress updates
PUT /api/Task/{architecture-task-id}/status
{
  "status": "InProgress", 
  "progress": 75,
  "actualHours": 15.0,
  "notes": "Architecture document 75% complete, need client feedback on database structure"
}

# 6. Complete task
PUT /api/Task/{architecture-task-id}/status
{
  "status": "Completed",
  "progress": 100,
  "actualHours": 18.0,
  "notes": "Architecture complete and approved by client"
}
```

### **Designer Updates UI Task**
```bash
# 1. Designer logs in
POST /api/Auth/login
{
  "email": "designer@acme.com",
  "password": "Designer123!"
}

# 2. Update mockup task progress
PUT /api/Task/{mockup-task-id}/status
{
  "status": "InProgress",
  "progress": 50,
  "actualHours": 15.0,
  "notes": "Completed mockups for login/register screens"
}

# 3. Upload design files
POST /api/Task/{mockup-task-id}/attachment
# Upload design files (Figma, images, etc.)

# 4. Add comment with client feedback
POST /api/Task/{mockup-task-id}/comment
{
  "comment": "Client loves the color scheme but wants larger buttons for better accessibility",
  "isPublic": true
}

# 5. Complete design task
PUT /api/Task/{mockup-task-id}/status
{
  "status": "Completed",
  "progress": 100, 
  "actualHours": 28.0,
  "notes": "All mockups completed and approved by client"
}
```

---

## 📊 Phase 6: Monitoring and Management

### **Manager Monitors Project Progress**
```bash
# 1. View project statistics
GET /api/Project/{projectId}/statistics
# Returns: total tasks, completed, in progress, overdue, budget utilization

# 2. View project tasks
GET /api/Project/{projectId}/tasks

# 3. Check team performance
GET /api/Dashboard/user-performance

# 4. View overdue tasks
GET /api/Task/overdue

# 5. Reassign task if needed
POST /api/Task/{taskId}/assign
{
  "assignedToId": "different-user-guid",
  "notes": "Reassigning due to workload rebalancing",
  "dueDate": "2025-07-10T17:00:00Z"
}
```

### **Company Admin Reviews Company Performance**
```bash
# 1. View company dashboard
GET /api/Dashboard/company-overview
# Returns: total users, projects, tasks, completion rates

# 2. View all company users
GET /api/User?pageNumber=1&pageSize=50

# 3. Check company statistics
GET /api/Company/{companyId}/statistics

# 4. View recent activities
GET /api/Dashboard/recent-activities

# 5. Promote high performer
PUT /api/User/{userId}/role
{
  "role": "Manager",
  "notes": "Promoted due to excellent performance on mobile app project"
}
```

---

## 🔔 Phase 7: Notifications and Communication

### **Real-time Notifications (Automatic)**
- Task assignments → User gets notified
- Status updates → Manager gets notified  
- Overdue tasks → All stakeholders notified
- Project milestones → Team gets notified

### **Manual Notification Checks**
```bash
# View notifications
GET /api/Notification

# Check unread count
GET /api/Notification/unread-count

# Mark as read
PUT /api/Notification/{notificationId}/read

# Mark all as read
PUT /api/Notification/read-all
```

---

## 🎯 Complete Workflow Summary

### **What Happens in Your System:**

1. **Setup Phase** (Once)
   - SuperAdmin creates company → Company Admin created
   - Company Admin creates team (Managers, Users, TaskAssigners)

2. **Project Phase** (Per Project)
   - Manager creates client → Manager creates project → Manager adds team members

3. **Task Phase** (Daily Work)
   - Manager/TaskAssigner creates tasks → Tasks assigned to users → Users work and update progress

4. **Monitoring Phase** (Ongoing)
   - Real-time dashboards → Progress tracking → Performance reviews → Role promotions

### **Key Features Working:**
- ✅ **Multi-tenant isolation** (each company's data separate)
- ✅ **Role-based access** (each role sees appropriate data)
- ✅ **Auto-generated codes** (PRJ-2025-0001, TSK-2025-0001)
- ✅ **Real-time notifications** (SignalR integration)
- ✅ **File uploads** (task attachments, avatars)
- ✅ **Comprehensive tracking** (time, progress, comments)

### **Your System is Production-Ready** 🚀
- Complete user management
- Full project lifecycle
- Task assignment and tracking
- Performance monitoring
- Security and multi-tenancy
- Real-time notifications
- File management
- Dashboard analytics

**Ready for your clients to start using immediately!**
