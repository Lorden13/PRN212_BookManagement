using System;

namespace BookManagement.WPF.Models
{
    public class ApprovalRecord
    {
        public int Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string Decision { get; set; } = string.Empty;
        public string ReviewedBy { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? RejectionReason { get; set; }
    }
}
