using System.Windows;
using Jtodo.Services;
using Jtodo.ViewModels;

namespace Jtodo.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize ViewModel
            _viewModel = new MainWindowViewModel(App.TodoListService, App.NavigationService);
            DataContext = _viewModel;
            
            InitializeNavigationService();
            MainFrame.Navigate(new WelcomePage());

            // Load upcoming tasks
            Loaded += async (s, e) =>
            {
                await _viewModel.LoadUpcomingTasksAsync();
            };
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
