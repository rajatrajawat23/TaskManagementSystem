#!/bin/bash

# 🔍 Service Database Operations Analyzer
# This script analyzes all services to extract:
# 1. Service methods and their parameters
# 2. Database operations (queries, inserts, updates, deletes)
# 3. Entity relationships and joins
# 4. Return types and DTOs

OUTPUT_FILE="service-database-analysis.md"

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Initialize report
cat > "$OUTPUT_FILE" << EOF
# 📊 Service Database Operations Analysis
Generated on: $(date)

## Overview
This document provides detailed analysis of all service methods including:
- Method signatures and parameters
- Database queries and operations
- Entity relationships
- Return types and DTOs

---

EOF

# Function to analyze a service file
analyze_service() {
    local service_file=$1
    local service_name=$(basename "$service_file" .cs)
    
    echo -e "${BLUE}Analyzing $service_name...${NC}"
    
    echo "## $service_name" >> "$OUTPUT_FILE"
    echo "" >> "$OUTPUT_FILE"
    
    # Extract the class content
    local in_method=false
    local method_name=""
    local method_signature=""
    local method_content=""
    local brace_count=0
    
    while IFS= read -r line; do
        # Check for public method start
        if [[ $line =~ ^[[:space:]]*public[[:space:]]+(async[[:space:]]+)?.*\( ]]; then
            if [ "$in_method" = true ] && [ -n "$method_name" ]; then
                # Process previous method
                process_method "$method_name" "$method_signature" "$method_content"
            fi
            
            in_method=true
            method_signature=$(echo "$line" | sed 's/^[[:space:]]*//' | sed 's/{$//')
            method_name=$(echo "$line" | grep -oE '\b\w+\s*\(' | sed 's/[[:space:]]*(//')
            method_content=""
            brace_count=0
            
            # Count braces in the signature line
            brace_count=$((brace_count + $(echo "$line" | grep -o '{' | wc -l)))
            brace_count=$((brace_count - $(echo "$line" | grep -o '}' | wc -l)))
        elif [ "$in_method" = true ]; then
            method_content+="$line"$'\n'
            
            # Count braces
            brace_count=$((brace_count + $(echo "$line" | grep -o '{' | wc -l)))
            brace_count=$((brace_count - $(echo "$line" | grep -o '}' | wc -l)))
            
            # Check if method ended
            if [ $brace_count -eq 0 ] && [[ $line =~ \} ]]; then
                process_method "$method_name" "$method_signature" "$method_content"
                in_method=false
                method_name=""
                method_signature=""
                method_content=""
            fi
        fi
    done < "$service_file"
    
    # Process last method if file ends
    if [ "$in_method" = true ] && [ -n "$method_name" ]; then
        process_method "$method_name" "$method_signature" "$method_content"
    fi
    
    echo "" >> "$OUTPUT_FILE"
}

# Function to process a method and extract database operations
process_method() {
    local method_name=$1
    local method_signature=$2
    local method_content=$3
    
    echo "### Method: $method_name" >> "$OUTPUT_FILE"
    echo "\`\`\`csharp" >> "$OUTPUT_FILE"
    echo "$method_signature" >> "$OUTPUT_FILE"
    echo "\`\`\`" >> "$OUTPUT_FILE"
    echo "" >> "$OUTPUT_FILE"
    
    # Extract parameters
    local params=$(echo "$method_signature" | grep -oE '\([^)]*\)' | sed 's/[()]//g')
    if [ -n "$params" ]; then
        echo "**Parameters:**" >> "$OUTPUT_FILE"
        echo "$params" | tr ',' '\n' | while read -r param; do
            param=$(echo "$param" | xargs)
            if [ -n "$param" ]; then
                echo "- $param" >> "$OUTPUT_FILE"
            fi
        done
        echo "" >> "$OUTPUT_FILE"
    fi
    
    # Extract return type
    local return_type=$(echo "$method_signature" | grep -oE '(Task<[^>]+>|Task|void|[A-Za-z]+<[^>]+>|[A-Za-z]+)\s+\w+\s*\(' | sed 's/[[:space:]]*\w*[[:space:]]*(//')
    if [ -n "$return_type" ]; then
        echo "**Return Type:** \`$return_type\`" >> "$OUTPUT_FILE"
        echo "" >> "$OUTPUT_FILE"
    fi
    
    # Extract database operations
    echo "**Database Operations:**" >> "$OUTPUT_FILE"
    
    # Check for various database operations
    if echo "$method_content" | grep -qE '_context\.|_dbContext\.'; then
        # Entity Framework operations
        echo "$method_content" | grep -E '_context\.|_dbContext\.' | while read -r db_line; do
            db_line=$(echo "$db_line" | xargs)
            if [ -n "$db_line" ]; then
                echo "- \`$db_line\`" >> "$OUTPUT_FILE"
            fi
        done
    fi
    
    # Check for LINQ queries
    if echo "$method_content" | grep -qE '\.Where\(|\.Include\(|\.FirstOrDefault\(|\.ToList\(|\.Any\(|\.Count\(|\.OrderBy\(|\.Select\('; then
        echo "" >> "$OUTPUT_FILE"
        echo "**LINQ Operations:**" >> "$OUTPUT_FILE"
        echo "$method_content" | grep -E '\.Where\(|\.Include\(|\.FirstOrDefault\(|\.ToList\(|\.Any\(|\.Count\(|\.OrderBy\(|\.Select\(' | while read -r linq_line; do
            linq_line=$(echo "$linq_line" | xargs)
            if [ -n "$linq_line" ]; then
                echo "- \`$linq_line\`" >> "$OUTPUT_FILE"
            fi
        done
    fi
    
    # Check for SaveChanges
    if echo "$method_content" | grep -qE 'SaveChanges|SaveChangesAsync'; then
        echo "" >> "$OUTPUT_FILE"
        echo "**Persistence:** ✅ Calls SaveChangesAsync" >> "$OUTPUT_FILE"
    fi
    
    echo "" >> "$OUTPUT_FILE"
    echo "---" >> "$OUTPUT_FILE"
    echo "" >> "$OUTPUT_FILE"
}

# Function to extract DTOs and models
extract_dtos_and_models() {
    echo -e "\n${BLUE}Extracting DTOs and Models...${NC}"
    
    echo "## DTOs and Models" >> "$OUTPUT_FILE"
    echo "" >> "$OUTPUT_FILE"
    
    # Check Request DTOs
    if [ -d "/Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Models/DTOs/Request" ]; then
        echo "### Request DTOs" >> "$OUTPUT_FILE"
        for dto in /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Models/DTOs/Request/*.cs; do
            if [ -f "$dto" ]; then
                dto_name=$(basename "$dto" .cs)
                echo "- $dto_name" >> "$OUTPUT_FILE"
            fi
        done
        echo "" >> "$OUTPUT_FILE"
    fi
    
    # Check Response DTOs
    if [ -d "/Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Models/DTOs/Response" ]; then
        echo "### Response DTOs" >> "$OUTPUT_FILE"
        for dto in /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Models/DTOs/Response/*.cs; do
            if [ -f "$dto" ]; then
                dto_name=$(basename "$dto" .cs)
                echo "- $dto_name" >> "$OUTPUT_FILE"
            fi
        done
        echo "" >> "$OUTPUT_FILE"
    fi
}

# Function to check database context configuration
check_db_context() {
    echo -e "\n${BLUE}Checking Database Context...${NC}"
    
    echo "## Database Context Configuration" >> "$OUTPUT_FILE"
    echo "" >> "$OUTPUT_FILE"
    
    local db_context="/Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.Infrastructure/Data/TaskManagementDbContext.cs"
    
    if [ -f "$db_context" ]; then
        echo "### DbSets (Tables)" >> "$OUTPUT_FILE"
        grep -E 'public\s+DbSet<.*>\s+\w+\s*{\s*get;\s*set;\s*}' "$db_context" | while read -r dbset; do
            echo "- $dbset" >> "$OUTPUT_FILE"
        done
        echo "" >> "$OUTPUT_FILE"
    fi
}

# Main execution
main() {
    echo -e "${GREEN}🚀 Starting Service Database Analysis...${NC}"
    
    # Analyze each service
    for service in /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Services/Implementation/*.cs; do
        if [ -f "$service" ] && [[ ! $(basename "$service") =~ \.backup$ ]]; then
            analyze_service "$service"
        fi
    done
    
    # Extract DTOs and models
    extract_dtos_and_models
    
    # Check database context
    check_db_context
    
    echo -e "\n${GREEN}✅ Analysis complete!${NC}"
    echo -e "${BLUE}📊 Report saved to: $OUTPUT_FILE${NC}"
}

# Run main function
main
