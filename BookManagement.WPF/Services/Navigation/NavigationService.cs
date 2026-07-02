using System;
using System.Windows.Controls;

namespace BookManagement.Services.Navigation
{
    public class NavigationService
    {
        private static NavigationService _instance;
        public static NavigationService Instance => _instance ??= new NavigationService();

        private BaseViewModel _currentViewModel;
        private Frame _mainFrame;
        private Frame _contentFrame;

        public event Action<BaseViewModel> CurrentViewModelChanged;

        private NavigationService() { }

        // ViewModel-first (UserControl) Navigation
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    CurrentViewModelChanged?.Invoke(_currentViewModel);
                }
            }
        }

        // Register frames
        public void RegisterMainFrame(Frame frame)
        {
            _mainFrame = frame;
        }

        public void RegisterContentFrame(Frame frame)
        {
            _contentFrame = frame;
        }

        // Navigate using Main Frame (e.g. Login -> Dashboard)
        public void NavigateMain(object pageOrView)
        {
            if (_mainFrame != null)
            {
                _mainFrame.Navigate(pageOrView);
            }
        }

        // Navigate using Content Frame (nested dashboard views)
        public void NavigateContent(object pageOrView)
        {
            if (_contentFrame != null)
            {
                _contentFrame.Navigate(pageOrView);
            }
        }

        // Navigate by ViewModel inside the UserControl content
        public void NavigateTo(BaseViewModel viewModel)
        {
            CurrentViewModel = viewModel;
        }
    }
}
