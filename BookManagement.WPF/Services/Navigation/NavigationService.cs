using System;
using System.Windows.Controls;

namespace BookManagement.Services.Navigation
{
    public class NavigationService
    {
        private static NavigationService _instance;
        public static NavigationService Instance => _instance ??= new NavigationService();

        private Frame _mainFrame;
        private Frame _contentFrame;


        private NavigationService() { }

        // ViewModel-first (UserControl) Navigation
   

        // Register frames
        public void RegisterMainFrame(Frame frame)
        {
            Console.WriteLine($"[NavigationService] RegisterMainFrame called. Frame is null? {frame == null}");
            _mainFrame = frame;
        }

        public void RegisterContentFrame(Frame frame)
        {
            Console.WriteLine($"[NavigationService] RegisterContentFrame called. Frame is null? {frame == null}");
            _contentFrame = frame;
        }

        // Navigate using Main Frame (e.g. Login -> Dashboard)
        public void NavigateMain(object pageOrView)
        {
            Console.WriteLine("NavigateMain called");

            if (_mainFrame == null)
            {
                Console.WriteLine("MainFrame NULL");
                return;
            }

            Console.WriteLine(pageOrView.GetType().Name);

            _mainFrame.Navigate(pageOrView);

            Console.WriteLine("Navigate Done");
        }

        // Navigate using Content Frame (nested dashboard views)
        public void NavigateContent(object pageOrView)
        {
            Console.WriteLine($"[NavigationService] NavigateContent called with {pageOrView?.GetType().Name}. _contentFrame is null? {_contentFrame == null}");
            if (_contentFrame != null)
            {
                bool success = _contentFrame.Navigate(pageOrView);
                Console.WriteLine($"[NavigationService] _contentFrame.Navigate returned {success}");
            }
        }

        // Navigate by ViewModel inside the UserControl content
        public void NavigateTo()
        {
           
        }

    }
}
