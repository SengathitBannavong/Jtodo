using System.Windows.Controls;
using Jtodo.ViewModels;
using Jtodo.Services;
using Jtodo.Services.Interfaces;

namespace Jtodo.Views
{
    public partial class WelcomePage : Page
    {
        private readonly WelcomeViewModel _viewModel;
        private readonly INavigationService _navigationService;

        public WelcomePage()
        {
            InitializeComponent();
            _navigationService = App.NavigationService;
            _viewModel = new WelcomeViewModel(_navigationService, App.TodoListService);
            DataContext = _viewModel;

            Loaded += (s, e) =>
            {
                if (_navigationService is AppNavigationService navService && NavigationService != null)
                {
                    navService.SetNavigationService(NavigationService);
                }
            };
        }
    }
}
