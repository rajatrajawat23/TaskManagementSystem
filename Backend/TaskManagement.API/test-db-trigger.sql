SELECT 
    t.name AS TriggerName,
    s.name AS SchemaName,
    o.name AS TableName,
    t.create_date,
    t.modify_date,
    m.definition
FROM sys.triggers t
INNER JOIN sys.objects o ON t.parent_id = o.object_id
INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
INNER JOIN sys.sql_modules m ON t.object_id = m.object_id
WHERE o.name = 'Tasks';