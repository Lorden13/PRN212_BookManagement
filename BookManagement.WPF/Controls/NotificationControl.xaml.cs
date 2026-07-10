using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BookManagement.Controls
{
    public partial class NotificationControl : UserControl
    {
        private readonly DispatcherTimer _timer;

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                nameof(IsOpen),
                typeof(bool),
                typeof(NotificationControl),
                new PropertyMetadata(false, OnIsOpenChanged));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(NotificationControl),
                new PropertyMetadata("Thông báo"));

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                nameof(Message),
                typeof(string),
                typeof(NotificationControl),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(
                nameof(Status),
                typeof(string),
                typeof(NotificationControl),
                new PropertyMetadata("Success"));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public string Status
        {
            get => (string)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public NotificationControl()
        {
            InitializeComponent();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) }; // Auto dismiss after 4 seconds
            _timer.Tick += Timer_Tick;
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NotificationControl control)
            {
                if ((bool)e.NewValue)
                {
                    control.UpdateTitleBasedOnStatus();
                    control._timer.Stop();
                    control._timer.Start();
                }
                else
                {
                    control._timer.Stop();
                }
            }
        }

        private void UpdateTitleBasedOnStatus()
        {
            // If the title hasn't been custom set, set it based on the status
            if (Title == "Thông báo" || string.IsNullOrEmpty(Title))
            {
                switch (Status?.ToLower())
                {
                    case "success":
                    case "approved":
                    case "active":
                        Title = "Thành công";
                        break;
                    case "warning":
                    case "pending":
                        Title = "Cảnh báo";
                        break;
                    case "danger":
                    case "rejected":
                    case "inactive":
                        Title = "Lỗi";
                        break;
                    case "info":
                    default:
                        Title = "Thông tin";
                        break;
                }
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            IsOpen = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}
