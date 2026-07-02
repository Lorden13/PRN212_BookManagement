using System.Collections.ObjectModel;

namespace BookManagement.ViewModels.Common
{
    public class DashboardViewModelBase : BaseViewModel
    {
        private string _pageTitle = "Dashboard";
        private SidebarViewModel _sidebar;
        private BaseViewModel? _currentPageViewModel;

        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        public SidebarViewModel Sidebar
        {
            get => _sidebar;
            set => SetProperty(ref _sidebar, value);
        }

        public BaseViewModel? CurrentPageViewModel
        {
            get => _currentPageViewModel;
            set => SetProperty(ref _currentPageViewModel, value);
        }

        private string? _toastMessage;
        public string? ToastMessage
        {
            get => _toastMessage;
            set => SetProperty(ref _toastMessage, value);
        }

        private string _toastStatus = "Success";
        public string ToastStatus
        {
            get => _toastStatus;
            set => SetProperty(ref _toastStatus, value);
        }

        private bool _isToastOpen;
        public bool IsToastOpen
        {
            get => _isToastOpen;
            set => SetProperty(ref _isToastOpen, value);
        }

        public void ShowToast(string message, string status = "Success")
        {
            ToastMessage = message;
            ToastStatus = status;
            IsToastOpen = false;
            IsToastOpen = true;
        }

        public ObservableCollection<NotificationModel> Notifications { get; } = new ObservableCollection<NotificationModel>();

        public DashboardViewModelBase(UserProfileModel user, INotificationService notificationService)
        {
            _sidebar = new SidebarViewModel(user);
            notificationService.NotificationRequested += (msg, status) => ShowToast(msg, status);
        }
    }
}
