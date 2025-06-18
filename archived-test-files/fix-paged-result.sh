#!/bin/bash

# Fix ClientService
echo "Fixing ClientService..."
sed -i '' 's/PagedResponseDto<ClientResponseDto>/PagedResult<ClientResponseDto>/g' Backend/TaskManagement.API/Services/Implementation/ClientService.cs
sed -i '' 's/new PagedResponseDto<ClientResponseDto>/new PagedResult<ClientResponseDto>/g' Backend/TaskManagement.API/Services/Implementation/ClientService.cs
sed -i '' 's/Data = /Items = /g' Backend/TaskManagement.API/Services/Implementation/ClientService.cs

# Fix UserService
echo "Fixing UserService..."
sed -i '' 's/PagedResponseDto<UserResponseDto>/PagedResult<UserResponseDto>/g' Backend/TaskManagement.API/Services/Implementation/UserService.cs
sed -i '' 's/new PagedResponseDto<UserResponseDto>/new PagedResult<UserResponseDto>/g' Backend/TaskManagement.API/Services/Implementation/UserService.cs
sed -i '' 's/Data = /Items = /g' Backend/TaskManagement.API/Services/Implementation/UserService.cs

# Fix ProjectService
echo "Fixing ProjectService..."
sed -i '' 's/PagedResponseDto<ProjectResponseDto>/PagedResult<ProjectResponseDto>/g' Backend/TaskManagement.API/Services/Implementation/ProjectService.cs
sed -i '' 's/new PagedResponseDto<ProjectResponseDto>/new PagedResult<ProjectResponseDto>/g' Backend/TaskManagement.API/Services/Implementation/ProjectService.cs
sed -i '' 's/Data = /Items = /g' Backend/TaskManagement.API/Services/Implementation/ProjectService.cs

echo "Done!"
