using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Controls
{
    public partial class SectionTitle : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SectionTitle), new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public SectionTitle()
        {
            InitializeComponent();
        }
    }
}
