using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Controls
{
    public partial class Header : UserControl
    {
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register(
                nameof(HeaderContent),
                typeof(object),
                typeof(Header),
                new PropertyMetadata(null));

        public object HeaderContent
        {
            get => GetValue(HeaderContentProperty);
            set => SetValue(HeaderContentProperty, value);
        }

        public Header()
        {
            InitializeComponent();
        }
    }
}
