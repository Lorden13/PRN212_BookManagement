using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookManagement.Controls
{
    public partial class Pagination : UserControl
    {
        public static readonly DependencyProperty InfoTextProperty =
            DependencyProperty.Register(nameof(InfoText), typeof(string), typeof(Pagination), new PropertyMetadata("Showing 1 to 8 of 48 items"));

        public string InfoText
        {
            get => (string)GetValue(InfoTextProperty);
            set => SetValue(InfoTextProperty, value);
        }

        public static readonly DependencyProperty PageInfoTextProperty =
            DependencyProperty.Register(nameof(PageInfoText), typeof(string), typeof(Pagination), new PropertyMetadata("Page 1 of 1"));

        public string PageInfoText
        {
            get => (string)GetValue(PageInfoTextProperty);
            set => SetValue(PageInfoTextProperty, value);
        }

        public static readonly DependencyProperty PrevCommandProperty =
            DependencyProperty.Register(nameof(PrevCommand), typeof(ICommand), typeof(Pagination), new PropertyMetadata(null));

        public ICommand PrevCommand
        {
            get => (ICommand)GetValue(PrevCommandProperty);
            set => SetValue(PrevCommandProperty, value);
        }

        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register(nameof(NextCommand), typeof(ICommand), typeof(Pagination), new PropertyMetadata(null));

        public ICommand NextCommand
        {
            get => (ICommand)GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }

        public Pagination()
        {
            InitializeComponent();
        }
    }
}
