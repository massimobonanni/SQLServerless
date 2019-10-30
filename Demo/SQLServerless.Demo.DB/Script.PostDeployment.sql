/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/*
	Enable Change Tracking for a Database (https://docs.microsoft.com/en-us/sql/relational-databases/track-changes/enable-and-disable-change-tracking-sql-server?view=sql-server-ver15#enable-change-tracking-for-a-database)
*/
IF NOT EXISTS (SELECT 1 
				FROM sys.change_tracking_databases 
				WHERE database_id=DB_ID(N'$(DatabaseName)'))
    BEGIN
		PRINT N'SET Change Tracking ON on Database';

        ALTER DATABASE [$(DatabaseName)]
            SET CHANGE_TRACKING = ON 
            WITH ROLLBACK IMMEDIATE;
    END

/*
	Enable Change Tracking for Contacts Table (https://docs.microsoft.com/en-us/sql/relational-databases/track-changes/enable-and-disable-change-tracking-sql-server?view=sql-server-ver15#enable-change-tracking-for-a-table) 
*/
IF NOT EXISTS (SELECT 1 
				FROM sys.change_tracking_tables 
				WHERE object_id = OBJECT_ID('dbo.Contacts'))
	BEGIN
		PRINT N'SET Change Tracking ON on Table';

        ALTER TABLE dbo.Contacts  
			ENABLE CHANGE_TRACKING  
		WITH (TRACK_COLUMNS_UPDATED = ON) 
    END
