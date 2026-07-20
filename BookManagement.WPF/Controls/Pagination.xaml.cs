using System;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Controls
{
    public partial class Pagination : UserControl
    {
        public static readonly DependencyProperty InfoTextProperty =
            DependencyProperty.Register(nameof(InfoText), typeof(string), typeof(Pagination), new PropertyMetadata("Showing 0 to 0 of 0 items"));

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

        public event RoutedEventHandler? PrevClick;
        public event RoutedEventHandler? NextClick;

        public Pagination()
        {
            InitializeComponent();
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            PrevClick?.Invoke(this, e);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            NextClick?.Invoke(this, e);
        }
    }
}
