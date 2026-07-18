using System.Windows.Controls;

namespace BookManagement.Views.Admin
{
    public partial class AdminBookReviewView : UserControl
    {

        private readonly BookModel _book;
        public AdminBookReviewView(BookModel book)
        {
            InitializeComponent();
            _book = book;
        }
    }
}
