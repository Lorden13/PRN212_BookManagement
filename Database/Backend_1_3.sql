USE [Project_PRN];
GO

-- NULL = Pending, 1 = Approved, 0 = Rejected.
-- Resolve the generated default-constraint name dynamically.
DECLARE @StatusDefaultConstraint sysname;

SELECT @StatusDefaultConstraint = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c
  ON c.object_id = dc.parent_object_id
 AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Books')
  AND c.name = N'Status';

IF @StatusDefaultConstraint IS NOT NULL
    EXEC(N'ALTER TABLE dbo.Books DROP CONSTRAINT ' + QUOTENAME(@StatusDefaultConstraint));

IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Books')
      AND name = N'Status'
      AND is_nullable = 0
)
    ALTER TABLE dbo.Books ALTER COLUMN Status bit NULL;

IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    JOIN sys.columns c
      ON c.object_id = dc.parent_object_id
     AND c.column_id = dc.parent_column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Books')
      AND c.name = N'Status'
)
    ALTER TABLE dbo.Books ADD CONSTRAINT DF_Books_Status DEFAULT NULL FOR Status;
GO

-- Prevent duplicate purchases caused by concurrent requests.
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Purchases')
      AND name = N'UX_Purchases_Reader_Book'
)
    CREATE UNIQUE INDEX UX_Purchases_Reader_Book
    ON dbo.Purchases(ReaderID, BookID);
GO
