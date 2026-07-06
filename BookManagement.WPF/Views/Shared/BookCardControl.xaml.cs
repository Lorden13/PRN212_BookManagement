using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookManagement.WPF.Views.Shared
{
    public partial class BookCardControl : UserControl
    {
        public BookCardControl()
        {
            InitializeComponent();
        }

        // Book DependencyProperty (type object to avoid circular references)
        public static readonly DependencyProperty BookProperty =
            DependencyProperty.Register(nameof(Book), typeof(object), typeof(BookCardControl), new PropertyMetadata(null));

        public object Book
        {
            get => GetValue(BookProperty);
            set => SetValue(BookProperty, value);
        }

        // ViewDetailsCommand
        public static readonly DependencyProperty ViewDetailsCommandProperty =
            DependencyProperty.Register(nameof(ViewDetailsCommand), typeof(ICommand), typeof(BookCardControl), new PropertyMetadata(null));

        public ICommand ViewDetailsCommand
        {
            get => (ICommand)GetValue(ViewDetailsCommandProperty);
            set => SetValue(ViewDetailsCommandProperty, value);
        }

        // FavoriteCommand
        public static readonly DependencyProperty FavoriteCommandProperty =
            DependencyProperty.Register(nameof(FavoriteCommand), typeof(ICommand), typeof(BookCardControl), new PropertyMetadata(null));

        public ICommand FavoriteCommand
        {
            get => (ICommand)GetValue(FavoriteCommandProperty);
            set => SetValue(FavoriteCommandProperty, value);
        }

        // BuyCommand
        public static readonly DependencyProperty BuyCommandProperty =
            DependencyProperty.Register(nameof(BuyCommand), typeof(ICommand), typeof(BookCardControl), new PropertyMetadata(null));

        public ICommand BuyCommand
        {
            get => (ICommand)GetValue(BuyCommandProperty);
            set => SetValue(BuyCommandProperty, value);
        }

        // RemoveFavoriteCommand
        public static readonly DependencyProperty RemoveFavoriteCommandProperty =
            DependencyProperty.Register(nameof(RemoveFavoriteCommand), typeof(ICommand), typeof(BookCardControl), new PropertyMetadata(null));

        public ICommand RemoveFavoriteCommand
        {
            get => (ICommand)GetValue(RemoveFavoriteCommandProperty);
            set => SetValue(RemoveFavoriteCommandProperty, value);
        }

        // ShowViewDetails
        public static readonly DependencyProperty ShowViewDetailsProperty =
            DependencyProperty.Register(nameof(ShowViewDetails), typeof(bool), typeof(BookCardControl), new PropertyMetadata(false));

        public bool ShowViewDetails
        {
            get => (bool)GetValue(ShowViewDetailsProperty);
            set => SetValue(ShowViewDetailsProperty, value);
        }

        // ShowFavorite
        public static readonly DependencyProperty ShowFavoriteProperty =
            DependencyProperty.Register(nameof(ShowFavorite), typeof(bool), typeof(BookCardControl), new PropertyMetadata(false));

        public bool ShowFavorite
        {
            get => (bool)GetValue(ShowFavoriteProperty);
            set => SetValue(ShowFavoriteProperty, value);
        }

        // ShowBuy
        public static readonly DependencyProperty ShowBuyProperty =
            DependencyProperty.Register(nameof(ShowBuy), typeof(bool), typeof(BookCardControl), new PropertyMetadata(false));

        public bool ShowBuy
        {
            get => (bool)GetValue(ShowBuyProperty);
            set => SetValue(ShowBuyProperty, value);
        }

        // ShowRemoveFavorite
        public static readonly DependencyProperty ShowRemoveFavoriteProperty =
            DependencyProperty.Register(nameof(ShowRemoveFavorite), typeof(bool), typeof(BookCardControl), new PropertyMetadata(false));

        public bool ShowRemoveFavorite
        {
            get => (bool)GetValue(ShowRemoveFavoriteProperty);
            set => SetValue(ShowRemoveFavoriteProperty, value);
        }
    }
}
