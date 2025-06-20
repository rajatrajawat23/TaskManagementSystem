# Task Management System - API Endpoints Overview

## 🌐 Base URL: http://localhost:5175

## 📚 API Documentation (Swagger UI)
- **URL**: http://localhost:5175
- **Description**: Interactive API documentation where you can test all endpoints

## 🔐 Authentication Controller (`/api/auth`)
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh-token` - Refresh JWT token
- `POST /api/auth/logout` - User logout
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password with token
- `GET /api/auth/verify-email/{token}` - Verify email address
- `POST /api/auth/change-password` - Change user password

## 📋 Task Controller (`/api/task`)
- `GET /api/task` - Get all tasks (with pagination & filtering)
- `GET /api/task/{id}` - Get task by ID
- `POST /api/task` - Create new task
- `PUT /api/task/{id}` - Update task
- `DELETE /api/task/{id}` - Delete task
- `POST /api/task/{id}/assign` - Assign task to user
- `PUT /api/task/{id}/status` - Update task status
- `GET /api/task/calendar/{year}/{month}` - Calendar view
- `GET /api/task/user/{userId}` - Get user's tasks
- `GET /api/task/overdue` - Get overdue tasks
- `POST /api/task/{id}/duplicate` - Duplicate task
- `GET /api/task/statistics` - Task statistics

## 👥 User Controller (`/api/user`)
- `GET /api/user` - Get all users
- `GET /api/user/{id}` - Get user by ID
- `PUT /api/user/{id}` - Update user
- `DELETE /api/user/{id}` - Delete user
- `PUT /api/user/{id}/status` - Update user status
- `PUT /api/user/{id}/role` - Update user role
- `GET /api/user/profile` - Get current user profile
- `PUT /api/user/profile` - Update current user profile

## 🏢 Company Controller (`/api/company`)
- `GET /api/company` - Get all companies
- `GET /api/company/{id}` - Get company by ID
- `POST /api/company` - Create new company
- `PUT /api/company/{id}` - Update company
- `DELETE /api/company/{id}` - Delete company
- `GET /api/company/{id}/users` - Get company users
- `GET /api/company/{id}/statistics` - Get company statistics

## 👤 Client Controller (`/api/client`)
- `GET /api/client` - Get all clients
- `GET /api/client/{id}` - Get client by ID
- `POST /api/client` - Create new client
- `PUT /api/client/{id}` - Update client
- `DELETE /api/client/{id}` - Delete client
- `GET /api/client/{id}/tasks` - Get client's tasks

## 📁 Project Controller (`/api/project`)
- `GET /api/project` - Get all projects
- `GET /api/project/{id}` - Get project by ID
- `POST /api/project` - Create new project
- `PUT /api/project/{id}` - Update project
- `DELETE /api/project/{id}` - Delete project
- `GET /api/project/{id}/tasks` - Get project tasks
- `POST /api/project/{id}/team/add` - Add team member
- `DELETE /api/project/{id}/team/remove` - Remove team member

## 📊 Dashboard Controller (`/api/dashboard`)
- `GET /api/dashboard/summary` - Get dashboard summary
- `GET /api/dashboard/task-statistics` - Get task statistics
- `GET /api/dashboard/user-performance` - Get user performance metrics
- `GET /api/dashboard/recent-activities` - Get recent activities

## 🔧 Diagnostics Controller (`/api/diagnostics`)
- `GET /api/diagnostics` - System diagnostics and status
- `GET /api/diagnostics/database` - Database connection status
- `GET /api/diagnostics/services` - Service health status

## 🩺 Health Check
- `GET /health` - Application health status

## 🔔 Real-time Notifications (SignalR)
- **Hub URL**: `/notificationHub`
- **Features**: Real-time task updates, notifications, and chat

## 📝 Testing Tips:

1. **First Step**: Open http://localhost:5175 in your browser to access Swagger UI
2. **Create Test User**: Use `/api/auth/register` to create a test user
3. **Login**: Use `/api/auth/login` to get JWT token
4. **Authorize**: Click "Authorize" button in Swagger UI and enter: `Bearer {your-token}`
5. **Test CRUD**: Try creating companies, users, projects, and tasks

## 🔑 Default Roles:
- SuperAdmin - Full system access
- CompanyAdmin - Company management
- Manager - Team and task management
- User - Basic task access
- TaskAssigner - Can assign tasks

## 💡 Quick Test Sequence:
1. Register a SuperAdmin user
2. Login to get token
3. Create a company
4. Create users for that company
5. Create projects and clients
6. Create and assign tasks

The API is ready for testing! All endpoints are documented in Swagger UI.
