# SQLServerless

## Enable Change Tracking 

1. Enable change tracking for DB
```sql
ALTER DATABASE <DB name>  
SET CHANGE_TRACKING = ON  
(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)
```

2. Enable change tracking for table
```sql
ALTER TABLE <table name>  
ENABLE CHANGE_TRACKING  
WITH (TRACK_COLUMNS_UPDATED = ON)
```

## Disable Change Tracking 

1. Enable change tracking for DB
```sql
ALTER DATABASE <DB name>  
SET CHANGE_TRACKING = OFF  
```

2. Disable change tracking for table
```sql
ALTER TABLE <table name>  
DISABLE CHANGE_TRACKING; 
```
  