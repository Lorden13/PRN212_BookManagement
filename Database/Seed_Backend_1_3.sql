USE [Project_PRN];
GO

/*
    Sample data for Backend 1.3 - Transactions & Approval.
    This script is safe to run more than once because every insert is guarded.

    Test accounts (all use password: 123456):
      Admin  : seed.admin@book.local
      Author : seed.author@book.local
      Reader : seed.reader@book.local

    The script also makes Books.Status nullable when required.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- NULL represents a book waiting for Admin approval.
IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Books')
      AND name = N'Status'
      AND is_nullable = 0
)
    ALTER TABLE dbo.Books ALTER COLUMN Status bit NULL;

DECLARE @AdminRoleId varchar(400) = '7cf72ff579e86452930098a96053329d7989259ff44b03a574';
DECLARE @AuthorRoleId varchar(400) = '87a19ae962b8bfe0959282eb7478a7d5805e08387ef8cbad56';
DECLARE @ReaderRoleId varchar(400) = '2cb0529d7467b186da50ad017256929dfd2fc17f5c4d172bae';

DECLARE @AdminId varchar(400) = 'seed-admin-00000000-0000-0000-0000-000000000001';
DECLARE @AuthorId varchar(400) = 'seed-author-0000000-0000-0000-0000-000000000001';
DECLARE @ReaderId varchar(400) = 'seed-reader-0000000-0000-0000-0000-000000000001';

-- SHA-256 of "123456" plus the private key used by the application.
DECLARE @PasswordHash varchar(256) = '4254153ea9e70e2f5c6dbbd2ae2802258ddb65aab05d6163aeda556c20604a71';

BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM dbo.Role WHERE RoleID = @AdminRoleId)
        INSERT dbo.Role(RoleID, RoleName) VALUES (@AdminRoleId, 'Admin');
    IF NOT EXISTS (SELECT 1 FROM dbo.Role WHERE RoleID = @AuthorRoleId)
        INSERT dbo.Role(RoleID, RoleName) VALUES (@AuthorRoleId, 'Author');
    IF NOT EXISTS (SELECT 1 FROM dbo.Role WHERE RoleID = @ReaderRoleId)
        INSERT dbo.Role(RoleID, RoleName) VALUES (@ReaderRoleId, 'Reader');

    IF NOT EXISTS (SELECT 1 FROM dbo.Account WHERE AccountID = @AdminId OR Email = 'seed.admin@book.local')
        INSERT dbo.Account(AccountID, RoleID, Email, Password, FullName, Address, Phone, IsActive)
        VALUES (@AdminId, @AdminRoleId, 'seed.admin@book.local', @PasswordHash,
                N'Seed Administrator', N'Bình Dương', '0900000001', 1);

    IF NOT EXISTS (SELECT 1 FROM dbo.Account WHERE AccountID = @AuthorId OR Email = 'seed.author@book.local')
        INSERT dbo.Account(AccountID, RoleID, Email, Password, FullName, Address, Phone, IsActive)
        VALUES (@AuthorId, @AuthorRoleId, 'seed.author@book.local', @PasswordHash,
                N'Seed Author', N'Thành phố Hồ Chí Minh', '0900000002', 1);

    IF NOT EXISTS (SELECT 1 FROM dbo.Account WHERE AccountID = @ReaderId OR Email = 'seed.reader@book.local')
        INSERT dbo.Account(AccountID, RoleID, Email, Password, FullName, Address, Phone, IsActive)
        VALUES (@ReaderId, @ReaderRoleId, 'seed.reader@book.local', @PasswordHash,
                N'Seed Reader', N'Hà Nội', '0900000003', 1);

    -- Also repair accounts created by an older version of this seed file.
    UPDATE dbo.Account
    SET Password = @PasswordHash, IsActive = 1
    WHERE AccountID IN (@AdminId, @AuthorId, @ReaderId);

    IF EXISTS (SELECT 1 FROM dbo.Account WHERE AccountID = @AdminId)
       AND NOT EXISTS (SELECT 1 FROM dbo.Admin WHERE AdminID = @AdminId)
        INSERT dbo.Admin(AdminID) VALUES (@AdminId);

    IF EXISTS (SELECT 1 FROM dbo.Account WHERE AccountID = @AuthorId)
       AND NOT EXISTS (SELECT 1 FROM dbo.Author WHERE AuthorID = @AuthorId)
        INSERT dbo.Author(AuthorID) VALUES (@AuthorId);

    IF EXISTS (SELECT 1 FROM dbo.Account WHERE AccountID = @ReaderId)
       AND NOT EXISTS (SELECT 1 FROM dbo.Reader WHERE ReaderID = @ReaderId)
        INSERT dbo.Reader(ReaderID) VALUES (@ReaderId);

    -- NULL = Pending, 1 = Approved, 0 = Rejected.
    IF NOT EXISTS (SELECT 1 FROM dbo.Books WHERE BookID = 'seed-book-pending-001')
        INSERT dbo.Books(BookID, AuthorID, Title, Description, Category, Price, FilePath, Status, CreatedAt)
        VALUES ('seed-book-pending-001', @AuthorId, N'Sách đang chờ duyệt',
                N'Dữ liệu mẫu cho chức năng Submit Book.', N'Công nghệ', 79000.00,
                'Books/seed-pending.pdf', NULL, DATEADD(day, -2, GETDATE()));

    IF NOT EXISTS (SELECT 1 FROM dbo.Books WHERE BookID = 'seed-book-approved-001')
        INSERT dbo.Books(BookID, AuthorID, Title, Description, Category, Price, FilePath, Status, CreatedAt)
        VALUES ('seed-book-approved-001', @AuthorId, N'Sách đã được duyệt',
                N'Dữ liệu mẫu để Reader mua, yêu thích và tải sách.', N'Lập trình', 99000.00,
                'Books/seed-approved.pdf', 1, DATEADD(day, -10, GETDATE()));

    IF NOT EXISTS (SELECT 1 FROM dbo.Books WHERE BookID = 'seed-book-rejected-001')
        INSERT dbo.Books(BookID, AuthorID, Title, Description, Category, Price, FilePath, Status, CreatedAt)
        VALUES ('seed-book-rejected-001', @AuthorId, N'Sách đã bị từ chối',
                N'Dữ liệu mẫu cho lịch sử từ chối và feedback.', N'Kỹ năng', 59000.00,
                'Books/seed-rejected.pdf', 0, DATEADD(day, -8, GETDATE()));

    IF NOT EXISTS (SELECT 1 FROM dbo.BookApprovals WHERE ApprovalID = 'seed-approval-approved-001')
        INSERT dbo.BookApprovals(ApprovalID, BookID, AdminID, IsApproved, Feedback, ActionDate)
        VALUES ('seed-approval-approved-001', 'seed-book-approved-001', @AdminId, 1,
                N'Nội dung phù hợp, sách được phê duyệt.', DATEADD(day, -7, GETDATE()));

    IF NOT EXISTS (SELECT 1 FROM dbo.BookApprovals WHERE ApprovalID = 'seed-approval-rejected-001')
        INSERT dbo.BookApprovals(ApprovalID, BookID, AdminID, IsApproved, Feedback, ActionDate)
        VALUES ('seed-approval-rejected-001', 'seed-book-rejected-001', @AdminId, 0,
                N'Cần bổ sung nguồn tham khảo và chỉnh sửa nội dung.', DATEADD(day, -5, GETDATE()));

    IF NOT EXISTS (SELECT 1 FROM dbo.Purchases WHERE ReaderID = @ReaderId AND BookID = 'seed-book-approved-001')
        INSERT dbo.Purchases(PurchaseID, ReaderID, BookID, DownloadToken, IsBought, PurchasedAt)
        VALUES ('seed-purchase-00000001', @ReaderId, 'seed-book-approved-001',
                'seed-download-token-approved-book-001', 1, DATEADD(day, -1, GETDATE()));

    IF NOT EXISTS (SELECT 1 FROM dbo.Favorites WHERE ReaderID = @ReaderId AND BookID = 'seed-book-approved-001')
        INSERT dbo.Favorites(ReaderID, BookID, AddedAt)
        VALUES (@ReaderId, 'seed-book-approved-001', GETDATE());

    COMMIT TRANSACTION;

    SELECT Email, FullName, IsActive
    FROM dbo.Account
    WHERE AccountID IN (@AdminId, @AuthorId, @ReaderId);

    SELECT BookID, Title, Status, Price FROM dbo.Books WHERE BookID LIKE 'seed-book-%';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
