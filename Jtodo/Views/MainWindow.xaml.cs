using System.Windows;
using Jtodo.Services;

namespace Jtodo.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeNavigationService();
            MainFrame.Navigate(new WelcomePage());
        }

        private void InitializeNavigationService()
        {
            if (App.NavigationService is AppNavigationService navService)
            {
                // Wait for MainFrame to be loaded
                MainFrame.Loaded += (s, e) =>
                {
                    if (MainFrame.NavigationService != null)
                    {
                        navService.SetNavigationService(MainFrame.NavigationService);
                    }
                };
            }
        }
    }
}