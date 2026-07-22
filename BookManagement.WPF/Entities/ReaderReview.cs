namespace BookManagement.WPF.Entities;

public partial class ReaderReview
{
    public string ReviewId { get; set; } = null!;

    public string ReaderId { get; set; } = null!;

    public string BookId { get; set; } = null!;

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public virtual Reader Reader { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
