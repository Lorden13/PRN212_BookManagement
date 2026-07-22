using System.Windows.Controls;

namespace BookManagement.Controls
{
    public partial class BookCard : UserControl
    {
        public BookCard()
        {
            InitializeComponent();
        }

      

        private void BtnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is BookModel book)
            {
                var detailView = new ReaderBookDetailView(book);
                BookManagement.Services.Navigation.NavigationService.Instance.NavigateContent(detailView);
            }
            e.Handled = true;
        }
    }
}
