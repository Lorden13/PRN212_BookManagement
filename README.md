# Book Management System

Before writing any code, read documents in this order:

1. Documents/FRONTEND_REQUIREMENTS.md
2. Documents/BOOK_MANAGEMENT_WORKFLOW.md
3. Documents/UI_SPECIFICATION.md
4. Documents/FEATURE_LIST.md
5. UI Design/

The UI Design folder contains the reference layouts.

Do not redesign the application.

Follow the UI images exactly.

Generate only Frontend.

Those notes describe the original frontend prototype. Backend modules now
coexist with the mock-backed screens; new backend code must not be replaced by
mock data.

## Backend Developer 3 module

The transactions and approval module is implemented under
`BookManagement.WPF/Services/Transactions` and contains:

- book submission, approval, rejection, feedback, and approval history;
- transactional purchases with the price captured at purchase time;
- favorite add/remove/list operations;
- download-token authorization;
- active-account, ownership, approved-book, duplicate, and state checks.

Run `Database/Backend3_Migration.sql` against `Project_PRN` before using the
new services. The migration makes `Books.Status` nullable for the Pending
state, adds `Purchases.Payment`, prevents duplicate reader/book purchases, and
adds the approval-history lookup index.
