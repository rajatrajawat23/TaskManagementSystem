#!/bin/bash

# 🔍 Extract Database Operations from Services
# This script extracts all database operations from service files

OUTPUT_FILE="database-operations-detailed.md"

# Initialize report
cat > "$OUTPUT_FILE" << EOF
# 📊 Detailed Database Operations in Services
Generated on: $(date)

## Summary
This document shows all database operations performed by each service, including:
- Entity queries and filters
- Include statements (joins)
- Add, Update, Delete operations
- SaveChanges calls

---

EOF

# Function to extract database operations from a service
extract_db_operations() {
    local service_file=$1
    local service_name=$(basename "$service_file" .cs)
    
    if [[ "$service_name" =~ \.backup$ ]]; then
        return
    fi
    
    echo "## $service_name" >> "$OUTPUT_FILE"
    echo "" >> "$OUTPUT_FILE"
    
    # Extract all lines with database context operations
    echo "### Database Context Operations:" >> "$OUTPUT_FILE"
    grep -n -E '_context\.|_dbContext\.' "$service_file" | while IFS=: read -r line_num line_content; do
        # Clean up the line
        line_content=$(echo "$line_content" | sed 's/^[[:space:]]*//')
        
        # Categorize the operation
        if echo "$line_content" | grep -qE '\.Add\(|\.AddAsync\(|\.AddRange\('; then
            echo "**Line $line_num - CREATE:** \`$line_content\`" >> "$OUTPUT_FILE"
        elif echo "$line_content" | grep -qE '\.Update\(|\.UpdateRange\('; then
            echo "**Line $line_num - UPDATE:** \`$line_content\`" >> "$OUTPUT_FILE"
        elif echo "$line_content" | grep -qE '\.Remove\(|\.RemoveRange\(|\.Delete\('; then
            echo "**Line $line_num - DELETE:** \`$line_content\`" >> "$OUTPUT_FILE"
        elif echo "$line_content" | grep -qE '\.Find\(|\.FindAsync\(|\.FirstOrDefault\(|\.SingleOrDefault\(|\.Where\('; then
            echo "**Line $line_num - QUERY:** \`$line_content\`" >> "$OUTPUT_FILE"
        elif echo "$line_content" | grep -qE '\.Include\(|\.ThenInclude\('; then
            echo "**Line $line_num - JOIN:** \`$line_content\`" >> "$OUTPUT_FILE"
        elif echo "$line_content" | grep -qE 'SaveChanges|SaveChangesAsync'; then
            echo "**Line $line_num - SAVE:** \`$line_content\`" >> "$OUTPUT_FILE"
        else
            echo "**Line $line_num - OTHER:** \`$line_content\`" >> "$OUTPUT_FILE"
        fi
    done
    
    echo "" >> "$OUTPUT_FILE"
    
    # Extract LINQ operations
    echo "### LINQ Operations:" >> "$OUTPUT_FILE"
    grep -n -E '\.Where\(|\.Select\(|\.OrderBy\(|\.OrderByDescending\(|\.GroupBy\(|\.Any\(|\.All\(|\.Count\(|\.Sum\(|\.Average\(|\.Min\(|\.Max\(|\.Take\(|\.Skip\(|\.ToList\(|\.ToArray\(|\.AsNoTracking\(' "$service_file" | while IFS=: read -r line_num line_content; do
        line_content=$(echo "$line_content" | sed 's/^[[:space:]]*//')
        echo "**Line $line_num:** \`$line_content\`" >> "$OUTPUT_FILE"
    done
    
    echo "" >> "$OUTPUT_FILE"
    
    # Count operations
    local create_count=$(grep -cE '\.Add\(|\.AddAsync\(|\.AddRange\(' "$service_file" || echo 0)
    local read_count=$(grep -cE '\.Find\(|\.FindAsync\(|\.FirstOrDefault\(|\.SingleOrDefault\(|\.Where\(|\.ToList\(' "$service_file" || echo 0)
    local update_count=$(grep -cE '\.Update\(|\.UpdateRange\(' "$service_file" || echo 0)
    local delete_count=$(grep -cE '\.Remove\(|\.RemoveRange\(|\.Delete\(' "$service_file" || echo 0)
    local save_count=$(grep -cE 'SaveChanges|SaveChangesAsync' "$service_file" || echo 0)
    
    echo "### Operation Count Summary:" >> "$OUTPUT_FILE"
    echo "- **CREATE operations:** $create_count" >> "$OUTPUT_FILE"
    echo "- **READ operations:** $read_count" >> "$OUTPUT_FILE"
    echo "- **UPDATE operations:** $update_count" >> "$OUTPUT_FILE"
    echo "- **DELETE operations:** $delete_count" >> "$OUTPUT_FILE"
    echo "- **SAVE operations:** $save_count" >> "$OUTPUT_FILE"
    
    echo "" >> "$OUTPUT_FILE"
    echo "---" >> "$OUTPUT_FILE"
    echo "" >> "$OUTPUT_FILE"
}

# Main execution
echo "🚀 Extracting database operations from all services..."

for service in /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Services/Implementation/*.cs; do
    if [ -f "$service" ]; then
        extract_db_operations "$service"
    fi
done

# Add summary section
echo "## Overall Summary" >> "$OUTPUT_FILE"
echo "" >> "$OUTPUT_FILE"

# Count total operations across all services
total_creates=0
total_reads=0
total_updates=0
total_deletes=0
total_saves=0

for service in /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Services/Implementation/*.cs; do
    if [ -f "$service" ] && [[ ! $(basename "$service") =~ \.backup$ ]]; then
        creates=$(grep -cE '\.Add\(|\.AddAsync\(|\.AddRange\(' "$service" 2>/dev/null || echo 0)
        reads=$(grep -cE '\.Find\(|\.FindAsync\(|\.FirstOrDefault\(|\.SingleOrDefault\(|\.Where\(|\.ToList\(' "$service" 2>/dev/null || echo 0)
        updates=$(grep -cE '\.Update\(|\.UpdateRange\(' "$service" 2>/dev/null || echo 0)
        deletes=$(grep -cE '\.Remove\(|\.RemoveRange\(|\.Delete\(' "$service" 2>/dev/null || echo 0)
        saves=$(grep -cE 'SaveChanges|SaveChangesAsync' "$service" 2>/dev/null || echo 0)
        
        total_creates=$((total_creates + creates))
        total_reads=$((total_reads + reads))
        total_updates=$((total_updates + updates))
        total_deletes=$((total_deletes + deletes))
        total_saves=$((total_saves + saves))
    fi
done

echo "### Total Database Operations Across All Services:" >> "$OUTPUT_FILE"
echo "- **Total CREATE operations:** $total_creates" >> "$OUTPUT_FILE"
echo "- **Total READ operations:** $total_reads" >> "$OUTPUT_FILE"
echo "- **Total UPDATE operations:** $total_updates" >> "$OUTPUT_FILE"
echo "- **Total DELETE operations:** $total_deletes" >> "$OUTPUT_FILE"
echo "- **Total SAVE operations:** $total_saves" >> "$OUTPUT_FILE"

echo "" >> "$OUTPUT_FILE"
echo "### Services Analyzed:" >> "$OUTPUT_FILE"
for service in /Users/rajatrajawat/TaskManagementSystem/Backend/TaskManagement.API/Services/Implementation/*.cs; do
    if [ -f "$service" ] && [[ ! $(basename "$service") =~ \.backup$ ]]; then
        service_name=$(basename "$service" .cs)
        echo "- $service_name" >> "$OUTPUT_FILE"
    fi
done

echo "✅ Extraction complete! Report saved to: $OUTPUT_FILE"
