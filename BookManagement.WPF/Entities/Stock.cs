namespace BookManagement.WPF.Entities;

public partial class Stock
{
    public string BookId { get; set; } = null!;

    public int Quantity { get; set; } = 100;

    public virtual Book Book { get; set; } = null!;
}
