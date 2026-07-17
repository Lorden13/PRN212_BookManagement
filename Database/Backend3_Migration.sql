USE [Project_PRN]
GO

SET XACT_ABORT ON
GO

BEGIN TRANSACTION

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE [object_id] = OBJECT_ID('dbo.Books')
      AND [name] = 'Status'
      AND [is_nullable] = 0
)
BEGIN
    ALTER TABLE [dbo].[Books]
        ALTER COLUMN [Status] bit NULL;
END
GO

IF COL_LENGTH('dbo.Purchases', 'Payment') IS NULL
BEGIN
    ALTER TABLE [dbo].[Purchases]
        ADD [Payment] decimal(18, 2) NULL;
END
GO

UPDATE purchase
SET [Payment] = book.[Price]
FROM [dbo].[Purchases] AS purchase
INNER JOIN [dbo].[Books] AS book ON book.[BookID] = purchase.[BookID]
WHERE purchase.[Payment] IS NULL;
GO

ALTER TABLE [dbo].[Purchases]
    ALTER COLUMN [Payment] decimal(18, 2) NOT NULL;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE [name] = 'CK_Purchases_Payment_NonNegative'
)
BEGIN
    ALTER TABLE [dbo].[Purchases]
        ADD CONSTRAINT [CK_Purchases_Payment_NonNegative]
        CHECK ([Payment] >= 0);
END
GO

IF EXISTS (
    SELECT [ReaderID], [BookID]
    FROM [dbo].[Purchases]
    GROUP BY [ReaderID], [BookID]
    HAVING COUNT(*) > 1
)
BEGIN
    THROW 51000, 'Cannot create unique purchase index because duplicate purchases exist.', 1;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = 'UX_Purchases_Reader_Book'
      AND [object_id] = OBJECT_ID('dbo.Purchases')
)
BEGIN
    CREATE UNIQUE INDEX [UX_Purchases_Reader_Book]
        ON [dbo].[Purchases] ([ReaderID], [BookID]);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = 'IX_BookApprovals_Book_ActionDate'
      AND [object_id] = OBJECT_ID('dbo.BookApprovals')
)
BEGIN
    CREATE INDEX [IX_BookApprovals_Book_ActionDate]
        ON [dbo].[BookApprovals] ([BookID], [ActionDate] DESC);
END
GO

COMMIT TRANSACTION
GO
